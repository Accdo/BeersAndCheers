using UnityEngine;

public class Bush : MonoBehaviour,IInteractable
{
    public float respawnDelay = 5f;
    public GameObject prefabReference;

    public string GetCursorType() => "Hand";
    public string GetInteractionID() => "Bush";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;

    public void Interact()
    {
        Debug.Log("´ýºÒ Ã¤ÁýÁß");
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }

    public void Respawn()
    {   
        objectSpawner.Instance.RespawnObject(prefabReference);
    }

    
}
