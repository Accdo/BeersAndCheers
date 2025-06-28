using UnityEngine;
using static UnityEditor.Progress;

public class Bush : MonoBehaviour,IInteractable
{
    public Item Blueberryitem;
    public Item Strawberryitem;

    public float respawnDelay = 5f;
    public GameObject prefabReference;

    public string GetCursorType() => "Hand";
    public string GetInteractionID() => "Bush";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;

    public void Interact()
    {
        Debug.Log("덤불 채집중");

        Item randomItem = Random.value < 0.5f ? Blueberryitem : Strawberryitem;

        int randomCount = Random.Range(1, 4);

        for (int i = 0; i < randomCount; i++)
        {
            GH_GameManager.instance.player.inventory.Add("Backpack", randomItem);
        }
       
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }

    public void Respawn()
    {   
        objectSpawner.Instance.RespawnObject(prefabReference);
    }

    
}
