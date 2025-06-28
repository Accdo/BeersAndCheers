using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class DungeonManager : MonoBehaviour // 프리팹화, 층마다 배치
{
    public const int MAX_FLOOR = 5;
    [SerializeField] private List<GameObject> allEnemies;
    [SerializeField] private int currentFloor;
    public int CurrentFloor => currentFloor;
    [SerializeField] private List<EnemySpawner> spawnZones;
    private List<GameObject> spawnable;
    public bool IsInDungeon;
    private int remainEnemy;
    [SerializeField] private GameObject inDungeonPortal;

    private static DungeonManager instance;
    public static DungeonManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("DungeonManager");
                instance = obj.AddComponent<DungeonManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void Awake()
    {
        IsInDungeon = false;
        ChangeFloor(1);
    }

    void Start()
    {
        EventManager.Instance.AddListener(EnemyEvents.DIED, CheckRemainEnemy);
    }


    private void FilterSpawnable()
    {
        spawnable = allEnemies
        .Where(enemy => enemy.GetComponent<Enemy>().Data.SpawnZone.x <= currentFloor &&
        currentFloor <= enemy.GetComponent<Enemy>().Data.SpawnZone.y)
        .ToList();
    }

    private void SpawnEnemies()
    {
        foreach (var zone in spawnZones)
        {
            GameObject selectedEnemy = SelectRandomEnemy();
            Vector3 pos = zone.GetRandomSpawnPosition();

            if (selectedEnemy != null)
            {
                GameObject enemyObj = Instantiate(selectedEnemy);
                Enemy enemyComponent = enemyObj.GetComponent<Enemy>();

                if (enemyComponent != null)
                {
                    NavMeshAgent agent = enemyComponent.NavMeshAgent;
                    if (agent != null)
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(pos, out hit, 5f, NavMesh.AllAreas))
                        {
                            agent.Warp(hit.position);
                            enemyComponent.Init();
                        }
                        else
                        {
                            Debug.Log("스폰 위치를 찾을 수 없음");
                            Destroy(enemyObj);
                            continue;
                        }
                    }
                    else
                    {
                        Debug.Log("Navmesh is null");
                        Destroy(enemyObj);
                        continue;
                    }
                }
                else
                {
                    Debug.Log("Enemy component is null");
                    Destroy(enemyObj);
                    continue;
                }
            }
        }
    }

    private GameObject SelectRandomEnemy()
    {
        if (spawnable == null || spawnable.Count == 0)
        {
            Debug.Log("스폰 가능한 적 리스트 비어있음");
            return null;
        }

        int randomIndex = Random.Range(0, spawnable.Count);

        return spawnable[randomIndex];
    }

    public void ChangeFloor(int floor) // 몹 스폰 포함
    {
        if (floor > 0 && floor <= MAX_FLOOR)
        {
            currentFloor = floor;
            remainEnemy = spawnZones.Count;
        }
        else
        {
            Debug.Log("올바르지 않은 층수");
        }
        FilterSpawnable();
        SpawnEnemies();
    }

    private void CheckRemainEnemy(object data)
    {
        remainEnemy--;
        if (remainEnemy <= 0)
        {
            CreateNextFloorPortal();
        }
    }

    private void CreateNextFloorPortal()
    {
        Instantiate(inDungeonPortal, PlaceXYZ.DUNGEON_CLEAR_XYZ, Quaternion.identity, null); // 생성 위치 나중에 조정할 필요가 있을듯
    }
}