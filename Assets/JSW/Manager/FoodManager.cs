using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance { get; private set; }

    [Header("음식 데이터")]
    public List<FoodData> availableFoods = new List<FoodData>();

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

    public FoodData GetRandomFood()
    {
        if (availableFoods.Count == 0) return null;
        return availableFoods[Random.Range(0, availableFoods.Count)];
    }

    public List<FoodData> GetRandomOrder(int count)
    {
        List<FoodData> order = new List<FoodData>();
        for (int i = 0; i < count; i++)
        {
            FoodData food = GetRandomFood();
            if (food != null)
                order.Add(food);
        }
        return order;
    }

    public FoodData GetFoodByName(string name)
    {
        return availableFoods.Find(food => food.itemName == name);
    }

    public void UnlockFood(FoodData food)
    {
        if (!availableFoods.Contains(food))
        {
            availableFoods.Add(food);
        }
    }
} 