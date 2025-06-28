using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GatherableData
{
    public GameObject prefab;
    public int poolSize = 10;
}

public class objectSpawner : MonoBehaviour
{
    public static objectSpawner Instance;

    public List<GatherableData> gatherableTypes; // 인스펙터에서 prefab 설정
    public BoxCollider spawnArea;

    private Dictionary<GameObject, List<GameObject>> poolByPrefab = new();

    void Awake() => Instance = this;

    void Start()
    {
        foreach (var data in gatherableTypes)
        {
            List<GameObject> pool = new();
            for (int i = 0; i < data.poolSize; i++)
            {
                GameObject obj = Instantiate(data.prefab);
                obj.SetActive(false);

                if (obj.TryGetComponent(out Bush bush))
                {
                    bush.prefabReference = data.prefab;
                }

                pool.Add(obj);
            }
            poolByPrefab[data.prefab] = pool;

            // 초기 스폰
            for (int i = 0; i < data.poolSize; i++)
                SpawnObject(data.prefab);
        }
    }

    public void SpawnObject(GameObject prefab)
    {
        if (!poolByPrefab.ContainsKey(prefab)) return;

        var pool = poolByPrefab[prefab];
        GameObject obj = pool.Find(o => !o.activeInHierarchy);
        if (obj == null) return;

        obj.transform.position = GetRandomPosition();
        obj.SetActive(true);

        Debug.Log("오브젝트 리스폰 완료: " + obj.name);
    }

    public void RespawnObject(GameObject prefab)
    {
        SpawnObject(prefab);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 center = spawnArea.transform.TransformPoint(spawnArea.center);
        Vector3 size = spawnArea.size;

        Vector3 localOffset = new Vector3(
            Random.Range(-size.x / 2f, size.x / 2f),
            0,
            Random.Range(-size.z / 2f, size.z / 2f)
        );

        Vector3 worldOffset = spawnArea.transform.TransformDirection(localOffset);
        Vector3 worldPos = center + worldOffset;

        if (Terrain.activeTerrain)
            worldPos.y = Terrain.activeTerrain.SampleHeight(worldPos) + Terrain.activeTerrain.transform.position.y;

        return worldPos;
    }
}
