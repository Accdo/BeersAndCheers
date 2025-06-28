using UnityEngine;

public class Store : MonoBehaviour,IInteractable
{
    public string GetCursorType() => "Store";
    public string GetInteractionID() => "Store";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public UI_Manager ui_Manager;
    public Interaction interaction;
    public InteractionUI interactionUI;

    public void Interact()
    {
        ui_Manager.ToggleShopUI();

        interaction.isBusy = true;
        interactionUI.ResetUI();
    }

}
