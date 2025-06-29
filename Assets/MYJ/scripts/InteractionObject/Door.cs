using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public Transform doorHinge; // 회전할 문 Transform
    public float angleAmount = 90f; // 열고 닫는 각도
    public float openSpeed = 2f; // 회전 속도

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion startRotation;
    private Quaternion targetRotation;

    public WoodenSignController woodenSignController;

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

    public bool GetDoorState()
    {
        return isOpen;
    }
}
