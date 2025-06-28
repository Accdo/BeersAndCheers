using UnityEngine;

public class Fermenting : MonoBehaviour, IInteractable
{
    #region ��ȣ�ۿ�
    public string GetCursorType() => "Fermenting";
    public string GetInteractionID() => "Fermenting";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion

    public InteractionUI interactionUI;
    public FermentingUI fermentingUI;
    public CraftManager craftManager;
    public UI_Manager ui_Manager;
    public Interaction interaction;
    public Cooking_UI cookingUI;

    [SerializeField] public int fermentingID; // �� ��ȿ���� ���� ID
    public void Interact()
    {
        // Cooking_UI�� ���� ��ȿ���� ID ����
        cookingUI.SetCurrentFermentingUI(fermentingID);

        // ��ȿ���� �ƴϰ�
        if (!fermentingUI.isFermenting)
        {
            interaction.isBusy = true;
            interactionUI.ResetUI();

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
                // �ؽ�Ʈ ���ֱ�
                fermentingUI.successText.text = "";
            }

        }
        else
        {


        }
    }
}
