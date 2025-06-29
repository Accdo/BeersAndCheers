using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CuttingMinigame : MonoBehaviour
{
    public Image cuttingImage; // �ڸ� ��� �̹���
    public Image[] cutImages; // 8���� Cut Image �迭
    public Image spacebarImage; // �����̽��� �̹���
    private int currentCutIndex = 0; // ���� Ȱ��ȭ�� Cut Image �ε���
    private float lastInputTime; // ������ �����̽��� �Է� �ð�
    private bool isFading = false; // ���̵� ȿ�� Ȱ��ȭ ����
    private Vector2 spacebarOriginalPos; // �����̽��� �̹��� ���� ��ġ
    [SerializeField] private TextMeshProUGUI resultText; // ��� ǥ�� �ؽ�Ʈ

    public InteractionUI interactionUI;
    public CookingMinigame cookingMinigame;

    public void ImageSet(Sprite sprite,Sprite sprite2)
    {
        cuttingImage.sprite = sprite;
        cookingMinigame.cookingImage.sprite = sprite2;
    }

    public void StartCuttingMinigame()
    {
        InitSet();
        GH_GameManager.instance.player.StartOtherWork();
    }

    void Update()
    {
        // �����̽��� �Է� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastInputTime = Time.time; // �Է� �ð� ����
            isFading = false; // ���̵� ȿ�� ��Ȱ��ȭ
            spacebarImage.color = new Color(1f, 1f, 1f, 1f); // ���İ� 1�� ����

            // Cut Image Ȱ��ȭ
            if (currentCutIndex < cutImages.Length)
            {
                SoundManager.Instance.Play("CuttingSFX");
                cutImages[currentCutIndex].gameObject.SetActive(true);
                currentCutIndex++;
                
                if(currentCutIndex == cutImages.Length)
                {
                    resultText.text = "����!";
                    StartCoroutine(EndGameAfterDelay()); // 2�� ��� �� ����
                }
              
            }

            // �����̽��� �̹��� �̵� �ִϸ��̼�
            StartCoroutine(MoveSpacebarImage());
        }

        // 1�� �̻� �Է� ������ ���̵� ȿ�� ����
        if (Time.time - lastInputTime > 1f && !isFading)
        {
            isFading = true;
        }

        // ���̵� ȿ�� ����
        if (isFading)
        {
            float alpha = Mathf.PingPong(Time.time, 1f); // 1�� �ֱ�� ���İ� 0~1 �ݺ�
            spacebarImage.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    public void InitSet()
    {
        
        resultText.text = "";
        currentCutIndex = 0;

        // ��� Cut Image�� ��Ȱ��ȭ�� �ʱ�ȭ
        if (cutImages == null || cutImages.Length != 8)
        {
            Debug.LogError("Cut Images �迭�� �ùٸ��� �������� �ʾҽ��ϴ�. 8���� �̹����� �Ҵ����ּ���.");
            return;
        }
        foreach (Image cutImage in cutImages)
        {
            cutImage.gameObject.SetActive(false);
        }

        // �����̽��� �̹��� �ʱ�ȭ
        if (spacebarImage == null)
        {
            Debug.LogError("Spacebar Image�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }
        lastInputTime = Time.time;
        spacebarOriginalPos = spacebarImage.rectTransform.anchoredPosition;
    }
    public IEnumerator MoveSpacebarImage()
    {
        // 3�ȼ� �Ʒ��� �̵�
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos + new Vector2(0f, -3f);
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        // ���� ��ġ�� ����
        spacebarImage.rectTransform.anchoredPosition = spacebarOriginalPos;
    }

    private IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        interactionUI.ShowCookingMiniGameUI();
       
        cookingMinigame.StartCookingMinigame();

        //gameObject.SetActive(false); // ĵ���� ��Ȱ��ȭ

        //foreach (Image cutImage in cutImages)
        //{
        //    cutImage.gameObject.SetActive(false);
        //}

    }
}