using UnityEngine;

public class Fermenting : MonoBehaviour, IInteractable
{
    #region ��ȣ�ۿ�
    public string GetCursorType() => "Fermenting";
    public string GetInteractionID() => "Fermenting";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion


    public InteractionUI interactionUI;
    public GameObject BeerRecipeUI;
    public FermentingUI fermentingUI;
    public CraftManager craftManager;
    public UI_Manager ui_Manager;

    private void Start()
    {
        BeerRecipeUI.SetActive(false);
    }

    public void Interact()
    {
        // ��ȿ���� �ƴϰ�
        if (!fermentingUI.isFermenting)
        {
            // ��ȿ�Ȱ� 0���̸�
            if (fermentingUI.fermentingBeer == 0)
            {
                // ���� ���� ������ ���̰�
                ui_Manager.ToggleBeerUI();

            }
            // ��ȿ�Ȱ� 1�� �̻��̸�
            else
            {
                // ������ ũ������ ��� ����
                craftManager.StartPlacement();
                // ��ȿ ���� �ʱ�ȭ
                fermentingUI.fermentingBeer = 0;
            }

        }
        else
        {


        }
    }
}
