using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour,IInteractable
{
    #region 상호작용
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
    public string requiredToolName = "FishingRod"; //인벤토리에서 낚시대 찾는거 넣기
    /*public Animator playerAnimator; //애니메이션*/

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
        Debug.Log("애니메이션 실행");

        yield return new WaitForSeconds(castAnimationTime);

        while (isFishing)
        {
            float waitTime = Random.Range(biteMinTime, biteMaxTime);
            Debug.Log($"랜덤 대기 중... ({waitTime:F1}초)");
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(HitWindow());

            // Hit 실패했을 때만 계속, 성공하면 루프 탈출
            if (!isFishing) break;
        }
    }
    IEnumerator HitWindow()
    {
        canCatch = true;
        interactionUI.ShowFishingHitUI();
        Debug.Log($"Hit UI 활성화");

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
            Debug.Log("타이밍 실패 - 다시 낚시 대기");
        }
    }
    private void StartMiniGame()
    {
        Debug.Log("미니게임 시작!");
        isFishing = false;
        interactionUI.ShowFishingMiniGameUI();

        FishingMiniGame.Instance.StartGame((success) =>
        {
            EndFishing(success);
        });
    }

    /*private bool IsHoldingFishingRod() //인벤토리 로직에 맞게 수정
    {
        return InventoryManager.Instance.CurrentItem?.itemName == requiredToolName;
    }*/

    public void EndFishing(bool success)
    {
        if (success)
        {
            Debug.Log("낚시 성공! 아이템 획득 처리");
            // 인벤토리.AddItem(...);
        }
        else
        {
            Debug.Log("낚시 실패!");
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
