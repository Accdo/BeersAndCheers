using UnityEngine;

public class Cooking : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Cooking";
    public string GetInteractionID() => "Cooking";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public InteractionUI interactionUI;
    public CuttingMinigame cuttingMinigame;
    public CookingMinigame cookingMinigame;
    public UI_Manager UI_manager;
    public Interaction interaction;

    private Coroutine CookingRoutine;

    public void Interact()
    {
        interaction.isBusy = true;
        interactionUI.ResetUI();

        // 레시피 시설 패널 생성
        UI_manager.ToggleRecipeUI();
        //CookingRoutine = StartCoroutine(CookingSystem());
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

    //IEnumerator CookingSystem()
    //{
    //    interactionUI.ResetUI();
    //    isCooking = true;
    //    interactionUI.ShowCuttingMiniGameUI();
    //    cuttingMinigame.StartCuttingMinigame();

    //    yield return null;
    //}
}
