using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;
    public int currentMoney = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"돈이 추가되었습니다. 현재 돈: {currentMoney}");
    }
    // 돈을 제거하는 메서드
    public void RemoveMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"돈이 감소되었습니다. 현재 돈: {currentMoney}");
        }
        else
        {
            Debug.LogWarning("돈이 부족합니다.");
        }
    }

}
