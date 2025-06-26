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
    public FermentingUI fermentingUI;
    public CraftManager craftManager;
    public UI_Manager ui_Manager;

    private void Start()
    {
        BeerRecipeUI.SetActive(false);
    }

    public void Interact()
    {
        // 발효중이 아니고
        if (!fermentingUI.isFermenting)
        {
            // 발효된게 0개이면
            if (fermentingUI.fermentingBeer == 0)
            {
                // 음료 제작 레시피 보이게
                ui_Manager.ToggleBeerUI();

            }
            // 발효된게 1개 이상이면
            else
            {
                // 맥주통 크래프팅 모드 실행
                craftManager.StartPlacement();
                // 발효 개수 초기화
                fermentingUI.fermentingBeer = 0;
            }

        }
        else
        {


        }
    }
}
