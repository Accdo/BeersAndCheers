using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Collider area;
    public Vector3 GetRandomSpawnPosition()
    {
        var b = area.bounds;
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.min.y,
            Random.Range(b.min.z, b.max.z)
        );
    }
}
