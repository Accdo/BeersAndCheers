using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    private Enemy owner;

    void Awake()
    {
        owner = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        Player_LYJ target = other.GetComponent<Player_LYJ>();
        owner.SetTarget(target);
    }

    void OnTriggerExit(Collider other)
    {
        owner.ReleaseTarget();
    }
}