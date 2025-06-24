using UnityEngine;

public class BeerRecipeUI : MonoBehaviour
{
    public GameObject BeerRecipeUIObject;

    public void SelectRecipe(Item selectedRecipe)
    {
        // 레시피 선택 시 호출되는 메서드

        GH_GameManager.instance.uiManager.cookingUI.SelectItem = selectedRecipe;
    }

    public void ToggleBeerRecipeUI()
    {
        if (BeerRecipeUIObject != null)
        {
            if (!BeerRecipeUIObject.activeSelf)
            {
                GH_GameManager.instance.player.MouseVisible(true);
                GH_GameManager.instance.player.StartOtherWork();
                BeerRecipeUIObject.SetActive(true);
            }
            else
            {
                GH_GameManager.instance.player.MouseVisible(false);
                GH_GameManager.instance.player.EndOtherWork();
                BeerRecipeUIObject.SetActive(false);
            }
        }

    }

}
