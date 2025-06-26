using UnityEngine;

public class Tapping : MonoBehaviour, IInteractable
{
    #region 상호작용
    public string GetCursorType() => "Tapping";
    public string GetInteractionID() => "Tapping";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion


    [SerializeField] public BeerMiniGame beerMiniGame;

    private void Start()
    {
        beerMiniGame.gameObject.SetActive(false);
    }
    public void Interact()
    {
        GH_GameManager.instance.player.StartOtherWork();
        beerMiniGame.gameObject.SetActive(true);
        beerMiniGame.BeerMiniGameStart();
    }
}
