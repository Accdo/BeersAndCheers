using UnityEngine;

public class Well : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Well";
    public string GetInteractionID() => "Well";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    public void Interact()
    {
        Debug.Log("�� ��ȣ�ۿ���...");
    }
}
