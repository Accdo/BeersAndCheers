using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FermentingUI : MonoBehaviour
{
    [SerializeField] private Image backBar; // �������� ��� �̹���
    [SerializeField] public Image gaugeBar; // �������� �̹���
    [SerializeField] private float fermentationTime = 30f; // ��ȿ �ð� (��)
    [SerializeField] private GameObject fermentingObject; // ���̾� ������ ���� Fermenting ������Ʈ ����
    [SerializeField] public TextMeshProUGUI successText;

    private float currentTime = 0f;
    public bool isFermenting { get; private set; } = false;
    public int fermentingBeer { get; set; } = 0;
    private Color originalGaugeColor; // ���� �������� ���� ����

    private void Start()
    {
      
        
        // ���� ���� ����
        if (gaugeBar != null)
        {
            originalGaugeColor = gaugeBar.color;
        }

        gaugeBar.gameObject.SetActive(false);
        backBar.gameObject.SetActive(false);
        successText.text = "";
    }

    private void Update()
    {
        if (isFermenting)
        {
            fermentingObject.layer = LayerMask.NameToLayer("Obstacle");
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(1f - (currentTime / fermentationTime));
            gaugeBar.fillAmount = progress;

            if (progress <= 0f)
            {
                isFermenting = false;
                fermentingBeer++;

                backBar.gameObject.SetActive(false);
                fermentingObject.layer = LayerMask.NameToLayer("Interactable");
                //successText.text = "����";

                // ��ȿ �Ϸ� �� �������� ������ �ʷϻ����� ����
                gaugeBar.color = new Color(0f, 1f, 0f);
                gaugeBar.fillAmount = 1;


            }
        }
    }

    public void StartFermentation()
    {
        if (gaugeBar == null || backBar == null)
        {
            Debug.LogError("GaugeBar or BackBar is not assigned in the Inspector!");
            return;
        }

        gaugeBar.gameObject.SetActive(true);
        backBar.gameObject.SetActive(true);
        gaugeBar.fillAmount = 1f;
        currentTime = 0f;
        isFermenting = true;
        // ��ȿ ���� �� ���� �������� ����
        if (gaugeBar != null)
        {
            gaugeBar.color = originalGaugeColor;
        }
    }
}