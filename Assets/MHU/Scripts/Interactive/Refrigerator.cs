using UnityEngine;

public class Refrigerator : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Refrigerator";
    public string GetInteractionID() => "Refrigerator";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public UI_Manager ui_Manager;
    public Interaction interaction;
    public InteractionUI interactionUI;

  
    public void Interact()
    {
        SoundManager.Instance.Play("WoodBoxSFX");
        ui_Manager.ToggleFreezeBoxUI();

        interaction.isBusy = true;
        interactionUI.ResetUI();
    }

}
