using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public InteractionUI interactionUI;
    
    [Header("Interaction Settings")]
    public float interactRange = 3f; //��ȣ�ۿ� �Ÿ�
    public LayerMask interactLayer; // ��ȣ�ۿ� ��ü Ȯ�� layer
    public float holdDuration = 1.5f; //������ ä������ �ð�

    [Header("reference")]
    public TimingBar timingBar;
    public Player_MYJ playerController;

    private Camera cam;
    private float holdTimer = 0f;
    private bool gaugeCompleted = false;

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
                string cursorType = interactable.GetCursorType();

                interactionUI.ShowGauge();
                interactionUI.SetCursor(cursorType);

                if (Input.GetKey(KeyCode.E))
                {
                    holdTimer += Time.deltaTime;
                    interactionUI.UpdateGauge(holdTimer / holdDuration);

                    if (holdTimer >= holdDuration)
                    {
                        gaugeCompleted = true;
                        ShowTimingBar();
                    }
                }
                else
                {
                    holdTimer = 0f;
                    interactionUI.UpdateGauge(0f);
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

    #region TimingBar(ä��)
    public void ShowTimingBar()
    {
        interactionUI.ShowTimingBar();

        playerController.StartOtherWork();

        timingBar?.StartTimingBar(this);
    }

    public void FinishTimingBar()
    {
        interactionUI.ResetUI();
        gaugeCompleted = false;
        holdTimer = 0f;

        playerController.EndOtherWork();
    }
    #endregion
}
