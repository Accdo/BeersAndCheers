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
    public Player_MYJ player_MYJ;

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

    #region TimingBar(채집)
    public void ShowTimingBar()
    {
        interactionUI.ShowTimingBar();

        player_MYJ.StartOtherWork();

        timingBar?.StartTimingBar(this);
    }

    public void FinishTimingBar()
    {
        interactionUI.ResetUI();
        gaugeCompleted = false;
        holdTimer = 0f;

        player_MYJ.EndOtherWork();
    }
    #endregion
}
