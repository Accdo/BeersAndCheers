using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

enum GameTimeState
{
    Day, // 낮
    Night, // 밤
    Sleep // 취침
}

public class TavernManager : MonoBehaviour
{
    public static TavernManager instance;

    private void Awake()

    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [Header("게임 시간 UI")]
    public GameTimeUI gameTime;

    [Header("게임 시간 상태")]
    [SerializeField] private GameTimeState gameTimeState = GameTimeState.Day;

    [Header("플레이어 리스폰 위치")]
    [SerializeField] private GameObject RespawnPoint; // 플레이어가 취침 후 리스폰할 위치

    [Header("페이드 인 아웃")]
    [SerializeField] private Image fadeImage; // 전체화면 Image (검정색)
    public float fadeDuration = 1f;


    [Header("만족도 보너스 골드 관련")]
    [SerializeField] private int defaultGold = 1000; // 만족도 기본 골드
    private bool isPluseGold = false;

    [Header("초당 체력 감소")]
    [SerializeField] private float playerHealthDecreaseRate = 1f; // 플레이어가 취침시간에 초당 감소하는 체력

    private void Start()
    {
        // 게임 시작 세팅
        FadeImageSet();
        // 게임 시작 시 페이드 인
        FadeIn();


        EventManager.Instance.AddListener(PlayerEvents.DIED, Sleeping);
    }

    private void FadeImageSet() // 활성화
    {
        fadeImage.gameObject.SetActive(true);
    }
    private void FadeImageClear() // 비활성화
    {
        fadeImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 시간에 따른 낮 밤 수면 상태
        if (gameTime.Timer > gameTime.sleepTime) // 취침 시간
        {
            gameTimeState = GameTimeState.Sleep;

            // 선술집 정산
            if (isPluseGold)
            {
                SoundManager.Instance.Play("MoneySFX");
                int bonusGold = defaultGold * PlateManager.instance.Satisfaction; // 만족도 보너스 골드
                GH_GameManager.instance.goldManager.AddMoney(bonusGold);
                Debug.Log($"만족도 보너스 골드: {bonusGold} G");

                isPluseGold = false;
            }

            // 취침 상태일 때는 플레이어 Hp 감소
            GH_GameManager.instance.player.Damage(playerHealthDecreaseRate * Time.deltaTime); // 초당 1씩 감소
        }
        else if (gameTime.Timer > gameTime.nightTime) // 밤 시간
        {
            gameTimeState = GameTimeState.Night;

            isPluseGold = true;
        }
        else // 낮 시간
        {
            gameTimeState = GameTimeState.Day;
        }
    }

    // 플레이어 피 0일때 호출
    // 플레이어가 침대 상호작용 시 호출
    public void Sleeping(object data = null)
    {
        StartCoroutine(Sleeping());
    }



    // 가게 열 닫을 시간인가? 에 대한 bool 함수
    public bool OpenTavernTime()
    {
        // 게임 시간이 밤이 되면 true
        return gameTimeState == GameTimeState.Night;
    }
    // 위 함수를 조건문으로 걸고 true면, 손님들이 선술집에 들어옴



    // 가게 문 닫을 시간인가? 에 대한 bool 함수
    public bool CloseTavernTime()
    {
        // 게임 시간이 취침 시간이면 true
        return gameTimeState == GameTimeState.Sleep;
    }
    // 위 함수를 조건문으로 걸고 true면, 손님들이 선술집을 나감

    public void FadeIn()
    {
        //fadeImage.DOFade(0f, fadeDuration);
        FadeImageSet(); // 활성화
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            FadeImageClear(); // 완료 후 비활성화
        });
    }

    public void FadeOut()
    {
        //fadeImage.DOFade(1f, fadeDuration);
        FadeImageSet(); // 활성화
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            FadeImageClear(); // 완료 후 비활성화
        });
    }
    IEnumerator Sleeping()
    {
        // 페이드 인 페이드 아웃
        FadeOut();
        GH_GameManager.instance.player.TempHeal();
        GH_GameManager.instance.player.Mujeok(true); // 무적 상태로 변경
        yield return new WaitForSeconds(fadeDuration);

        // 플레이어 위치 이동
        GH_GameManager.instance.player.transform.position = RespawnPoint.transform.position;

        // 피가 100
        GH_GameManager.instance.player.Heal();

        gameTime.NextDay();

        // 시간이 08:00으로 초기화
        gameTime.Timer = 0f; // 시간을 초기화

        GH_GameManager.instance.player.Mujeok(false); // 무적 상태로 변경
        FadeIn();
    }
}
