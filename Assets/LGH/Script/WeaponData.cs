using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Item/WeaponData", order = 1)]
public class WeaponData : ItemData
{
    public int attackPoint;
    public GameObject weaponPrefab;
}
