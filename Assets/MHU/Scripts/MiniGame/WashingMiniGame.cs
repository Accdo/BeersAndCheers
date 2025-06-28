using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WashingMinigame : MonoBehaviour
{
    [SerializeField] private Image spacebarImage; // 스페이스바 이미지
    [SerializeField] private Image plateImage; // 접시 이미지
    [SerializeField] private Image plateImageSuccess; // 성공 접시 이미지
    [SerializeField] private Image waterImage; // 물 이미지

    [SerializeField] private Slider progressBar; // 진행률 바
    [SerializeField] private TextMeshProUGUI resultText; // 결과 텍스트

    private Vector2 spacebarOriginalPos; // 스페이스바 이미지 원래 위치
    private float pressTime; // 현재 스페이스바 누르고 있는 시간
    private float totalPressTime; // 누적 누름 시간
    private bool isPressing; // 스페이스바 누르는 중 여부
    private const float MAX_PRESS_TIME = 2f; // 목표 누름 시간 (10초)
    private float shakeAngle = 10f; // 흔들기 각도
    private float shakeSpeed = 10f; // 흔들기 속도

    //private void Start()
    //{

    //    WashingMiniGameStart(); // 게임 시작
    //}


    void Update()
    {
        if (resultText.text == "성공!") return; // 게임 완료 시 입력 중지

        // 스페이스바 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressing = true;
            pressTime = 0f;
            StartCoroutine(MoveSpacebarImage());
        }
        if (isPressing)
        {
            pressTime += Time.deltaTime;
            totalPressTime += Time.deltaTime; // 누적 시간 증가
            progressBar.value = totalPressTime / MAX_PRESS_TIME; // 진행률 업데이트
            // 좌우 흔들기 (사인파로 회전)
            float angle = Mathf.Sin(Time.time * shakeSpeed) * shakeAngle;
            plateImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
            if (totalPressTime >= MAX_PRESS_TIME)
            {
                GameComplete(); // 미니게임 성공
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isPressing = false;
            plateImage.rectTransform.rotation = Quaternion.identity; // 회전 초기화
        }
    }

    private IEnumerator MoveSpacebarImage()
    {
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    public void WashingMiniGameStart()
    {
        gameObject.SetActive(true);

        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
        resultText.text = "";

        GH_GameManager.instance.player.StartOtherWork();
        GH_GameManager.instance.uiManager.ActiveHotbarUI(false);

        totalPressTime = 0f;
        progressBar.value = 0f;
        resultText.text = "";
        plateImage.rectTransform.rotation = Quaternion.identity;
        plateImageSuccess.gameObject.SetActive(false); // 성공 접시 비활성화
        waterImage.gameObject.SetActive(true); // 물 활성화
    }

    private void GameComplete()
    {
        resultText.text = "성공!";
        plateImage.rectTransform.rotation = Quaternion.identity; // 완료 시 회전 초기화
        plateImageSuccess.gameObject.SetActive(true); // 성공 접시 활성화
        waterImage.gameObject.SetActive(false); // 물 비활성화
        StartCoroutine(WaitEnd()); // 1초 후 비활성화
    }

    private IEnumerator WaitEnd()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        GH_GameManager.instance.player.EndOtherWork();
        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);

    }
}