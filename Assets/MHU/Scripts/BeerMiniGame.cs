using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeerMiniGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image beerFillImage; // Fill�� ����� �̹���
    [SerializeField] private TMP_Text resultText;

    [Header("����")]
    [SerializeField] private float fillSpeed = 0.3f; // �������� �ӵ�
    [SerializeField] private float successMin = 0.45f;
    [SerializeField] private float successMax = 0.55f;

    private bool isFilling = false;
    private bool isGameEnded = false;

    void Start()
    {
        beerFillImage.fillAmount = 0f;
        resultText.text = "";
    }

    void Update()
    {
        if (isGameEnded) return;

        // Space�� ������ ������ fill ����
        if (Input.GetKey(KeyCode.Space))
        {
            isFilling = true;
            beerFillImage.fillAmount += fillSpeed * Time.deltaTime;

            // �����÷ο� üũ
            if (beerFillImage.fillAmount >= 1f)
            {
                beerFillImage.fillAmount = 1f;
                EndGame(false);
            }
        }
        else if (isFilling) // �����̽����� ���� �� ����
        {
            CheckResult();
        }
    }

    void CheckResult()
    {
        float amount = beerFillImage.fillAmount;

        if (amount >= successMin && amount <= successMax)
        {
            EndGame(true);
        }
        else
        {
            EndGame(false);
        }
    }

    void EndGame(bool success)
    {
        isGameEnded = true;
        resultText.text = success ? "����!" : "����!";
        resultText.color = success ? Color.green : Color.red;

        // ���� ó�� (��: ��ư Ŭ�� �� ����� ��)
    }
}
