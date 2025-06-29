using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishingMiniGame : MonoBehaviour
{
    public static FishingMiniGame Instance;

    [Header("Game Settings")]
    public int fishGoal = 3;
    public float totalTimeLimit = 5f;          // 전체 제한 시간 (초)
    public float minSpawnDelay = 0.4f;          // 물고기 등장 간 최소 대기시간
    public float maxSpawnDelay = 1.2f;          // 물고기 등장 간 최대 대기시간
    public float minVisibleTime = 1.0f;         // 물고기 등장 시간 최소
    public float maxVisibleTime = 1.8f;         // 물고기 등장 시간 최대

    [Header("References")]
    public GameObject miniGamePanel;            // 미니게임 UI 패널
    public Button[] fishSlotButtons;            // 물고기 슬롯 버튼 배열
    public Image[] catchCountIcons;             // 잡은 카운트 표시 아이콘 배열
    public Sprite emptyIcon;                     // 빈 아이콘 스프라이트
    public Sprite filledIcon;                    // 채워진 아이콘 스프라이트

    [Header("Time UI")]
    public Image timeBarFill;                    // 시간바 Image (Fill Amount 사용)

    private int catchCount = 0;                  // 잡은 물고기 수
    private Action<bool> onComplete;             // 게임 종료 콜백

    private void Awake()
    {
        Instance = this;
        miniGamePanel.SetActive(false);
    }

    public void StartGame(Action<bool> callback)
    {
        // 플레이어 이동 제한 시작
        Player_LYJ player = GameObject.FindWithTag("Player")?.GetComponent<Player_LYJ>();
        player?.MouseVisible(true);
        player?.StartOtherWork();

        catchCount = 0;
        onComplete = callback;
        UpdateCatchIcons();
        miniGamePanel.SetActive(true);

        // 시간바 초기화
        if (timeBarFill != null)
            timeBarFill.fillAmount = 1f;

        StartCoroutine(GameLoop());
    }

    private void UpdateCatchIcons()
    {
        for (int i = 0; i < catchCountIcons.Length; i++)
        {
            catchCountIcons[i].sprite = (i < catchCount) ? filledIcon : emptyIcon;
        }
    }

    IEnumerator GameLoop()
    {
        float elapsedTime = 0f;

        while (catchCount < fishGoal && elapsedTime < totalTimeLimit)
        {
            // 시간바 업데이트
            if (timeBarFill != null)
                timeBarFill.fillAmount = 1f - (elapsedTime / totalTimeLimit);

            // 랜덤 물고기 버튼 선택
            int index = UnityEngine.Random.Range(0, fishSlotButtons.Length);
            Button btn = fishSlotButtons[index];

            // 물고기 이미지 & 애니메이터 찾기
            Transform fishImage = btn.transform.Find("FishImage");
            if (fishImage == null)
            {
                Debug.LogError($"{btn.name} 안에 FishImage라는 이름의 자식이 없습니다!");
                yield break;
            }

            Animator anim = fishImage.GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError($"{fishImage.name}에 Animator가 없습니다!");
                yield break;
            }
            bool caught = false;
            bool isFishVisible = true;
            // 클릭 리스너 초기화 및 등록
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (!caught&& isFishVisible)
                {
                    caught = true;
                    catchCount++;
                    UpdateCatchIcons();
                }
            });

            // 물고기 나타내고 애니메이션 실행
            fishImage.gameObject.SetActive(true);
            anim.ResetTrigger("Pop");
            anim.SetTrigger("Pop");
            SoundManager.Instance.Play("FishingSFX");

            float visibleTime = UnityEngine.Random.Range(minVisibleTime, maxVisibleTime);
            float timer = 0f;

            // 물고기 보여지는 동안 루프
            while (timer < visibleTime && elapsedTime < totalTimeLimit)
            {
                float delta = Time.deltaTime;
                timer += delta;
                elapsedTime += delta;

                if (timeBarFill != null)
                    timeBarFill.fillAmount = 1f - (elapsedTime / totalTimeLimit);

                if (caught) break;

                yield return null;
            }

            fishImage.gameObject.SetActive(false);
            isFishVisible = false;

            if (elapsedTime >= totalTimeLimit)
                break;

            // 물고기 간 대기시간 (프레임 단위 업데이트)
            float delay = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            float delayTimer = 0f;

            while (delayTimer < delay && elapsedTime < totalTimeLimit)
            {
                float delta = Time.deltaTime;
                delayTimer += delta;
                elapsedTime += delta;

                if (timeBarFill != null)
                    timeBarFill.fillAmount = 1f - (elapsedTime / totalTimeLimit);

                yield return null;
            }
        }

        miniGamePanel.SetActive(false);

        // 플레이어 이동 제한 해제
        Player_LYJ player = GameObject.FindWithTag("Player")?.GetComponent<Player_LYJ>();
        player?.MouseVisible(false);
        player?.EndOtherWork();

        // 성공 여부 콜백 호출
        bool success = (catchCount >= fishGoal);
        onComplete?.Invoke(success);
    }
}
