using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class Mushroom : MonoBehaviour, IInteractable
{
    public Item MushroomItem;
    public float respawnDelay = 5f;
    public GameObject prefabReference;

    public string GetCursorType() => "Hand";
    public string GetInteractionID() => "Mushroom";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;
    public void Interact()
    {
        Debug.Log("¹ö¼¸ Ã¤ÁýÁß");
        GH_GameManager.instance.player.inventory.Add("Backpack", MushroomItem);
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }
    public void Respawn()
    {
        objectSpawner.Instance.RespawnObject(prefabReference);
    }
}