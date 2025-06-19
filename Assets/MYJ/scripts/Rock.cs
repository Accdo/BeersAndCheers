using UnityEngine;

public class Rock : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Hand";
    public string GetInteractionID() => "Rock";
    public InteractionType GetInteractionType() => InteractionType.GaugeThenTiming;
    public void Interact()
    {
        Debug.Log("�� ��ȣ�ۿ���...");
    }
}
