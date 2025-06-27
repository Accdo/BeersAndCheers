using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CustomerSpawnManager : MonoBehaviour
{
    [Header("손님 프리팹 목록")]
    public List<GameObject> customerPrefabs;
    [Header("특별 손님 프리팹 목록")]
    public List<GameObject> specialCustomerPrefabs;
    [Header("손님 생성 위치")]
    public Transform spawnPoint;
    [Header("생성 주기(초)")]
    public float spawnInterval = 5f;

    [Header("월드 내 최대 손님 수")]
    public int maxWanderingCustomers = 10; // 배회하는 손님 수
    private int maxTotalCustomers; // 최종 최대 손님 수 (계산됨)

    [Header("퀘스트 손님 생성 확률(0~1)")]
    [Range(0f, 1f)]
    public float questCustomerSpawnChance = 0.05f;
    [Header("특별 손님 생성 확률(0~1)")]
    [Range(0f, 1f)]
    public float specialSpawnProbability = 0.05f;
    [Header("손님 생성 ON/OFF 용 문 할당")]
    public Door door; // 문이 닫히면 모든 생성 중지 (이 기능은 유지)

    private float timer = 0f;
    private HashSet<CustomerType> spawnedSpecialCustomers = new HashSet<CustomerType>();
    private bool isQuestCustomerSpawned = false;

    public static CustomerSpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 최대 손님 수 계산
        UpdateMaxTotalCustomers();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;

            // 1. 현재 월드에 있는 모든 손님 수를 센다.
            int currentCustomerCount = FindObjectsOfType<CustomerAI>().Length;

            // 2. 최대 손님 수를 넘지 않았을 때만 생성을 시도한다.
            if (currentCustomerCount < maxTotalCustomers)
            {
                // 특별 손님 또는 일반/퀘스트 손님 생성 시도
                if (Random.value < specialSpawnProbability)
                {
                    SpawnRandomSpecialCustomer();
                }
                else
                {
                    AttemptToSpawnNormalOrQuestCustomer();
                }
            }
        }
    }
    
    // 일반 또는 퀘스트 손님 생성 시도
    void AttemptToSpawnNormalOrQuestCustomer()
    {
        bool willTryToSpawnQuestGiver = !isQuestCustomerSpawned && Random.value < questCustomerSpawnChance;

        if (willTryToSpawnQuestGiver)
        {
            if (QuestManager.Instance != null && QuestManager.Instance.HasAvailableQuest())
            {
                Debug.Log("<color=green>[SpawnManager] 모든 조건 충족! 퀘스트 손님을 생성합니다.</color>");
                SpawnCustomer(true);
            }
            else
            {
                 SpawnCustomer(false);
            }
        }
        else
        {
            SpawnCustomer(false);
        }
    }
    
    // 최대 손님 수를 다시 계산하는 public 함수
    public void UpdateMaxTotalCustomers()
    {
        int seatCount = SeatManager.Instance.allSeatGroups.Sum(group => group.seats.Count);
        int queueCount = SeatManager.Instance.maxQueueSize;
        maxTotalCustomers = seatCount + queueCount + maxWanderingCustomers;
        Debug.Log($"최대 손님 수 업데이트: 좌석({seatCount}) + 대기열({queueCount}) + 배회({maxWanderingCustomers}) = {maxTotalCustomers}명");
    }

    void SpawnCustomer(bool makeAsQuestGiver)
    {
        if (customerPrefabs.Count == 0 || spawnPoint == null) return;

        int idx = Random.Range(0, customerPrefabs.Count);
        GameObject obj = Instantiate(customerPrefabs[idx], spawnPoint.position, spawnPoint.rotation);
        
        CustomerAI customerAI = obj.GetComponent<CustomerAI>();
        if (customerAI != null && makeAsQuestGiver)
        {
            customerAI.isQuestCustomer = true;
            isQuestCustomerSpawned = true;
        }
    }

    void SpawnRandomSpecialCustomer()
    {
        if (specialCustomerPrefabs.Count == 0 || spawnPoint == null) return;

        // 아직 소환되지 않은 특별 손님 찾기
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < specialCustomerPrefabs.Count; i++)
        {
            CustomerAI customerAI = specialCustomerPrefabs[i].GetComponent<CustomerAI>();
            if (customerAI != null && !spawnedSpecialCustomers.Contains(customerAI.customerType))
            {
                availableIndices.Add(i);
            }
        }

        // 소환 가능한 특별 손님이 있는 경우
        if (availableIndices.Count > 0)
        {
            int randomIdx = availableIndices[Random.Range(0, availableIndices.Count)];
            GameObject obj = Instantiate(specialCustomerPrefabs[randomIdx], spawnPoint.position, spawnPoint.rotation);
            
            // 소환된 특별 손님 기록
            CustomerAI spawnedCustomer = obj.GetComponent<CustomerAI>();
            if (spawnedCustomer != null)
            {
                spawnedSpecialCustomers.Add(spawnedCustomer.customerType);
            }
        }
    }

    // 특별 손님 제거 시 호출할 함수
    public void RemoveSpecialCustomer(CustomerType customerType)
    {
        spawnedSpecialCustomers.Remove(customerType);
    }

    // 퀘스트 손님이 떠날 때 호출될 함수
    public void OnQuestCustomerLeft()
    {
        isQuestCustomerSpawned = false;
        Debug.Log("<color=orange>[SpawnManager] 퀘스트 손님이 퇴장했으므로, 다음 퀘스트 손님 생성이 가능합니다.</color>");
    }

    //void SpawnCustomerGroup(int teamSize)
    //{
    //    CustomerGroup group = new CustomerGroup();
    //    group.members = new List<CustomerAI>();
    //    for (int i = 0; i < teamSize; i++)
    //    {
    //        int idx = Random.Range(0, customerPrefabs.Count);
    //        GameObject obj = Instantiate(customerPrefabs[idx], spawnPoint.position, spawnPoint.rotation);
    //        CustomerAI ai = obj.GetComponent<CustomerAI>();
    //        ai.myGroup = group; // CustomerAI에 myGroup 필드 필요
    //        group.members.Add(ai);
    //    }
    //    SeatManager.Instance.AddGroupToQueue(group);
    //}
}
