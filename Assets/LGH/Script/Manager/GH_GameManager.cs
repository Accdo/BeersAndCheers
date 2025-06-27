using UnityEngine;

public class GH_GameManager : MonoBehaviour
{
    public static GH_GameManager instance;

    public ItemManager itemManager;
    public UI_Manager uiManager;
    public GoldManager goldManager;
    public RecipeManager recipeManager;


    public Player_LYJ player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        itemManager = GetComponent<ItemManager>();
        uiManager = GetComponent<UI_Manager>();
        goldManager = GetComponent<GoldManager>();
        recipeManager = GetComponent<RecipeManager>();

        player = FindAnyObjectByType<Player_LYJ>();
    }
}
