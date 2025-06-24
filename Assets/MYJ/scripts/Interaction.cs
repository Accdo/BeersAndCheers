using UnityEngine;
using UnityEngine.UI;


public class Interaction : MonoBehaviour
{
    public InteractionUI interactionUI;
    
    [Header("Interaction Settings")]

    public float interactRange = 3f; //상호작용 거리
    public LayerMask interactLayer; // 상호작용 물체 확인 layer
    public float holdDuration = 1.5f; //게이지 채워지는 시간


    [Header("reference")]
    public TimingBar timingBar;
    public Player_LYJ player_LYJ;

    private Camera cam;
    private float holdTimer = 0f;
    private bool gaugeCompleted = false;
    private IInteractable currentTarget;

    private void Start()
    {
        cam = Camera.main;
        interactionUI.ResetUI();
    }

    void Update()
    {
        letsinteraction();
    }

    public void letsinteraction() //상호작용
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer);

        if (hitSomething && !gaugeCompleted)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentTarget = interactable;
                interactionUI.SetCursor(interactable.GetCursorType());

                //어떤 물체와 상호작용하는지 검사하여 커서 이미지를 다르게 보여줌
                string cursorType = interactable.GetCursorType();

                switch (interactable.GetInteractionType())
                {
                    case InteractionType.Instant:
                        interactionUI.ShowCursor();
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            interactable.Interact();
                        }
                        break;

                    case InteractionType.GaugeThenTiming:
                        interactionUI.ShowGauge();
                        if (Input.GetKey(KeyCode.E))
                        {
                            holdTimer += Time.deltaTime;
                            interactionUI.UpdateGauge(holdTimer / holdDuration);
                            if (holdTimer >= holdDuration)
                            {
                                gaugeCompleted = true;
                                timingBar.StartTimingBar(currentTarget, this, player_LYJ, interactionUI);
                            }
                        }
                        else
                        {
                            holdTimer = 0f;
                            interactionUI.UpdateGauge(0f);
                        }
                        break;

                    case InteractionType.MiniGame:
                        interactionUI.ShowGauge();
                        if (Input.GetKey(KeyCode.E))
                        {
                            holdTimer += Time.deltaTime;
                            interactionUI.UpdateGauge(holdTimer / holdDuration);
                            if (holdTimer >= holdDuration)
                            {
                                gaugeCompleted = true;
                                interactable.Interact();
                                ResetInteractionState();
                            }
                        }
                        else
                        {
                            holdTimer = 0f;
                            interactionUI.UpdateGauge(0f);
                        }
                        break;
                }

                return;
            }
        }

        if (!gaugeCompleted)
        {
            interactionUI.ResetUI();
            holdTimer = 0f;
            interactionUI.UpdateGauge(0f);
        }
    }
    public void ResetInteractionState()
    {
        gaugeCompleted = false;
        holdTimer = 0f;
    }
}
