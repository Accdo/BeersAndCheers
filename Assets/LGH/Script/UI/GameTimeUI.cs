using UnityEngine;
using TMPro;

// 게임 시간 UI를 관리하는 스크립트
// 게임 시간은 08:00 ~ 24:00 사이로 설정
// 하루는 10분으로 설정 (실제 시간의 10분이 게임 내 하루)
public class GameTimeUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float sleepTime = 600f; // 낮밤 합쳐서 10분
    public float nightTime = 300f;
    public float Timer = 0f;

    void Update()
    {
        Timer += Time.deltaTime;

        // 반복 (하루가 끝나면 다시 시작)
        if (Timer > sleepTime)
        {
            // 취침시간
            //elapsedTime = 0f; <= 기상할 때 시간
        }

        // 실제 시간 -> 게임 시간 (08:00 ~ 24:00)
        float normalizedTime = Timer / sleepTime; // 0 ~ 1
        float gameHourFloat = 8f + normalizedTime * 16f; // 8시 ~ 24시
        if (gameHourFloat >= 24f) gameHourFloat -= 24f;

        int hour = Mathf.FloorToInt(gameHourFloat);
        int minute = Mathf.FloorToInt((gameHourFloat - hour) * 60f);

        // UI 표시
        timeText.text = $"{hour:D2}:{minute:D2}";
    }

    


}