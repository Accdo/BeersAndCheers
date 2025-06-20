using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CookingMinigame : MonoBehaviour
{
    [Header("게임 설정")]
    [SerializeField] private float timerDuration = 15f; // 타이머 지속 시간 (초 단위)
    [SerializeField] private float[] rotationSpeeds = { 300f, 400f, 500f }; // 레벨별 회전 속도

    [Header("UI 요소")]
    [SerializeField] private Image spacebarImage; // 스페이스바 이미지
    [SerializeField] private Image cookingImage; // 요리 중인 음식 이미지
    [SerializeField] private Image[] perfectZoneImages; // 각 레벨의 퍼펙트 존 이미지
    [SerializeField] private Image lineImage; // 회전하는 선 이미지
    [SerializeField] private TextMeshProUGUI resultText; // 결과 표시 텍스트

    private readonly float[] perfectZoneAngles = { 88f, 44f, 22f }; // 레벨별 성공 각도
    private Vector2 spacebarOriginalPos; // 스페이스바 이미지의 원래 위치
    private int currentLevel = 0; // 현재 레벨 (0 = 1레벨, 1 = 2레벨, 2 = 3레벨)
    private float currentLineAngle; // 선 이미지의 현재 회전 각도
    private float perfectZoneStartAngle; // 퍼펙트 존의 시작 각도
    private float currentRotationSpeed; // 현재 회전 속도
    private bool isClockwise = true; // 회전 방향 (true: 시계 방향, false: 반시계 방향)
    private Coroutine colorChangeCoroutine; // 색상 변화 코루틴
    private bool canSpacebar = true; // 스페이스바 입력 가능 여부

    public InteractionUI interactionUI;

    // 게임 시작 시 초기화
    public void StartCookingMinigame()
    {
        InitSet();
        InitializeUI(); // UI 초기화
        StartLevel(currentLevel); // 첫 레벨 시작
        colorChangeCoroutine = StartCoroutine(ChangeCookingImageColor()); // 색상 변화 코루틴 시작
    }

    // 매 프레임 업데이트
    private void Update()
    {
        if (!canSpacebar) return; // 입력 비활성화 시 업데이트 중지

        // 선 이미지 회전
        float rotationDelta = currentRotationSpeed * Time.deltaTime * (isClockwise ? -1f : 1f);
        currentLineAngle = (currentLineAngle + rotationDelta) % 360f;
        if (currentLineAngle < 0f) currentLineAngle += 360f; // 0~360도 유지
        lineImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, -currentLineAngle);

        // 스페이스바 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpacebarInput(); // 스페이스바 입력 처리
        }
    }

    // UI 요소 초기화
    private void InitializeUI()
    {
        // UI 요소 검증
        if (cookingImage == null || perfectZoneImages == null || perfectZoneImages.Length != 3 ||
            lineImage == null || resultText == null)
        {
            Debug.LogError("모든 UI 요소를 인스펙터에 할당해야 합니다.");
            enabled = false;
            return;
        }

        // 스페이스바 원래 위치 저장
        if (spacebarImage != null)
        {
            spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
        }

        // 모든 퍼펙트 존 이미지 비활성화
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        resultText.text = ""; // 결과 텍스트 초기화
    }

    private void InitSet()
    {
        currentLevel = 0;
        cookingImage.color = Color.white;
    }

    // 지정된 레벨 시작
    private void StartLevel(int level)
    {
        // 모든 퍼펙트 존 이미지 비활성화
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        // 현재 레벨의 퍼펙트 존 활성화
        perfectZoneImages[level].gameObject.SetActive(true);
        currentRotationSpeed = rotationSpeeds[level];
        SetRandomPerfectZone(); // 퍼펙트 존 랜덤 설정
    }

    // 퍼펙트 존을 랜덤한 각도로 설정
    private void SetRandomPerfectZone()
    {
        perfectZoneStartAngle = Random.Range(0f, 360f);
        perfectZoneImages[currentLevel].rectTransform.rotation = Quaternion.Euler(0f, 0f, -perfectZoneStartAngle);
    }

    // 선이 퍼펙트 존 범위 내에 있는지 확인
    private bool IsLineInPerfectZone()
    {
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentLineAngle, perfectZoneStartAngle));
        return angleDiff <= perfectZoneAngles[currentLevel] / 2f; // 현재 레벨의 절반 각도 내
    }

    // 스페이스바 입력 처리
    private void HandleSpacebarInput()
    {
        // 스페이스바 이미지 애니메이션
        if (spacebarImage != null)
        {
            StartCoroutine(MoveSpacebarImage());
        }

        // 회전 방향 반전
        isClockwise = !isClockwise;

        // 퍼펙트 존 성공 여부 확인
        if (IsLineInPerfectZone())
        {
            currentLevel++;
            if (currentLevel >= perfectZoneImages.Length)
            {
                // 게임 성공 처리
                canSpacebar = false;
                resultText.text = "성공!";
                StopColorChange(); // 색상 변화 중지
                StartCoroutine(EndGameAfterDelay()); // 2초 대기 후 종료
                return;
            }
            StartLevel(currentLevel); // 다음 레벨 시작
        }
        else
        {
            // 실패 시 이전 레벨로 이동 (최소 0레벨)
            currentLevel = Mathf.Max(0, currentLevel - 1);
            StartLevel(currentLevel);
        }
    }

    // 스페이스바 이미지 이동 애니메이션
    private IEnumerator MoveSpacebarImage()
    {
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    // 음식 이미지 색상을 검은색으로 변화
    private IEnumerator ChangeCookingImageColor()
    {
        float elapsedTime = 0f;
        Color startColor = cookingImage.color; // 초기 색상
        Color targetColor = Color.black; // 목표 색상

        while (elapsedTime < timerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / timerDuration;
            cookingImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // 시간 초과 시 실패 처리
        canSpacebar = false;
        resultText.text = "실패!";
        yield return new WaitForSeconds(2f);
        //gameObject.SetActive(false); // 캔버스 비활성화
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();
    }

    // 게임 종료 후 1초 대기
    private IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        //gameObject.SetActive(false); // 캔버스 비활성화
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();
        
    }

    // 색상 변화 코루틴 중지
    private void StopColorChange()
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }
    }
}