using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WashingMinigame : MonoBehaviour
{
    [SerializeField] private Image spacebarImage; // �����̽��� �̹���
    [SerializeField] private Image plateImage; // ���� �̹���
    [SerializeField] private Image plateImageSuccess; // ���� ���� �̹���
    [SerializeField] private Image waterImage; // �� �̹���

    [SerializeField] private Slider progressBar; // ����� ��
    [SerializeField] private TextMeshProUGUI resultText; // ��� �ؽ�Ʈ

    private Vector2 spacebarOriginalPos; // �����̽��� �̹��� ���� ��ġ
    private float pressTime; // ���� �����̽��� ������ �ִ� �ð�
    private float totalPressTime; // ���� ���� �ð�
    private bool isPressing; // �����̽��� ������ �� ����
    private const float MAX_PRESS_TIME = 2f; // ��ǥ ���� �ð� (10��)
    private float shakeAngle = 10f; // ���� ����
    private float shakeSpeed = 10f; // ���� �ӵ�

    //private void Start()
    //{

    //    WashingMiniGameStart(); // ���� ����
    //}


    void Update()
    {
        if (resultText.text == "����!") return; // ���� �Ϸ� �� �Է� ����

        // �����̽��� �Է� ó��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressing = true;
            pressTime = 0f;
            StartCoroutine(MoveSpacebarImage());
        }
        if (isPressing)
        {
            pressTime += Time.deltaTime;
            totalPressTime += Time.deltaTime; // ���� �ð� ����
            progressBar.value = totalPressTime / MAX_PRESS_TIME; // ����� ������Ʈ
            // �¿� ���� (�����ķ� ȸ��)
            float angle = Mathf.Sin(Time.time * shakeSpeed) * shakeAngle;
            plateImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
            if (totalPressTime >= MAX_PRESS_TIME)
            {
                GameComplete(); // �̴ϰ��� ����
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isPressing = false;
            plateImage.rectTransform.rotation = Quaternion.identity; // ȸ�� �ʱ�ȭ
        }
    }

    private IEnumerator MoveSpacebarImage()
    {
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    public void WashingMiniGameStart()
    {
        gameObject.SetActive(true);

        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
        resultText.text = "";

        GH_GameManager.instance.player.StartOtherWork();
        GH_GameManager.instance.uiManager.ActiveHotbarUI(false);

        totalPressTime = 0f;
        progressBar.value = 0f;
        resultText.text = "";
        plateImage.rectTransform.rotation = Quaternion.identity;
        plateImageSuccess.gameObject.SetActive(false); // ���� ���� ��Ȱ��ȭ
        waterImage.gameObject.SetActive(true); // �� Ȱ��ȭ
    }

    private void GameComplete()
    {
        resultText.text = "����!";
        plateImage.rectTransform.rotation = Quaternion.identity; // �Ϸ� �� ȸ�� �ʱ�ȭ
        plateImageSuccess.gameObject.SetActive(true); // ���� ���� Ȱ��ȭ
        waterImage.gameObject.SetActive(false); // �� ��Ȱ��ȭ
        StartCoroutine(WaitEnd()); // 1�� �� ��Ȱ��ȭ
    }

    private IEnumerator WaitEnd()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        GH_GameManager.instance.player.EndOtherWork();
        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);

    }
}