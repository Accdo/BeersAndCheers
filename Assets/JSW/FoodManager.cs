using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance { get; private set; }

    [Header("음식 데이터")]
    public List<FoodItem> availableFoods = new List<FoodItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public FoodItem GetRandomFood()
    {
        if (availableFoods.Count == 0) return null;
        return availableFoods[Random.Range(0, availableFoods.Count)];
    }

    public List<FoodItem> GetRandomOrder(int count)
    {
        List<FoodItem> order = new List<FoodItem>();
        for (int i = 0; i < count; i++)
        {
            FoodItem food = GetRandomFood();
            if (food != null)
                order.Add(food);
        }
        return order;
    }

    public FoodItem GetFoodByName(string name)
    {
        return availableFoods.Find(food => food.foodName == name);
    }

   
} 