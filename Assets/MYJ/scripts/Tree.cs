using UnityEngine;

public class Tree : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("���� ��ȣ�ۿ���...");
    }

    public string GetCursorType()
    {
        return "Tree";
    }
}
