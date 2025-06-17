using UnityEngine;

public class GH_GameManager : MonoBehaviour
{
    public static GH_GameManager instance;

    public ItemManager itemManager;
    public UI_Manager uiManager;

    public Player_JSW player;

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

        player = FindAnyObjectByType<Player_JSW>();
    }
}
