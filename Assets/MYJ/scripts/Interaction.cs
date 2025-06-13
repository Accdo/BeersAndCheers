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
    public float interactRange = 3f; //상호작용 거리
    public LayerMask interactLayer; // 상호작용 물체 확인 layer
    public float holdDuration = 2f; //게이지 채워지는 시간

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

    public void letsinteraction() //상호작용
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer);

        if (hitSomething && !gaugeCompleted)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                //어떤 물체와 상호작용하는지 검사하여 커서 이미지를 다르게 보여줌
                string cursorType = interactable.GetCursorType();

                crosshairUI.SetActive(false);
                interactionUI.SetActive(true);//얘는 기본 상호작용 UI
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

        // 상호작용 대상이 없거나 게이지가 완료된 후
        interactionUI.SetActive(false);

        if (!gaugeCompleted)
        {
            crosshairUI.SetActive(true);
            holdTimer = 0f;
            interactionUIController.UpdateGauge(0f);
        }
    }

    #region TimingBar(채집)
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
