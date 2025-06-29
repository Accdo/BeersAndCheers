using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CuttingMinigame : MonoBehaviour
{
    public Image cuttingImage; // 자를 재료 이미지
    public Image[] cutImages; // 8개의 Cut Image 배열
    public Image spacebarImage; // 스페이스바 이미지
    private int currentCutIndex = 0; // 현재 활성화할 Cut Image 인덱스
    private float lastInputTime; // 마지막 스페이스바 입력 시간
    private bool isFading = false; // 페이드 효과 활성화 여부
    private Vector2 spacebarOriginalPos; // 스페이스바 이미지 원래 위치
    [SerializeField] private TextMeshProUGUI resultText; // 결과 표시 텍스트

    public InteractionUI interactionUI;
    public CookingMinigame cookingMinigame;

    public void ImageSet(Sprite sprite,Sprite sprite2)
    {
        cuttingImage.sprite = sprite;
        cookingMinigame.cookingImage.sprite = sprite2;
    }

    public void StartCuttingMinigame()
    {
        InitSet();
        GH_GameManager.instance.player.StartOtherWork();
    }

    void Update()
    {
        // 스페이스바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastInputTime = Time.time; // 입력 시간 갱신
            isFading = false; // 페이드 효과 비활성화
            spacebarImage.color = new Color(1f, 1f, 1f, 1f); // 알파값 1로 복원

            // Cut Image 활성화
            if (currentCutIndex < cutImages.Length)
            {
                SoundManager.Instance.Play("CuttingSFX");
                cutImages[currentCutIndex].gameObject.SetActive(true);
                currentCutIndex++;
                
                if(currentCutIndex == cutImages.Length)
                {
                    resultText.text = "성공!";
                    StartCoroutine(EndGameAfterDelay()); // 2초 대기 후 종료
                }
              
            }

            // 스페이스바 이미지 이동 애니메이션
            StartCoroutine(MoveSpacebarImage());
        }

        // 1초 이상 입력 없으면 페이드 효과 시작
        if (Time.time - lastInputTime > 1f && !isFading)
        {
            isFading = true;
        }

        // 페이드 효과 적용
        if (isFading)
        {
            float alpha = Mathf.PingPong(Time.time, 1f); // 1초 주기로 알파값 0~1 반복
            spacebarImage.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    public void InitSet()
    {
        
        resultText.text = "";
        currentCutIndex = 0;

        // 모든 Cut Image를 비활성화로 초기화
        if (cutImages == null || cutImages.Length != 8)
        {
            Debug.LogError("Cut Images 배열이 올바르게 설정되지 않았습니다. 8개의 이미지를 할당해주세요.");
            return;
        }
        foreach (Image cutImage in cutImages)
        {
            cutImage.gameObject.SetActive(false);
        }

        // 스페이스바 이미지 초기화
        if (spacebarImage == null)
        {
            Debug.LogError("Spacebar Image가 할당되지 않았습니다.");
            return;
        }
        lastInputTime = Time.time;
        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
    }
    public IEnumerator MoveSpacebarImage()
    {
        // 3픽셀 아래로 이동
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        // 원래 위치로 복귀
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    private IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        interactionUI.ShowCookingMiniGameUI();
       
        cookingMinigame.StartCookingMinigame();

        //gameObject.SetActive(false); // 캔버스 비활성화

        //foreach (Image cutImage in cutImages)
        //{
        //    cutImage.gameObject.SetActive(false);
        //}

    }
}