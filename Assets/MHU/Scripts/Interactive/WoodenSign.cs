using System.Collections;
using UnityEngine;

public class WoodenSign : MonoBehaviour,IInteractable
{
    [Header("��ȣ�ۿ� ��Ÿ��")]
    [SerializeField] private float coolTime = 30f;

    [SerializeField] private WoodenSignController woodenSignController;

    private string cursorType = "Close"; // ���� Ŀ�� Ÿ��
    private string interactionID = "Close"; // ���� ���ͷ��� ID
    private int interactionCount = 0; // ��ȣ�ۿ� Ƚ��

    public string GetCursorType() => cursorType;
    public string GetInteractionID() => interactionID;
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    private void Awake()
    {
        woodenSignController = GetComponentInParent<WoodenSignController>();
    }

    public void Interact()
    {
        // ��ȣ�ۿ� Ƚ�� ����
        interactionCount++;
        // "Open"�� "Closed" ������ ����
        cursorType = (interactionCount % 2 == 1) ? "Open" : "Close";
        interactionID = (interactionCount % 2 == 1) ? "Open" : "Close";

        // ǥ���� ȸ��
        woodenSignController.RotateWoodenSign();


        // ���̾ "Default"�� ����
        gameObject.layer = LayerMask.NameToLayer("Default");

        // ��Ÿ�� �� ���� ���̾�� �� ����
        StartCoroutine(RestoreLayerAfterDelay());
    }

    private IEnumerator RestoreLayerAfterDelay()
    {
        yield return new WaitForSeconds(coolTime);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        
        // ���� cursorType�� interactionID�� ����
        //cursorType = "Close";
        //interactionID = "Close";
    }
}