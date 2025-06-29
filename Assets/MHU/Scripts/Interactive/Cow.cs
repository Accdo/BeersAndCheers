using System.Collections;
using UnityEngine;

public class Cow : MonoBehaviour,IInteractable
{
    public string GetCursorType() => "Cow";
    public string GetInteractionID() => "Cow";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    [Header("��ȣ�ۿ� ��Ÿ��")]
    [SerializeField] private float coolTime = 5f;

    public Item milk;

    public void Interact()
    {
        //������ ��´�
        GH_GameManager.instance.player.inventory.Add("Backpack", milk);
        SoundManager.Instance.Play("CowSFX");

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