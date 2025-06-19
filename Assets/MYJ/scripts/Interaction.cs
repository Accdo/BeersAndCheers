using UnityEngine;
using UnityEngine.UI;


public class Interaction : MonoBehaviour
{
    public InteractionUI interactionUI;
    
    [Header("Interaction Settings")]
    public float interactRange = 3f; //��ȣ�ۿ� �Ÿ�
    public LayerMask interactLayer; // ��ȣ�ۿ� ��ü Ȯ�� layer
    public float holdDuration = 1f; //������ ä������ �ð�

    [Header("reference")]
    public TimingBar timingBar;
    public Player_MYJ player_MYJ;

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

    public void letsinteraction() //��ȣ�ۿ�
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer);

        if (hitSomething && !gaugeCompleted)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                //� ��ü�� ��ȣ�ۿ��ϴ��� �˻��Ͽ� Ŀ�� �̹����� �ٸ��� ������
                currentTarget = interactable;
                interactionUI.SetCursor(interactable.GetCursorType());

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
                                timingBar.StartTimingBar(currentTarget, this, player_MYJ, interactionUI);
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
