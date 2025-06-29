using UnityEngine;

public class Stove : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Stove";
    public string GetInteractionID() => "Stove";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public InteractionUI interactionUI;
    public CuttingMinigame cuttingMinigame;
    public CookingMinigame cookingMinigame;
    public UI_Manager UI_manager;
    public Interaction interaction;

    public void Interact()
    {
        interaction.isBusy = true;
        interactionUI.ResetUI();

        // 레시피 시설 패널 생성
        UI_manager.ToggleRecipeUI();
    }

    public void CookingSystem(Sprite sprite, Item item)
    {
        
        interactionUI.ResetUI();
        interactionUI.ShowCuttingMiniGameUI();
        cuttingMinigame.ImageSet(sprite,item.data.icon);
        cuttingMinigame.StartCuttingMinigame();
        cookingMinigame.SetItem(item);
        UI_manager.ToggleRecipeUI();
    }
}
