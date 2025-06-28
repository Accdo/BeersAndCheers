using System.Collections;
using UnityEngine;

public class Chicken : MonoBehaviour,IInteractable
{
    public string GetCursorType() => "Chicken";
    public string GetInteractionID() => "Chicken";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    [Header("상호작용 쿨타임")]
    [SerializeField] private float coolTime = 5f;

    public Item egg;

    public void Interact()
    {
        //달걀를 얻는다
        GH_GameManager.instance.player.inventory.Add("Backpack", egg);

        // 레이어를 "notInter"로 변경
        gameObject.layer = LayerMask.NameToLayer("Default");

        // 5초 뒤 원래 레이어로 복구
        StartCoroutine(RestoreLayerAfterDelay());
    }
    IEnumerator RestoreLayerAfterDelay()
    {
        yield return new WaitForSeconds(coolTime);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

}
