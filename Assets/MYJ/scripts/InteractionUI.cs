using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject crosshairUI;
    public GameObject CursorUI;
    public GameObject GaugeUI;
    public GameObject timingBarUI;

    [Header("Cursor")]
    public Image cursorImage;
    public Sprite TreeCursor;
    public Sprite RockCursor;

    [Header("Gauge")]
    public Image gaugeImage;
    public Image gaugeUnfilledImage;

    public void SetCursor(string type)
    {
        switch (type)
        {
            case "Tree":
                cursorImage.sprite = TreeCursor;
                break;
            case "Rock":
                cursorImage.sprite = RockCursor;
                break;
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
    }

    public void ResetUI()
    {
        crosshairUI.SetActive(true);
        CursorUI.SetActive(false);
        GaugeUI.SetActive(false);
        timingBarUI.SetActive(false);
    }
}
