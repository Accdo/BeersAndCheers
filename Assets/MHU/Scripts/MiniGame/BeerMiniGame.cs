using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BeerMiniGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image beerFillImage; // ���� ä��� �̹���
    [SerializeField] private TMP_Text resultText; // ��� �ؽ�Ʈ
    [SerializeField] private Image successImage; // ���� �� ǥ�� �̹���
    [SerializeField] private Image spacebarImage; // �����̽��� �̹���

    [Header("����")]
    [SerializeField] private float fillSpeed = 0.3f; // ��ü �������� �ӵ�
    [SerializeField] private float successMin = 0.9f; // ���� �ּҰ�

    public Item selectBeer;

    private bool isFilling = false;
    private bool isGameEnded = false;
    private Vector2 spacebarOriginalPos; // �����̽��� �̹��� ���� ��ġ

    public void BeerMiniGameStart()
    {
        Initialize();
    }

    private void Initialize()
    {
        GH_GameManager.instance.uiManager.ActiveHotbarUI(false);
        // �ʱ�ȭ
        if (beerFillImage == null || resultText == null || spacebarImage == null || successImage == null)
        {
            Debug.LogError("UI ��Ұ� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        isGameEnded = false;
        beerFillImage.fillAmount = 0.1f;
        resultText.text = "";
        successImage.gameObject.SetActive(false);
        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (isGameEnded) return;

        // �����̽��� �Է� ó��
        if (Input.GetKey(KeyCode.Space))
        {
            HandlePouring();
        }
        else if (isFilling) // �����̽��ٿ��� ���� �� ����
        {
            isFilling = false;
            CheckResult();
        }
    }

    private void HandlePouring()
    {
        // �����̽��� �ִϸ��̼�
        StartCoroutine(MoveSpacebarImage());

        spacebarImage.color = new Color(1f, 1f, 1f, 1f); // ���İ� ����
        isFilling = true;

        // ��ü ä���
        beerFillImage.fillAmount += fillSpeed * Time.deltaTime;

        // �ִ밪 �ʰ� �� 1�� ����
        if (beerFillImage.fillAmount > 1f)
        {
            beerFillImage.fillAmount = 1f;
        }

        // ���� ���� Ȯ��
        CheckResult();
    }

    private void CheckResult()
    {
        float amount = beerFillImage.fillAmount;

        // ���� ����: successMin �̻�
        if (amount >= successMin)
        {
            HandleSuccessCoroutine();
        }
    }

    private void HandleSuccessCoroutine()
    {
        // ���� ó��
        successImage.gameObject.SetActive(true);
        resultText.text = "����!";
        resultText.color = Color.green;

        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
       

        isGameEnded = true;
        GH_GameManager.instance.player.EndOtherWork();

        // �߰� ó�� (��: ����� ��ư Ȱ��ȭ ��)
        if (selectBeer.data is FoodData foodData)
        {
            GH_GameManager.instance.player.inventory.Add("Backpack", selectBeer);
            // �κ� ���ΰ�ħ
            GH_GameManager.instance.uiManager.RefreshAll();
        }

        gameObject.SetActive(false);
        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);

    }

    private IEnumerator MoveSpacebarImage()
    {
        // �����̽��� �̹��� 3�ȼ� �Ʒ��� �̵�
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        // ���� ��ġ�� ����
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }
}