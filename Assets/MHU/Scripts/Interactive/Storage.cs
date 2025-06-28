using UnityEngine;

public class Storage : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Storage";
    public string GetInteractionID() => "Storage";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public UI_Manager ui_Manager;
    public Interaction interaction;
    public InteractionUI interactionUI;

    public void Interact()
    {
        ui_Manager.ToggleStorageBoxUI();

        interaction.isBusy = true;
        interactionUI.ResetUI();
    }

}
