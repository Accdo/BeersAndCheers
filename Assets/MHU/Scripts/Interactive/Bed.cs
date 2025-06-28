using UnityEngine;
using static Inventory;

public class Bed : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Bed";
    public string GetInteractionID() => "Bed";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public void Interact()
    {
        //잠에 든다.

    }
}
