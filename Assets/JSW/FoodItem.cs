using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "BeersAndCheers/Food Item")]
public class FoodItem : ScriptableObject
{
    [Header("기본 정보")]
    public string foodName;        // 음식 이름
    public float price;           // 가격
    public Sprite foodImage;      // 음식 이미지
    public GameObject foodPrefab;  // 음식 프리팹
    [TextArea(3, 5)]
    public string description;    // 음식 설명

    [Header("시간 설정")]
    public float preparationTime; // 준비 시간
    public float eatingTime;      // 식사 시간

    [Header("음식 타입")]
    public FoodType foodType;     // 음식 타입
}

public enum FoodType
{
    Beer,       // 맥주
    Food,       // 안주
    Dessert     // 디저트
} 