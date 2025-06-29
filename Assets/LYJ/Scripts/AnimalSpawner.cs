using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> allAnimals;
    [SerializeField] private Collider area;
    [SerializeField] private List<int> remainAnimals;

    private static AnimalSpawner instance;
    public static AnimalSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("AnimalSpawner");
                instance = obj.AddComponent<AnimalSpawner>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        area = GetComponent<Collider>();
    }
    void Start()
    {
        foreach (var animal in allAnimals)
        {
            PoolManager.Instance.CreatePool(animal, 5);
            for (int i = 0; i < 5; i++)
            {
                SpawnAnimal(allAnimals.IndexOf(animal));
            }
        }
    }

    private void SpawnAnimal(int index)
    {
        if (index >= allAnimals.Count)
        {
            return;
        }
        GameObject animalObj = PoolManager.Instance.GetObject(allAnimals[index]);
        Vector3 pos = GetRandomSpawnPosition();

        if (animalObj != null)
        {
            Animal animalComponent = animalObj.GetComponent<Animal>();

            if (animalComponent != null)
            {
                NavMeshAgent agent = animalComponent.NavMeshAgent;
                if (agent != null)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(pos, out hit, 5f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                        animalComponent.Init();
                    }
                    else
                    {
                        Debug.Log("스폰 위치를 찾을 수 없음");
                        PoolManager.Instance.ReturnObject(animalObj);
                        return;
                    }
                }
                else
                {
                    Debug.Log("Navmesh is null");
                    PoolManager.Instance.ReturnObject(animalObj);
                    return;
                }
            }
            else
            {
                Debug.Log("Animal component is null");
                PoolManager.Instance.ReturnObject(animalObj);
                return;
            }
        }
    }
    private Vector3 GetRandomSpawnPosition()
    {
        var b = area.bounds;
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.max.y,
            Random.Range(b.min.z, b.max.z)
        );
    }

    public void AnimalReturned(int index)
    {
        SpawnAnimal(index);
    }

}
