using UnityEngine;
using System.Collections.Generic;

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
    [Header("생성 확률(0~1)")]
    [Range(0f, 1f)]
    public float spawnProbability = 0.7f;
    [Header("특별 손님 생성 확률(0~1)")]
    [Range(0f, 1f)]
    public float specialSpawnProbability = 0.1f;
    [Header("손님 생성 ON/OFF 용 문 할당")]
    public Door door;

    private float timer = 0f;
    private HashSet<CustomerType> spawnedSpecialCustomers = new HashSet<CustomerType>();

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
    void Update()
    {
        if(door != null && !door.GetDoorState())
        {
            // 문이 닫혀있으면 손님 생성 중지
            return;
        }
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            
            // 일반 손님 생성
            if (Random.value < spawnProbability)
            {
                int teamSize = Random.Range(1, 2);
                if (SeatManager.Instance.CanAcceptNewCustomer(1))
                {
                    SpawnRandomCustomer();
                }
            }

            // 특별 손님 생성
            if (Random.value < specialSpawnProbability)
            {
                if (SeatManager.Instance.CanAcceptNewCustomer(1))
                {
                    SpawnRandomSpecialCustomer();
                }
            }
        }
    }

    void SpawnRandomCustomer()
    {
        if (customerPrefabs.Count == 0 || spawnPoint == null) return;
        int idx = Random.Range(0, customerPrefabs.Count);
        GameObject obj = Instantiate(customerPrefabs[idx], spawnPoint.position, spawnPoint.rotation);
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
