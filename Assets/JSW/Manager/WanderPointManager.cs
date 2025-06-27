using UnityEngine;
using System.Collections.Generic;

public class WanderPointManager : MonoBehaviour
{
    public static WanderPointManager Instance { get; private set; }

    public List<Transform> wanderPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetRandomWanderPoint()
    {
        if (wanderPoints == null || wanderPoints.Count == 0)
        {
            Debug.LogWarning("배회 지점이 설정되지 않았습니다!");
            return Vector3.zero;
        }
        int randomIndex = Random.Range(0, wanderPoints.Count);
        return wanderPoints[randomIndex].position;
    }
}