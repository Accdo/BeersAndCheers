using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Text text;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    void Start()
    {
        EventManager.Instance.AddListener(PlayerEvents.HEALTH_CHANGED, OnPlayerHealthChangedUI);
        EventManager.Instance.AddListener(PlayerEvents.DIED, OnPlayerDiedUI);
    }

    private void OnPlayerHealthChangedUI(object data)
    {
        if (data is float fData)
        {
            int hp = Mathf.FloorToInt(fData);
            text.text = string.Format("{0:D3}", hp);
        }
    }

    private void OnPlayerDiedUI(object data)
    {
        Debug.Log("플레이어 사망");

    }

}
