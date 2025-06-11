using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemData", order = 50)]
public class ItemData : ScriptableObject
{
    public string itemName = "Item Name";
    public Sprite icon;
}
