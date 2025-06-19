using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public Transform doorHinge; // ȸ���� �� Transform
    public float angleAmount = 90f; // ���� �ݴ� ����
    public float openSpeed = 2f; // ȸ�� �ӵ�

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion startRotation;
    private Quaternion targetRotation;

    public string GetCursorType() => "Door";
    public string GetInteractionID() => "Door";
    public InteractionType GetInteractionType() => InteractionType.Instant;

    public void Interact()
    {
        if (isAnimating) return;

        startRotation = doorHinge.rotation;

        float targetAngle = isOpen ? -angleAmount : angleAmount;
        targetRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);

        isOpen = !isOpen;
        StartCoroutine(RotateDoor());
    }

    private System.Collections.IEnumerator RotateDoor()
    {
        isAnimating = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorHinge.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        doorHinge.rotation = targetRotation;
        isAnimating = false;
    }
}
