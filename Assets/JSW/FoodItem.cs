using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "BeersAndCheers/Food Item")]
public class FoodItem : ScriptableObject
{
    [Header("기본 정보")]
    public string foodName;        // 음식 이름
    public float price;           // 가격
    public Sprite foodImage;      // 음식 이미지
    public GameObject foodPrefab;  // 음식 프리팹
    public float frashness;
    [TextArea(3, 5)]
    public string description;    // 음식 설명

}
