using UnityEngine;

public class Fermenting : MonoBehaviour, IInteractable
{
    #region 상호작용
    public string GetCursorType() => "Fermenting";
    public string GetInteractionID() => "Fermenting";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion


    public InteractionUI interactionUI;
    public GameObject BeerRecipeUI;

    public void Interact()
    {
        GH_GameManager.instance.player.MouseVisible(true);
        GH_GameManager.instance.player.StartOtherWork();
        BeerRecipeUI.SetActive(true);
    }

}
