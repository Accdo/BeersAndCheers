using System.Collections;
using UnityEngine;

public class WoodenSign : MonoBehaviour,IInteractable
{
    [Header("상호작용 쿨타임")]
    [SerializeField] private float coolTime = 30f;

    [SerializeField] private WoodenSignController woodenSignController;

    private string cursorType = "Close"; // 동적 커서 타입
    private string interactionID = "Close"; // 동적 인터랙션 ID
    private int interactionCount = 0; // 상호작용 횟수

    public string GetCursorType() => cursorType;
    public string GetInteractionID() => interactionID;
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    private void Awake()
    {
        woodenSignController = GetComponentInParent<WoodenSignController>();
    }

    public void Interact()
    {
        // 상호작용 횟수 증가
        interactionCount++;
        // "Open"과 "Closed" 번갈아 설정
        cursorType = (interactionCount % 2 == 1) ? "Open" : "Close";
        interactionID = (interactionCount % 2 == 1) ? "Open" : "Close";

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
        
        // 원래 cursorType과 interactionID로 복구
        //cursorType = "Close";
        //interactionID = "Close";
    }
}