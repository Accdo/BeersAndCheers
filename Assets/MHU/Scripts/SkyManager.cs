using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkyManager : MonoBehaviour
{
    private static SkyManager instance; // 싱글톤 인스턴스
    [SerializeField] private Light directionalLight; // 방향성 라이트 컴포넌트

    private float morningTemperature = 20000f; // 아침 색온도 (켈빈)
    private float nightTemperature = 1500f; // 밤 색온도 (켈빈)

    public static SkyManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    private void Update()
    {
        if (directionalLight != null)
        {
            //// 게임 시간과 sleepTime을 기반으로 lerp 비율 계산
            //float t = Mathf.Clamp01(TavernManager.instance.gameTime.Timer / TavernManager.instance.gameTime.sleepTime);
            //// 아침 색온도(20000K)에서 밤 색온도(1500K)로 lerp
            //float currentTemperature = Mathf.Lerp(morningTemperature, nightTemperature, t);
            //directionalLight.colorTemperature = currentTemperature;

            if(TavernManager.instance.gameTime.Timer==0)
            {
                SetMorning();
            }
            else if (TavernManager.instance.gameTime.Timer >= TavernManager.instance.gameTime.nightTime)
            {
                SetNight();
            }
        }
    }

    // 특정 시간대 설정 메서드
    public void SetMorning()
    {
        if (directionalLight != null)
        {
            directionalLight.colorTemperature = morningTemperature;
        }
    }

    public void SetAfternoon()
    {
        if (directionalLight != null)
        {
            directionalLight.colorTemperature = 5000f;
        }
    }

    public void SetNight()
    {
        if (directionalLight != null)
        {
            directionalLight.colorTemperature = nightTemperature;
        }
    }
}
