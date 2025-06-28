using UnityEngine;

public class Tapping : MonoBehaviour, IInteractable
{
    #region 상호작용
    public string GetCursorType() => "Tapping";
    public string GetInteractionID() => "Tapping";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion

    [SerializeField] public BeerMiniGame beerMiniGame;
    [SerializeField] private int maxExecutions = 2; // 최대 실행 횟수 (Inspector에서 설정 가능)

    private int executionCount = 0; // 실행 횟수 카운터

    private void Start()
    {
        if (beerMiniGame == null)
        {
            Debug.LogError("BeerMiniGame is not assigned in the Inspector!");
            return;
        }
        beerMiniGame.gameObject.SetActive(false);
    }

    public void Interact()
    {
        executionCount++; // 실행 횟수 증가

        GH_GameManager.instance.player.StartOtherWork();
        beerMiniGame.gameObject.SetActive(true);
        beerMiniGame.BeerMiniGameStart();

        // 실행 횟수가 maxExecutions에 도달하면 오브젝트 삭제
        if (executionCount >= maxExecutions)
        {
            Destroy(gameObject);
        }
    }
}