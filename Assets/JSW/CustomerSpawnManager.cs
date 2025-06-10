using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawnManager : MonoBehaviour
{
    [Header("손님 프리팹 목록")]
    public List<GameObject> customerPrefabs;
    [Header("손님 생성 위치")]
    public Transform spawnPoint;
    [Header("생성 주기(초)")]
    public float spawnInterval = 5f;
    [Header("생성 확률(0~1)")]
    [Range(0f, 1f)]
    public float spawnProbability = 0.7f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            // 확률적으로 생성
            if (Random.value < spawnProbability)
            {
                SpawnRandomCustomer();
            }
        }
    }

    void SpawnRandomCustomer()
    {
        if (customerPrefabs.Count == 0 || spawnPoint == null) return;
        int idx = Random.Range(0, customerPrefabs.Count);
        GameObject obj = Instantiate(customerPrefabs[idx], spawnPoint.position, spawnPoint.rotation);
    }
}
