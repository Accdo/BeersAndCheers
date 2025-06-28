using UnityEngine;
using static UnityEditor.Progress;

public class Rock : MonoBehaviour, IInteractable
{
    public Item RockItem;
    public float respawnDelay = 5f;
    public GameObject prefabReference;

    public string GetCursorType() => "Hand";
    public string GetInteractionID() => "Rock";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;
    public void Interact()
    {
        Debug.Log("돌 채집중");
        GH_GameManager.instance.player.inventory.Add("Backpack", RockItem);
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }
    public void Respawn()
    {
        objectSpawner.Instance.RespawnObject(prefabReference);
    }
}
