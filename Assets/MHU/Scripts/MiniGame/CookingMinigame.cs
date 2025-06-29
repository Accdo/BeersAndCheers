using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CookingMinigame : MonoBehaviour
{
    [Header("게임 설정")]
    [SerializeField] private float timerDuration = 15f; // 미니게임 타이머 지속 시간 (초 단위)
    [SerializeField] private float[] rotationSpeeds = { 300f, 400f, 500f }; // 각 레벨별 회전 속도

    [Header("UI 요소")]
    [SerializeField] private Image spacebarImage; // 스페이스바 입력 표시 이미지
    [SerializeField] public Image cookingImage; // 요리 진행 상태 표시 이미지
    [SerializeField] private Image[] perfectZoneImages; // 각 레벨의 퍼펙트 존 표시 이미지
    [SerializeField] private Image lineImage; // 회전하는 선 이미지
    [SerializeField] private TextMeshProUGUI resultText; // 게임 결과 표시 텍스트

    private readonly float[] perfectZoneAngles = { 88f, 44f, 22f }; // 각 레벨별 퍼펙트 존 각도 범위
    private Vector2 spacebarOriginalPos; // 스페이스바 이미지의 원래 위치
    private int currentLevel = 0; // 현재 게임 레벨 (0 = 1레벨, 1 = 2레벨, 2 = 3레벨)
    private float currentLineAngle; // 선 이미지의 현재 회전 각도
    private float perfectZoneStartAngle; // 퍼펙트 존의 시작 각도
    private float currentRotationSpeed; // 현재 회전 속도
    private bool isClockwise = true; // 회전 방향 (true: 시계 방향, false: 반시계 방향)
    private Coroutine colorChangeCoroutine; // 요리 이미지 색상 변경 코루틴
    private bool canSpacebar = true; // 스페이스바 입력 가능 여부

    public InteractionUI interactionUI; // 상호작용 UI 관리 클래스
    public Interaction interaction; // 상호작용 상태 관리 클래스

    public Cooking_UI cooking_ui; // 요리 UI 관리 클래스

    public bool isCookingSuccess = false; // 요리 성공 여부 플래그

    public Item selectItem;

    public void SetItem(Item item)
    {
        selectItem = item;
    }

    // 요리 이미지 스프라이트 설정 함수
    public void ImageSet(Sprite sprite)
    {
        cookingImage.sprite = sprite;
    }

    // 요리 미니게임 시작 함수
    public void StartCookingMinigame()
    {
        SoundManager.Instance.Play("BoilingSFX");
        // 기존 색상 변경 코루틴 중지
        StopColorChange();

        // 게임 초기화 및 UI 설정
        InitSet();
        InitializeUI();
        StartLevel(currentLevel);
        colorChangeCoroutine = StartCoroutine(ChangeCookingImageColor());
    }

    // 매 프레임 호출되는 업데이트 함수
    private void Update()
    {
        if (!canSpacebar) return;

        // 선 회전 처리
        float rotationDelta = currentRotationSpeed * Time.deltaTime * (isClockwise ? -1f : 1f);
        currentLineAngle = (currentLineAngle + rotationDelta) % 360f;
        if (currentLineAngle < 0f) currentLineAngle += 360f;
        lineImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, -currentLineAngle);

        // 스페이스바 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpacebarInput();
        }
    }

    // UI 요소 초기화 함수
    private void InitializeUI()
    {
        // UI 요소가 제대로 할당되었는지 확인
        if (cookingImage == null || perfectZoneImages == null || perfectZoneImages.Length != 3 ||
            lineImage == null || resultText == null || spacebarImage == null)
        {
            Debug.LogError("필수 UI 요소가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        // 스페이스바 이미지 원래 위치 저장
        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;

        // 모든 퍼펙트 존 이미지 비활성화
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        // UI 초기 상태 설정
        cookingImage.color = Color.white; // 요리 이미지 색상 초기화
        resultText.text = ""; // 결과 텍스트 초기화
        lineImage.rectTransform.rotation = Quaternion.identity; // 선 회전 초기화
    }

    // 게임 변수 초기화 함수
    private void InitSet()
    {
        currentLevel = 0; // 레벨 초기화
        currentLineAngle = 0f; // 선 각도 초기화
        isClockwise = true; // 회전 방향 초기화
        canSpacebar = true; // 스페이스바 입력 가능 상태로 설정
        isCookingSuccess = false; // 요리 성공 플래그 초기화
    }

    // 특정 레벨 시작 함수
    private void StartLevel(int level)
    {
        // 모든 퍼펙트 존 이미지 비활성화
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        // 현재 레벨의 퍼펙트 존 활성화 및 설정
        perfectZoneImages[level].gameObject.SetActive(true);
        currentRotationSpeed = rotationSpeeds[level];
        SetRandomPerfectZone();
    }

    // 퍼펙트 존 시작 각도를 무작위로 설정하는 함수
    private void SetRandomPerfectZone()
    {
        perfectZoneStartAngle = Random.Range(0f, 360f);
        perfectZoneImages[currentLevel].rectTransform.rotation = Quaternion.Euler(0f, 0f, -perfectZoneStartAngle);
    }

    // 선이 퍼펙트 존 안에 있는지 확인하는 함수
    private bool IsLineInPerfectZone()
    {
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentLineAngle, perfectZoneStartAngle));
        return angleDiff <= perfectZoneAngles[currentLevel] * 1f;
    }

    // 스페이스바 입력 처리 함수
    private void HandleSpacebarInput()
    {
        // 스페이스바 이미지 애니메이션 시작
        if (spacebarImage != null)
        {
            StartCoroutine(MoveSpacebarImage());
        }

        // 회전 방향 반전
        isClockwise = !isClockwise;

        // 퍼펙트 존 안에 있는 경우
        if (IsLineInPerfectZone())
        {
            currentLevel++;
            if (currentLevel >= perfectZoneImages.Length)
            {
                // 게임 성공 처리
                canSpacebar = false;
                resultText.text = "성공";
                StopColorChange();
                StartCoroutine(EndGameAfterDelay());
                return;
            }
            StartLevel(currentLevel);
        }
        // 퍼펙트 존 밖에 있는 경우
        else
        {
            currentLevel = Mathf.Max(0, currentLevel - 1);
            StartLevel(currentLevel);
        }
    }

    // 스페이스바 이미지 이동 애니메이션 코루틴
    private IEnumerator MoveSpacebarImage()
    {
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    // 요리 이미지 색상 점진적 변경 코루틴
    private IEnumerator ChangeCookingImageColor()
    {
        float elapsedTime = 0f;
        Color startColor = Color.white; // 초기 색상 (흰색)
        Color targetColor = Color.black; // 목표 색상 (검은색)

        while (elapsedTime < timerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / timerDuration;
            cookingImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // 게임 실패 처리 (시간 초과)
        canSpacebar = false;
        resultText.text = "실패";
        yield return new WaitForSeconds(1f);
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();

        isCookingSuccess = false;

        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);
        GH_GameManager.instance.player.EndOtherWork();
        GH_GameManager.instance.player.MouseVisible(false);
        interaction.isBusy = false;

        SoundManager.Instance.Stop("BoilingSFX");
    }

    // 게임 성공 후 종료 처리 코루틴
    private IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();

        isCookingSuccess = true; // 요리 성공 플래그 설정
        //cooking_ui.Cook(); // 요리 완료 처리
        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);
        // 인벤토리 완성된 음식 추가
        GH_GameManager.instance.player.inventory.Add("Backpack", selectItem);
        // 인벤 새로고침
        GH_GameManager.instance.uiManager.RefreshAll();

        GH_GameManager.instance.player.EndOtherWork();
        GH_GameManager.instance.player.MouseVisible(false);
        interaction.isBusy = false;

        SoundManager.Instance.Stop("BoilingSFX");

    }

    // 요리 이미지 색상 변경 코루틴 중지 함수
    private void StopColorChange()
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }
        cookingImage.color = Color.white; // 요리 이미지 색상 초기화
    }
}