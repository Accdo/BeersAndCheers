using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    public GameObject[] recipeLock;
    public GameObject[] recipeSlot;

    public void UnlockRecipe(string recipeName)
    {
        switch (recipeName)
        {
            case "Sandwich":
                recipeLock[0].SetActive(false);
                recipeSlot[0].GetComponent<Button>().enabled = true;
                break;
            case "Pancake":
                recipeLock[1].SetActive(false);
                recipeSlot[1].GetComponent<Button>().enabled = true;
                break;
            case "PufferFishBall":
                recipeLock[2].SetActive(false);
                recipeSlot[2].GetComponent<Button>().enabled = true;
                break;
            case "StarfishPasta":
                recipeLock[3].SetActive(false);
                recipeSlot[3].GetComponent<Button>().enabled = true;
                break;
            default:
                Debug.LogWarning("Unknown recipe name: " + recipeName);
                break;
        }
    }
}
