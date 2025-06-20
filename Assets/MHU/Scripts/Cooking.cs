using System.Collections;
using UnityEngine;

public class Cooking : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Cooking";
    public string GetInteractionID() => "Cooking";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    public InteractionUI interactionUI;
    public CuttingMinigame cuttingMinigame;

    private bool isCooking = false;

    private Coroutine CookingRoutine;

    public void Interact()
    {
        if (isCooking) return;
        CookingRoutine = StartCoroutine(CookingSystem());
    }

    IEnumerator CookingSystem()
    {
        interactionUI.ResetUI();
        isCooking = true;
        interactionUI.ShowCuttingMiniGameUI();
        cuttingMinigame.StartCuttingMinigame();

        yield return null;
    }
}
