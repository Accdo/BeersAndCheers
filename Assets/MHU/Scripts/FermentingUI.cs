using UnityEngine;
using UnityEngine.UI;

public class FermentingUI : MonoBehaviour
{
    [SerializeField] private Image backBar; // �������� ��� �̹���
    [SerializeField] private Image gaugeBar; // �������� �̹���
    [SerializeField] private float fermentationTime = 30f; // ��ȿ �ð� (��)
    [SerializeField] private Transform playerTransform; // �÷��̾� Transform ����

    private float currentTime = 0f;
    public bool isFermenting { get; private set; } = false;
    public int fermentingBeer { get; set; } = 0;

    private void Start()
    {
        // �÷��̾� Transform�� �������� ���� ��� �±׷� ã��
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player Transform is not assigned and no GameObject with tag 'Player' found!");
            }
        }
    }

    private void Update()
    {
        if (isFermenting)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(1f - (currentTime / fermentationTime));
            gaugeBar.fillAmount = progress;

            if (progress <= 0f)
            {
                isFermenting = false;
                fermentingBeer++;
                gaugeBar.gameObject.SetActive(false);
                backBar.gameObject.SetActive(false);
            }
        }

        // �÷��̾� ������ ���ϵ��� UI ȸ�� (Z���� -90�� ����)
        if (playerTransform != null && gaugeBar.gameObject.activeInHierarchy)
        {
            Vector3 directionToPlayer = playerTransform.position - gaugeBar.transform.position;
            directionToPlayer.y = 0f; // Y�� ȸ���� ���� (X�� ���� ȸ��)
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
                Quaternion finalRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, -180f);
                gaugeBar.transform.rotation = finalRotation;
                backBar.transform.rotation = finalRotation;
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
    }
}