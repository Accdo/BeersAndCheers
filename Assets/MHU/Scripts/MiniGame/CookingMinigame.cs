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
    [SerializeField] public Image cookingImage; // �丮 ���� ���� �̹���
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
    public Interaction interaction;
    public Cooking cooking;

    public bool isCookingSuccess = false;
    
    public void ImageSet(Sprite sprite)
    {
        cookingImage.sprite = sprite;
    }
    // ���� ���� �� �ʱ�ȭ
    public void StartCookingMinigame()
    {
        // ���� �ڷ�ƾ ����
        StopColorChange();

        
        // ���� �� UI �ʱ�ȭ
        InitSet();
        InitializeUI();
        StartLevel(currentLevel);
        colorChangeCoroutine = StartCoroutine(ChangeCookingImageColor());
    }

    // �� ������ ������Ʈ
    private void Update()
    {
        if (!canSpacebar) return;

        float rotationDelta = currentRotationSpeed * Time.deltaTime * (isClockwise ? -1f : 1f);
        currentLineAngle = (currentLineAngle + rotationDelta) % 360f;
        if (currentLineAngle < 0f) currentLineAngle += 360f;
        lineImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, -currentLineAngle);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpacebarInput();
        }
    }

    // UI ��� �ʱ�ȭ
    private void InitializeUI()
    {
        if (cookingImage == null || perfectZoneImages == null || perfectZoneImages.Length != 3 ||
            lineImage == null || resultText == null || spacebarImage == null)
        {
            Debug.LogError("��� UI ��Ҹ� �ν����Ϳ� �Ҵ��ؾ� �մϴ�.");
            enabled = false;
            return;
        }

        // �����̽��� ��ġ �ʱ�ȭ
        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;

        // ��� ����Ʈ �� �̹��� ��Ȱ��ȭ
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        // UI ��� �ʱ�ȭ
        cookingImage.color = Color.white; // ���� �̹��� ���� �ʱ�ȭ
        resultText.text = "";
        lineImage.rectTransform.rotation = Quaternion.identity; // �� ȸ�� �ʱ�ȭ
    }

    private void InitSet()
    {
        currentLevel = 0;
        currentLineAngle = 0f;
        isClockwise = true;
        canSpacebar = true;
    }

    // ������ ���� ����
    private void StartLevel(int level)
    {
        foreach (Image zoneImage in perfectZoneImages)
        {
            zoneImage.gameObject.SetActive(false);
        }

        perfectZoneImages[level].gameObject.SetActive(true);
        currentRotationSpeed = rotationSpeeds[level];
        SetRandomPerfectZone();
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
        return angleDiff <= perfectZoneAngles[currentLevel] / 2f;
    }

    // �����̽��� �Է� ó��
    private void HandleSpacebarInput()
    {
        if (spacebarImage != null)
        {
            StartCoroutine(MoveSpacebarImage());
        }

        isClockwise = !isClockwise;

        if (IsLineInPerfectZone())
        {
            currentLevel++;
            if (currentLevel >= perfectZoneImages.Length)
            {
                //���� ó��
                canSpacebar = false;
                resultText.text = "����!";
                StopColorChange();
                StartCoroutine(EndGameAfterDelay());
                return;
            }
            StartLevel(currentLevel);
        }
        else
        {
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

    // ���� �̹��� ���� ��ȭ
    private IEnumerator ChangeCookingImageColor()
    {
        float elapsedTime = 0f;
        Color startColor = Color.white; // �׻� ������� ����
        Color targetColor = Color.black;

        while (elapsedTime < timerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / timerDuration;
            cookingImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // ���� ó�� (�ð��ʰ�)
        canSpacebar = false;
        resultText.text = "����!";
        yield return new WaitForSeconds(1f);
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();
        interaction.ResetInteractionState();
        cooking.isCooking = false; // Cooking Ŭ������ ������Ƽ ���

        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);
    }

    // ���� ó�� �ڷ�ƾ (���� ���� �� 1�� ���)
    private IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        interactionUI.HideCookingMiniGameUI();
        interactionUI.ResetUI();
        interaction.ResetInteractionState();
        cooking.isCooking = false;
        isCookingSuccess = true;

        GH_GameManager.instance.uiManager.ActiveHotbarUI(true);
    }

    // ���� ��ȭ �ڷ�ƾ ����
    private void StopColorChange()
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }
        cookingImage.color = Color.white; // ���� �ʱ�ȭ
    }
}