using UnityEngine;

public class GH_GameManager : MonoBehaviour
{
    public static GH_GameManager instance;

    public ItemManager itemManager;

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
    }
}
