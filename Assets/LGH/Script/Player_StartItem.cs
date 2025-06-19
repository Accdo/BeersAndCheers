using UnityEngine;

public class Player_StartItem : MonoBehaviour
{
    public InventoryManager inventory;

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
    }

    private void Start()
    {
        
        for (int i = 0; i < 10; i++)
        {
            inventory.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName("Carrot"));
            inventory.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName("Potato"));
            inventory.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName("Onion"));
        }
    }
}
