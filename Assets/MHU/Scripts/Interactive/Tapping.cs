using UnityEngine;

public class Tapping : MonoBehaviour, IInteractable
{
    #region ��ȣ�ۿ�
    public string GetCursorType() => "Tapping";
    public string GetInteractionID() => "Tapping";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion

    [SerializeField] public BeerMiniGame beerMiniGame;
    [SerializeField] private int maxExecutions = 2; // �ִ� ���� Ƚ�� (Inspector���� ���� ����)

    private int executionCount = 0; // ���� Ƚ�� ī����

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
        executionCount++; // ���� Ƚ�� ����

        GH_GameManager.instance.player.StartOtherWork();
        beerMiniGame.gameObject.SetActive(true);
        beerMiniGame.BeerMiniGameStart();

        // ���� Ƚ���� maxExecutions�� �����ϸ� ������Ʈ ����
        if (executionCount >= maxExecutions)
        {
            Destroy(gameObject);
        }
    }
}