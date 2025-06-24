using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour,IInteractable
{
    #region ��ȣ�ۿ�
    public string GetCursorType() => "Fish";
    public string GetInteractionID() => "Fish";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion

    [Header("Fishing Setting")]
    public float castAnimationTime = 1.5f;
    public float biteMinTime = 2f;
    public float biteMaxTime = 5f;
    public float hitDuration = 1f;

    [Header("References")]
    public string requiredToolName = "FishingRod"; //�κ��丮���� ���ô� ã�°� �ֱ�
    /*public Animator playerAnimator; //�ִϸ��̼�*/

    [SerializeField] private Player_MYJ player;
    public InteractionUI interactionUI;

    private bool isFishing = false;
    private bool canCatch = false;
    private Coroutine fishingRoutine;
    private Interaction interaction;

    public void Interact()
    {
        /* if (isFishing || !IsHoldingFishingRod()) return;*/
        if (isFishing) return;

        fishingRoutine = StartCoroutine(FishingSystem());
    }
    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.GetComponent<Player_MYJ>();
            Debug.Log($"[Fishing] Player ref found at Start: {player}");
        }

        if (interaction == null)
            interaction = GetComponentInParent<Interaction>();
    }

    IEnumerator FishingSystem()
    {
        isFishing = true;
        Debug.Log($"player reference: {player}");
        player?.StartOtherWork();
        interactionUI.ResetUI();

        /*playerAnimator.SetTrigger("Cast");*/
        Debug.Log("�ִϸ��̼� ����");

        yield return new WaitForSeconds(castAnimationTime);

        while (isFishing)
        {
            float waitTime = Random.Range(biteMinTime, biteMaxTime);
            Debug.Log($"���� ��� ��... ({waitTime:F1}��)");
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(HitWindow());

            // Hit �������� ���� ���, �����ϸ� ���� Ż��
            if (!isFishing) break;
        }
    }
    IEnumerator HitWindow()
    {
        canCatch = true;
        interactionUI.ShowFishingHitUI();
        Debug.Log($"Hit UI Ȱ��ȭ");

        float timer = 0f;
        bool hitSuccess = false;

        while (timer < hitDuration)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hitSuccess = true;
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        interactionUI.ResetFishingUI();
        canCatch = false;

        if (hitSuccess)
        {
            StartMiniGame();
        }
        else
        {
            Debug.Log("Ÿ�̹� ���� - �ٽ� ���� ���");
        }
    }
    private void StartMiniGame()
    {
        Debug.Log("�̴ϰ��� ����!");
        isFishing = false;
        interactionUI.ShowFishingMiniGameUI();

        FishingMiniGame.Instance.StartGame((success) =>
        {
            EndFishing(success);
        });
    }

    /*private bool IsHoldingFishingRod() //�κ��丮 ������ �°� ����
    {
        return InventoryManager.Instance.CurrentItem?.itemName == requiredToolName;
    }*/

    public void EndFishing(bool success)
    {
        if (success)
        {
            Debug.Log("���� ����! ������ ȹ�� ó��");
            // �κ��丮.AddItem(...);
        }
        else
        {
            Debug.Log("���� ����!");
        }

        interactionUI.ResetFishingUI();
        interactionUI.ResetUI();
        isFishing = false;
        player?.EndOtherWork();
        interaction?.ResetInteractionState();
        interaction?.letsinteraction();
    }

    private void OnDisable()
    {
        if (fishingRoutine != null) StopCoroutine(fishingRoutine);
        isFishing = false;
        canCatch = false;

        player?.EndOtherWork();
        interaction?.ResetInteractionState();
    }
}
