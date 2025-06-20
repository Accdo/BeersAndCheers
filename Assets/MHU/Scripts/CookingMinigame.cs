using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CookingMinigame : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField] private float timerDuration = 15f; // Ÿ�̸� ���� �ð� (�� ����)
    [SerializeField] private float[] rotationSpeeds = { 300f, 400f, 500f }; // ������ ȸ�� �ӵ�

    [Header("UI ���")]
    [SerializeField] private Image spacebarImage; // �����̽��� �̹���
    [SerializeField] private Image cookingImage; // �丮 ���� ���� �̹���
    [SerializeField] private Image[] perfectZoneImages; // �� ������ ����Ʈ �� �̹���
    [SerializeField] private Image lineImage; // ȸ���ϴ� �� �̹���
    [SerializeField] private TextMeshProUGUI resultText; // ��� ǥ�� �ؽ�Ʈ

    private readonly float[] perfectZoneAngles = { 88f, 44f, 22f }; // ������ ���� ����
    private Vector2 spacebarOriginalPos; // �����̽��� �̹����� ���� ��ġ
    private int currentLevel = 0; // ���� ���� (0 = 1����, 1 = 2����, 2 = 3����)
    private float currentLineAngle; // �� �̹����� ���� ȸ�� ����
    private float perfectZoneStartAngle; // ����Ʈ ���� ���� ����
    private float currentRotationSpeed; // ���� ȸ�� �ӵ�
    private bool isClockwise = true; // ȸ�� ���� (true: �ð� ����, false: �ݽð� ����)
    private Coroutine colorChangeCoroutine; // ���� ��ȭ �ڷ�ƾ
    private bool canSpacebar = true; // �����̽��� �Է� ���� ����

    public InteractionUI interactionUI;

    // ���� ���� �� �ʱ�ȭ
    public void StartCookingMinigame()
    {
        InitSet();
        InitializeUI(); // UI �ʱ�ȭ
        StartLevel(currentLevel); // ù ���� ����
        colorChangeCoroutine = StartCoroutine(ChangeCookingImageColor()); // ���� ��ȭ �ڷ�ƾ ����
    }

    // �� ������ ������Ʈ
    private void Update()
    {
        if (!canSpacebar) return; // �Է� ��Ȱ��ȭ �� ������Ʈ ����

        // �� �̹��� ȸ��
        float rotationDelta = currentRotationSpeed * Time.deltaTime * (isClockwise ? -1f : 1f);
        currentLineAngle = (currentLineAngle + rotationDelta) % 360f;
        if (currentLineAngle < 0f) currentLineAngle += 360f; // 0~360�� ����
        lineImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, -currentLineAngle);

        // �����̽��� �Է� ó��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpacebarInput(); // �����̽��� �Է� ó��
        }
    }

    // UI ��� �ʱ�ȭ
    private void InitializeUI()
    {
        // UI ��� ����
        if (cookingImage == null || perfectZoneImages == null || perfectZoneImages.Length != 3 ||
            lineImage == null || resultText == null)
        {
            Debug.LogError("��� UI ��Ҹ� �ν����Ϳ� �Ҵ��ؾ� �մϴ�.");
            enabled = false;
            return;
        }

        // �����̽��� ���� ��ġ ����
        if (spacebarImage != null)
        {
            spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
        }

        // ��� ����Ʈ �� �̹��� ��Ȱ��ȭ
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        resultText.text = ""; // ��� �ؽ�Ʈ �ʱ�ȭ
    }

    private void InitSet()
    {
        currentLevel = 0;
        cookingImage.color = Color.white;
    }

    // ������ ���� ����
    private void StartLevel(int level)
    {
        // ��� ����Ʈ �� �̹��� ��Ȱ��ȭ
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        // ���� ������ ����Ʈ �� Ȱ��ȭ
        perfectZoneImages[level].gameObject.SetActive(true);
        currentRotationSpeed = rotationSpeeds[level];
        SetRandomPerfectZone(); // ����Ʈ �� ���� ����
    }

    // ����Ʈ ���� ������ ������ ����
    private void SetRandomPerfectZone()
    {
        perfectZoneStartAngle = Random.Range(0f, 360f);
        perfectZoneImages[currentLevel].rectTransform.rotation = Quaternion.Euler(0f, 0f, -perfectZoneStartAngle);
    }

    // ���� ����Ʈ �� ���� ���� �ִ��� Ȯ��
    private bool IsLineInPerfectZone()
    {
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentLineAngle, perfectZoneStartAngle));
        return angleDiff <= perfectZoneAngles[currentLevel] / 2f; // ���� ������ ���� ���� ��
    }

    // �����̽��� �Է� ó��
    private void HandleSpacebarInput()
    {
        // �����̽��� �̹��� �ִϸ��̼�
        if (spacebarImage != null)
        {
            StartCoroutine(MoveSpacebarImage());
        }

        // ȸ�� ���� ����
        isClockwise = !isClockwise;

        // ����Ʈ �� ���� ���� Ȯ��
        if (IsLineInPerfectZone())
        {
            currentLevel++;
            if (currentLevel >= perfectZoneImages.Length)
            {
                // ���� ���� ó��
                canSpacebar = false;
                resultText.text = "����!";
                StopColorChange(); // ���� ��ȭ ����
                StartCoroutine(EndGameAfterDelay()); // 2�� ��� �� ����
                return;
            }
            StartLevel(currentLevel); // ���� ���� ����
        }
        else
        {
            // ���� �� ���� ������ �̵� (�ּ� 0����)
            currentLevel = Mathf.Max(0, currentLevel - 1);
            StartLevel(currentLevel);
        }
    }

    // �����̽��� �̹��� �̵� �ִϸ��̼�
    private IEnumerator MoveSpacebarImage()
    {
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f);
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    // ���� �̹��� ������ ���������� ��ȭ
    private IEnumerator ChangeCookingImageColor()
    {
        float elapsedTime = 0f;
        Color startColor = cookingImage.color; // �ʱ� ����
        Color targetColor = Color.black; // ��ǥ ����

        while (elapsedTime < timerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / timerDuration;
            cookingImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // �ð� �ʰ� �� ���� ó��
        canSpacebar = false;
        resultText.text = "����!";
        yield return new WaitForSeconds(2f);
        //gameObject.SetActive(false); // ĵ���� ��Ȱ��ȭ
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();
    }

    // ���� ���� �� 1�� ���
    private IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        //gameObject.SetActive(false); // ĵ���� ��Ȱ��ȭ
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();
        
    }

    // ���� ��ȭ �ڷ�ƾ ����
    private void StopColorChange()
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }
    }
}