using UnityEngine;

public class TimingBar : MonoBehaviour
{
    public RectTransform bar;
    public RectTransform targetZone;
    public float barSpeed = 300f;
    public bool isHorizontal = true;

    private bool barMoving = true;
    private float elapsed = 0f;

    private IInteractable currentTarget;
    private Player_MYJ player_MYJ;
    private InteractionUI interactionUI;

    private Interaction interactionRef;

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

    
    public void StartTimingBar(IInteractable target, Interaction interaction, Player_MYJ player, InteractionUI ui)
    {
        currentTarget = target;
        interactionRef = interaction;
        player_MYJ = player;
        interactionUI = ui;

        barMoving = true;
        elapsed = 0f;

        interactionUI.ShowTimingBar();
        player_MYJ.StartOtherWork();
    }
    private void CheckResult()
    {
        Rect barRect = new Rect(bar.anchoredPosition, bar.sizeDelta);
        Rect targetRect = new Rect(targetZone.anchoredPosition, targetZone.sizeDelta);

        if (barRect.Overlaps(targetRect))
        {
            Debug.Log("성공!");
            currentTarget?.Interact();
        }
        else
        {
            Debug.Log("실패!");
        }

        FinishTimingBar(); // Interaction으로 UI 복구
    }

    private void FinishTimingBar()
    {
        currentTarget = null;
        barMoving = false;
        elapsed = 0f;

        if (interactionRef != null)
        {
            interactionRef.ResetInteractionState();
        }

        interactionUI.ResetUI();
        player_MYJ.EndOtherWork();
    }
}
