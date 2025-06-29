using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class Well : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Well";
    public string GetInteractionID() => "Well";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public Item waterPail;

    public void Interact()
    {
        GH_GameManager.instance.player.inventory.Add("Hotbar", waterPail);
        SoundManager.Instance.Play("WaterSFX");

    }
}
