using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeerMiniGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image beerFillImage; // Fill을 사용할 이미지
    [SerializeField] private TMP_Text resultText;

    [Header("설정")]
    [SerializeField] private float fillSpeed = 0.3f; // 차오르는 속도
    [SerializeField] private float successMin = 0.45f;
    [SerializeField] private float successMax = 0.55f;

    private bool isFilling = false;
    private bool isGameEnded = false;

    void Start()
    {
        beerFillImage.fillAmount = 0f;
        resultText.text = "";
    }

    void Update()
    {
        if (isGameEnded) return;

        // Space를 누르고 있으면 fill 증가
        if (Input.GetKey(KeyCode.Space))
        {
            isFilling = true;
            beerFillImage.fillAmount += fillSpeed * Time.deltaTime;

            // 오버플로우 체크
            if (beerFillImage.fillAmount >= 1f)
            {
                beerFillImage.fillAmount = 1f;
                EndGame(false);
            }
        }
        else if (isFilling) // 스페이스에서 손을 뗀 순간
        {
            CheckResult();
        }
    }

    void CheckResult()
    {
        float amount = beerFillImage.fillAmount;

        if (amount >= successMin && amount <= successMax)
        {
            EndGame(true);
        }
        else
        {
            EndGame(false);
        }
    }

    void EndGame(bool success)
    {
        isGameEnded = true;
        resultText.text = success ? "성공!" : "실패!";
        resultText.color = success ? Color.green : Color.red;

        // 이후 처리 (예: 버튼 클릭 시 재시작 등)
    }
}
