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
    private Player_LYJ player_LYJ;
    private InteractionUI interactionUI;

    private Interaction interactionRef;

    public int requiredSuccesses = 5;
    private bool isTree = false;
    private int successCount = 0;
    private int failCount = 0;

    private void Update()
    {
        if (!barMoving) return;

        elapsed += Time.deltaTime;

        float range = 180f; // ��ü �̵� ���� (90 + 90)
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

    
    public void StartTimingBar(IInteractable target, Interaction interaction, Player_LYJ player, InteractionUI ui)
    {
        currentTarget = target;
        interactionRef = interaction;
        player_LYJ = player;
        interactionUI = ui;

        isTree = target is Tree;
        successCount = 0;
        failCount = 0;

        barMoving = true;
        elapsed = 0f;

        interactionUI.ShowTimingBar();
        player_LYJ.StartOtherWork();

        RandomizeTargetZone();
    }
    private void CheckResult()
    {
        Rect barRect = new Rect(bar.anchoredPosition, bar.sizeDelta);
        Rect targetRect = new Rect(targetZone.anchoredPosition, targetZone.sizeDelta);

        if (barRect.Overlaps(targetRect))
        {
            Debug.Log("����!");
            successCount++;

            if (isTree && successCount >= 5)
            {
                currentTarget?.Interact();
                FinishTimingBar();
            }
            else if (!isTree)
            {
                currentTarget?.Interact();
                FinishTimingBar();
            }
            else
            {
                // ���� Ÿ�̹� �õ�
                elapsed = 0f;
                barMoving = true;
                RandomizeTargetZone(); // ���� ��ġ�� ���� Ÿ�̹�
            }
        }
        else
        {
            Debug.Log("����!");
            failCount++;

            if (isTree && failCount >= 3)
            {
                Debug.Log("���� ���� ����!");
                FinishTimingBar();
            }
            else if (!isTree)
            {
                FinishTimingBar();
            }
            else
            {
                elapsed = 0f;
                barMoving = true;
                RandomizeTargetZone();
            }
        }
    }

    private void RandomizeTargetZone()
    {
        float range = 180f - targetZone.sizeDelta.x; // ��ü ���� - Ÿ�� �ʺ�
        float randomX = Random.Range(-range / 2f, range / 2f);

        if (isHorizontal)
            targetZone.anchoredPosition = new Vector2(randomX, targetZone.anchoredPosition.y);
        else
            targetZone.anchoredPosition = new Vector2(targetZone.anchoredPosition.x, randomX); // ������ ���
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
        player_LYJ.EndOtherWork();
    }
}
