using System.Collections;
using UnityEngine;

public class Chicken : MonoBehaviour,IInteractable
{
    public string GetCursorType() => "Chicken";
    public string GetInteractionID() => "Chicken";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    [Header("��ȣ�ۿ� ��Ÿ��")]
    [SerializeField] private float coolTime = 5f;

    public Item egg;

    public void Interact()
    {
        //�ް��� ��´�
        GH_GameManager.instance.player.inventory.Add("Backpack", egg);

        // ���̾ "notInter"�� ����
        gameObject.layer = LayerMask.NameToLayer("Default");

        // 5�� �� ���� ���̾�� ����
        StartCoroutine(RestoreLayerAfterDelay());
    }
    IEnumerator RestoreLayerAfterDelay()
    {
        yield return new WaitForSeconds(coolTime);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

}
