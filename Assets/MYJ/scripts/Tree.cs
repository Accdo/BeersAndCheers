using UnityEngine;

public class Tree : MonoBehaviour, IInteractable
{
    public float respawnDelay = 10f;
    public GameObject prefabReference;

    public string GetCursorType() => "Tree";
    public string GetInteractionID() => "Tree";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;

    public void Interact()
    {
        Debug.Log("나무 벌목 완료!");
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }

    public void Respawn()
    {
        objectSpawner.Instance.RespawnObject(prefabReference);
    }
}
