using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class DungeonManager : MonoBehaviour // 프리팹화, 층마다 배치
{
    [SerializeField] private List<GameObject> allEnemies;
    [SerializeField] private int currentFloor;
    [SerializeField] private List<EnemySpawner> spawnZones;
    private List<GameObject> spawnable;

    void Awake()
    {
        FilterSpawnable();
        SpawnEnemies();
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

}