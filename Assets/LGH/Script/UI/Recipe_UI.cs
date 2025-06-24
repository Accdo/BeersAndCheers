using UnityEngine;
using System.Collections.Generic;

public class Recipe_UI : MonoBehaviour
{
    //public List<Item> recipeArray = new List<Item>();

    public void SelectRecipe(Item selectedRecipe)
    {
        // 레시피 선택 시 호출되는 메서드

        GH_GameManager.instance.uiManager.cookingUI.SelectItem = selectedRecipe;
    }
}
