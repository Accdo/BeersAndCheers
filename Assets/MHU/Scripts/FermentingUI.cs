using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FermentingUI : MonoBehaviour
{
    [SerializeField] private Image backBar; // 게이지바 배경 이미지
    [SerializeField] public Image gaugeBar; // 게이지바 이미지
    [SerializeField] private float fermentationTime = 30f; // 발효 시간 (초)
    [SerializeField] private GameObject fermentingObject; // 레이어 복구를 위한 Fermenting 오브젝트 참조
    [SerializeField] public TextMeshProUGUI successText;

    private float currentTime = 0f;
    public bool isFermenting { get; private set; } = false;
    public int fermentingBeer { get; set; } = 0;
    private Color originalGaugeColor; // 원본 게이지바 색상 저장

    private void Start()
    {
      
        
        // 원본 색상 저장
        if (gaugeBar != null)
        {
            originalGaugeColor = gaugeBar.color;
        }

        gaugeBar.gameObject.SetActive(false);
        backBar.gameObject.SetActive(false);
        successText.text = "";
    }

    private void Update()
    {
        if (isFermenting)
        {
            fermentingObject.layer = LayerMask.NameToLayer("Obstacle");
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(1f - (currentTime / fermentationTime));
            gaugeBar.fillAmount = progress;

            if (progress <= 0f)
            {
                isFermenting = false;
                fermentingBeer++;

                backBar.gameObject.SetActive(false);
                fermentingObject.layer = LayerMask.NameToLayer("Interactable");
                //successText.text = "성공";

                // 발효 완료 시 게이지바 색상을 초록색으로 변경
                gaugeBar.color = new Color(0f, 1f, 0f);
                gaugeBar.fillAmount = 1;


            }
        }
    }

    public void StartFermentation()
    {
        if (gaugeBar == null || backBar == null)
        {
            Debug.LogError("GaugeBar or BackBar is not assigned in the Inspector!");
            return;
        }

        gaugeBar.gameObject.SetActive(true);
        backBar.gameObject.SetActive(true);
        gaugeBar.fillAmount = 1f;
        currentTime = 0f;
        isFermenting = true;
        // 발효 시작 시 원본 색상으로 복원
        if (gaugeBar != null)
        {
            gaugeBar.color = originalGaugeColor;
        }
    }
}