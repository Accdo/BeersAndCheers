using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BeerMiniGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image beerFillImage; // 맥주 채우기 이미지
    [SerializeField] private TMP_Text resultText; // 결과 텍스트
    [SerializeField] private Image successImage; // 성공 시 표시 이미지
    [SerializeField] private Image spacebarImage; // 스페이스바 이미지

    [Header("설정")]
    [SerializeField] private float fillSpeed = 0.3f; // 액체 차오르는 속도
    [SerializeField] private float successMin = 0.9f; // 성공 최소값

    public Item selectBeer;

    private bool isFilling = false;
    private bool isGameEnded = false;
    private Vector2 spacebarOriginalPos; // 스페이스바 이미지 원래 위치

    public void BeerMiniGameStart()
    {
        Initialize();
    }

    private void Initialize()
    {
        GH_GameManager.instance.uiManager.ActiveHotbarUI(false);
        // 초기화
        if (beerFillImage == null || resultText == null || spacebarImage == null || successImage == null)
        {
            Debug.LogError("UI 요소가 할당되지 않았습니다.");
            return;
        }

        isGameEnded = false;
        beerFillImage.fillAmount = 0.1f;
        resultText.text = "";
        successImage.gameObject.SetActive(false);
        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (isGameEnded) return;

        // 스페이스바 입력 처리
        if (Input.GetKey(KeyCode.Space))
        {
            HandlePouring();
        }
        else if (isFilling) // 스페이스바에서 손을 뗀 순간
        {
            isFilling = false;
            CheckResult();
        }
    }

    private void HandlePouring()
    {
        // 스페이스바 애니메이션
        StartCoroutine(MoveSpacebarImage());

        spacebarImage.color = new Color(1f, 1f, 1f, 1f); // 알파값 복원
        isFilling = true;

        // 액체 채우기
        beerFillImage.fillAmount += fillSpeed * Time.deltaTime;

        // 최대값 초과 시 1로 고정
        if (beerFillImage.fillAmount > 1f)
        {
            beerFillImage.fillAmount = 1f;
        }

        // 성공 조건 확인
        CheckResult();
    }

    private void CheckResult()
    {
        float amount = beerFillImage.fillAmount;

        // 성공 조건: successMin 이상
        if (amount >= successMin)
        {
            HandleSuccessCoroutine();
        }
    }

    private void HandleSuccessCoroutine()
    {
        // 성공 처리
        successImage.gameObject.SetActive(true);
        resultText.text = "성공!";
        resultText.color = Color.green;

        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
       

        isGameEnded = true;
        GH_GameManager.instance.player.EndOtherWork();

        // 추가 처리 (예: 재시작 버튼 활성화 등)
        if (selectBeer.data is FoodData foodData)
        {
            GH_GameManager.instance.player.inventory.Add("Backpack", selectBeer);
            // 인벤 새로고침
            GH_GameManager.instance.uiManager.RefreshAll();
        }

        gameObject.SetActive(false);
        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);

    }

    private IEnumerator MoveSpacebarImage()
    {
        // 스페이스바 이미지 3픽셀 아래로 이동
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        // 원래 위치로 복귀
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }
}