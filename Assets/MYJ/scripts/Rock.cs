using UnityEngine;

public class Rock : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("�� ��ȣ�ۿ���...");
    }

    public string GetCursorType()
    {
        return "Rock";
    }
}
