using UnityEngine;

public class Tree : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("나무 상호작용중...");
    }

    public string GetCursorType()
    {
        return "Tree";
    }
}
