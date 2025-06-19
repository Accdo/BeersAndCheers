using UnityEngine;

public class Bush : MonoBehaviour,IInteractable
{
    public string GetCursorType() => "Hand";
    public string GetInteractionID() => "Bush";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;

    public void Interact()
    {
        Debug.Log("덤불 채집중");
    }
}
