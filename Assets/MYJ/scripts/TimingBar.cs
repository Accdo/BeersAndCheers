using UnityEngine;

public class TimingBar : MonoBehaviour
{
    public RectTransform bar;
    public RectTransform targetZone;
    public float barSpeed = 300f;
    public bool isHorizontal = true;

    private bool barMoving = true;
    private float elapsed = 0f;
    private Interaction interactionRef;

    public void StartTimingBar(Interaction interaction)
    {
        interactionRef = interaction;
        barMoving = true;
        elapsed = 0f;
    }
    private void Update()
    {
        if (!barMoving) return;

        elapsed += Time.deltaTime;

        float range = 180f; // 전체 이동 범위 (90 + 90)
        float position = Mathf.PingPong(elapsed * barSpeed, range) - range / 2f;

        if (isHorizontal)
            bar.anchoredPosition = new Vector2(position, bar.anchoredPosition.y);
        else
            bar.anchoredPosition = new Vector2(bar.anchoredPosition.x, position);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            barMoving = false;
            CheckResult();
        }
    }

    private void CheckResult()
    {
        Rect barRect = new Rect(bar.anchoredPosition, bar.sizeDelta);
        Rect targetRect = new Rect(targetZone.anchoredPosition, targetZone.sizeDelta);

        if (barRect.Overlaps(targetRect))
        {
            Debug.Log("성공!");
        }
        else
        {
            Debug.Log("실패!");
        }

        interactionRef?.FinishTimingBar(); // Interaction으로 UI 복구
    }
}
