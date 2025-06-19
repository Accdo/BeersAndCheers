using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CursorData
{
    public string cursorType;
    public Sprite cursorSprite;
}

public class InteractionUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject crosshairUI;
    public GameObject CursorUI;
    public GameObject GaugeUI;
    public GameObject timingBarUI;
    public GameObject FishingGameUI;

    [Header("Cursor")]
    public Image cursorImage;

    [SerializeField]
    private List<CursorData> cursorDataList;

    private Dictionary<string, Sprite> cursorDict;

    [Header("Gauge")]
    public Image gaugeImage;
    public Image gaugeUnfilledImage;

    [Header("Fishing UI")]
    public GameObject fishingUI;
    public GameObject hitUI;
    public GameObject fishingGameUI;

    private void Awake()
    {
        cursorDict = new Dictionary<string, Sprite>();

        foreach(var data in cursorDataList)
        {
            if (!cursorDict.ContainsKey(data.cursorType))
                cursorDict.Add(data.cursorType, data.cursorSprite);
        }
    }

    public void SetCursor(string type)
    {
        if (cursorDict.TryGetValue(type, out Sprite sprite))
        {
            cursorImage.sprite = sprite;
        }
    }

    public void ShowCursor()
    {
        CursorUI.SetActive(true);
    }

    public void ShowGauge()
    {
        crosshairUI.SetActive(false);
        CursorUI.SetActive(true);
        GaugeUI.SetActive(true);
        timingBarUI.SetActive(false);
        FishingGameUI.SetActive(false);
    }

    public void UpdateGauge(float amount)
    {
        gaugeImage.fillAmount = Mathf.Clamp01(amount);
    }

    public void ShowTimingBar()
    {
        crosshairUI.SetActive(false);
        CursorUI.SetActive(false);
        GaugeUI.SetActive(false);
        timingBarUI.SetActive(true);
        FishingGameUI.SetActive(false);
    }
    public void ShowFishingHitUI()
    {
        crosshairUI.SetActive(false);
        CursorUI.SetActive(false);
        GaugeUI.SetActive(false);
        timingBarUI.SetActive(false);

        fishingUI.SetActive(true);
        hitUI.SetActive(true);
        fishingGameUI.SetActive(false);
    }

    public void ShowFishingMiniGameUI()
    {
        crosshairUI.SetActive(false);
        CursorUI.SetActive(false);
        GaugeUI.SetActive(false);
        timingBarUI.SetActive(false);

        fishingUI.SetActive(true);
        hitUI.SetActive(false);
        fishingGameUI.SetActive(true);
    }

    public void ResetFishingUI()
    {
        fishingUI.SetActive(false);
        hitUI.SetActive(false);
        fishingGameUI.SetActive(false);
    }

    public void ResetUI()
    {
        crosshairUI.SetActive(true);
        CursorUI.SetActive(false);
        GaugeUI.SetActive(false);
        timingBarUI.SetActive(false);
        ResetFishingUI();
    }
}
