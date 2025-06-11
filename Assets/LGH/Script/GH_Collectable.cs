using UnityEngine;

[RequireComponent(typeof(Item))]
public class GH_Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GH_Player player = other.GetComponent<GH_Player>();

        if (player)
        {
            Item item = GetComponent<Item>();

            if (item != null)
            {
                player.inventory.Add(item);
                Destroy(this.gameObject);
            }
        }
    }
}
