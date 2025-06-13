using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [Header("UI")]
    public GameObject crosshairUI;
    public GameObject interactionUI;
    public GameObject timingBarUI;
    public InteractionUI interactionUIController;
    
    [Header("Interaction Settings")]
    public float interactRange = 3f; //��ȣ�ۿ� �Ÿ�
    public LayerMask interactLayer; // ��ȣ�ۿ� ��ü Ȯ�� layer
    public float holdDuration = 2f; //������ ä������ �ð�

    [Header("reference")]
    public TimingBar timingBar;
    public PlayerController playerController;

    private Camera cam;
    private float holdTimer = 0f;
    private bool gaugeCompleted = false;

    private void Start()
    {
        cam = Camera.main;
        crosshairUI.SetActive(true);
        interactionUI.SetActive(false);
        timingBarUI.SetActive(false);
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

                crosshairUI.SetActive(false);
                interactionUI.SetActive(true);//��� �⺻ ��ȣ�ۿ� UI
                interactionUIController.SetCursor(cursorType);
                interactionUIController.ShowGauge(true);

                if (Input.GetKey(KeyCode.E))
                {
                    holdTimer += Time.deltaTime;
                    interactionUIController.UpdateGauge(holdTimer / holdDuration);

                    if (holdTimer >= holdDuration)
                    {
                        gaugeCompleted = true;
                        ShowTimingBar();
                    }
                }
                else
                {
                    holdTimer = 0f;
                    interactionUIController.UpdateGauge(0f);
                }

                return;
            }
        }

        // ��ȣ�ۿ� ����� ���ų� �������� �Ϸ�� ��
        interactionUI.SetActive(false);

        if (!gaugeCompleted)
        {
            crosshairUI.SetActive(true);
            holdTimer = 0f;
            interactionUIController.UpdateGauge(0f);
        }
    }

    #region TimingBar(ä��)
    public void ShowTimingBar()
    {
        crosshairUI.SetActive(false);
        interactionUI.SetActive(false);
        timingBarUI.SetActive(true);

        playerController.StartOtherWork();

        timingBar?.StartTimingBar(this);
    }

    public void FinishTimingBar()
    {
        crosshairUI.SetActive(true);
        timingBarUI.SetActive(false);
        gaugeCompleted = false;
        holdTimer = 0f;
        interactionUIController.ResetUI();

        playerController.EndOtherWork();
    }
    #endregion
}
