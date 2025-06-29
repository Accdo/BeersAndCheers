using System.Collections;
using UnityEngine;

public class WoodenSign : MonoBehaviour, IInteractable
{
    public string GetCursorType() => cursorType;
    public string GetInteractionID() => interactionID;
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    [Header("��ȣ�ۿ� ��Ÿ��")]
    [SerializeField] private float coolTime = 30f;

    [Header("�ڵ� �Ҵ�")]
    [SerializeField] private WoodenSignController woodenSignController;

    private string cursorType = "Open"; // ���� Ŀ�� Ÿ��
    private string interactionID = "Open"; // ���� ���ͷ��� ID
    public bool isOpen { get; private set; } = false; // Close

    private void Awake()
    {
        woodenSignController = GetComponentInParent<WoodenSignController>();
    }

    public void Interact()
    {
        SoundManager.Instance.Play("WoodSFX");
        isOpen = !isOpen;

        // Close�� �ٲٸ� ������ ��
        if (!isOpen) // Close
        {
            // ex) �� 12�� �Ǳ�

            cursorType = "Open"; // ��ȣ�ۿ� �� �������� �ٲ۴ٴ� Ŀ�� ǥ��
            interactionID = "Open";
        }
        else
        {
            cursorType = "Close";
            interactionID = "Close";
        }

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
    }
}