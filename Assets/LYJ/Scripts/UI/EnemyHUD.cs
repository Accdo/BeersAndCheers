using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    private Slider slider;
    private RectTransform rect;
    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        EventManager.Instance.AddListener(EnemyEvents.HEALTH_CHANGED, OnEnemyHealthChangedUI);
        EventManager.Instance.AddListener(EnemyEvents.DIED, OnEnemyDiedUI);
    }
    
    private void OnEnemyHealthChangedUI(object data)
    {
        if (data is float fData)
        {
            slider.value = fData;
        }
    }

    private void OnEnemyDiedUI(object data)
    {
        Debug.Log("적 사망");
    }

}
