using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
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
    public void SetCursorVisible(bool visible)
    {
        cursorImage.enabled = visible;
    }

    public void UpdateGauge(float amount)
    {
        gaugeImage.fillAmount = Mathf.Clamp01(amount);
    }

    public void ShowGauge(bool show)
    {
        gaugeImage.enabled = show;
        gaugeUnfilledImage.enabled = show;
    }

    public void ResetUI()
    {
        SetCursorVisible(true);
        UpdateGauge(0f);
        ShowGauge(false);
    }
}
