using UnityEngine;

public class Rock : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("돌 상호작용중...");
    }

    public string GetCursorType()
    {
        return "Rock";
    }
}
