using UnityEngine;
using UnityEngine.UI;

public enum Recipe
{
    Sandwitch,
    Pancake,
    Pufferfishball,
    StarfishPasta
}

public class RecipeManager : MonoBehaviour
{
    public GameObject[] recipeLock;
    public GameObject[] recipeSlot;

    public void UnlockRecipe(Recipe recipe)
    {
        switch (recipe)
        {
            case Recipe.Sandwitch:
                recipeLock[0].SetActive(false);
                recipeSlot[0].GetComponent<Button>().enabled = true;
                break;
            case Recipe.Pancake:
                recipeLock[1].SetActive(false);
                recipeSlot[1].GetComponent<Button>().enabled = true;
                break;
            case Recipe.Pufferfishball:
                recipeLock[2].SetActive(false);
                recipeSlot[2].GetComponent<Button>().enabled = true;
                break;
            case Recipe.StarfishPasta:
                recipeLock[3].SetActive(false);
                recipeSlot[3].GetComponent<Button>().enabled = true;
                break;
            default:
                Debug.LogWarning("Unknown recipe name: " + recipe);
                break;
        }
    }
}
