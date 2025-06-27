using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    // [SerializeField] private GameObject parentEnemy;
    private Slider slider;
    private RectTransform rect;
    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        // EventManager.Instance.AddListener(EnemyEvents.HEALTH_CHANGED, OnEnemyHealthChangedUI);
        // EventManager.Instance.AddListener(EnemyEvents.DIED, OnEnemyDiedUI);
    }

    public void ManuallyChangeHealth(float health)
    {
        slider.value = health;
    }

    // private void OnEnemyHealthChangedUI(object data)
    // {
    //     if (data is object[] eData && eData[0] is GameObject eObj && eData[1] is float eHealth)
    //     {
    //         if (parentEnemy == eObj)
    //         {
    //             slider.value = eHealth;
    //         }
    //     }
    // }

    // private void OnEnemyDiedUI(object data)
    // {
    //     Debug.Log("적 사망");
    // }

    

}
