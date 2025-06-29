using System.Collections;
using UnityEngine;

public class WoodenSign : MonoBehaviour, IInteractable
{
    public string GetCursorType() => cursorType;
    public string GetInteractionID() => interactionID;
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    [Header("상호작용 쿨타임")]
    [SerializeField] private float coolTime = 30f;

    [Header("자동 할당")]
    [SerializeField] private WoodenSignController woodenSignController;

    private string cursorType = "Open"; // 동적 커서 타입
    private string interactionID = "Open"; // 동적 인터랙션 ID
    public bool isOpen { get; private set; } = false; // Close

    public static WoodenSign instance { get; private set; }

    private void Awake()
    {
        woodenSignController = GetComponentInParent<WoodenSignController>();
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        SoundManager.Instance.Play("WoodSFX");
        isOpen = !isOpen;

        // Close로 바꾸면 실행할 것
        if (!isOpen) // Close
        {
            // ex) 밤 12시 되기

            cursorType = "Open"; // 상호작용 시 오픈으로 바꾼다는 커서 표시
            interactionID = "Open";
        }
        else
        {
            cursorType = "Close";
            interactionID = "Close";
        }

        // 표지판 회전
        woodenSignController.RotateWoodenSign();

        // 레이어를 "Default"로 변경
        gameObject.layer = LayerMask.NameToLayer("Default");

        // 쿨타임 후 원래 레이어와 값 복구
        StartCoroutine(RestoreLayerAfterDelay());
    }

    private IEnumerator RestoreLayerAfterDelay()
    {
        yield return new WaitForSeconds(coolTime);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

}