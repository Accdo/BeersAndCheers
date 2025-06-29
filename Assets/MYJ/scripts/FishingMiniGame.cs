using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishingMiniGame : MonoBehaviour
{
    public static FishingMiniGame Instance;

    [Header("Game Settings")]
    public int fishGoal = 3;
    public float totalTimeLimit = 5f;          // ��ü ���� �ð� (��)
    public float minSpawnDelay = 0.4f;          // ����� ���� �� �ּ� ���ð�
    public float maxSpawnDelay = 1.2f;          // ����� ���� �� �ִ� ���ð�
    public float minVisibleTime = 1.0f;         // ����� ���� �ð� �ּ�
    public float maxVisibleTime = 1.8f;         // ����� ���� �ð� �ִ�

    [Header("References")]
    public GameObject miniGamePanel;            // �̴ϰ��� UI �г�
    public Button[] fishSlotButtons;            // ����� ���� ��ư �迭
    public Image[] catchCountIcons;             // ���� ī��Ʈ ǥ�� ������ �迭
    public Sprite emptyIcon;                     // �� ������ ��������Ʈ
    public Sprite filledIcon;                    // ä���� ������ ��������Ʈ

    [Header("Time UI")]
    public Image timeBarFill;                    // �ð��� Image (Fill Amount ���)

    private int catchCount = 0;                  // ���� ����� ��
    private Action<bool> onComplete;             // ���� ���� �ݹ�

    private void Awake()
    {
        Instance = this;
        miniGamePanel.SetActive(false);
    }

    public void StartGame(Action<bool> callback)
    {
        // �÷��̾� �̵� ���� ����
        Player_LYJ player = GameObject.FindWithTag("Player")?.GetComponent<Player_LYJ>();
        player?.MouseVisible(true);
        player?.StartOtherWork();

        catchCount = 0;
        onComplete = callback;
        UpdateCatchIcons();
        miniGamePanel.SetActive(true);

        // �ð��� �ʱ�ȭ
        if (timeBarFill != null)
            timeBarFill.fillAmount = 1f;

        StartCoroutine(GameLoop());
    }

    private void UpdateCatchIcons()
    {
        for (int i = 0; i < catchCountIcons.Length; i++)
        {
            catchCountIcons[i].sprite = (i < catchCount) ? filledIcon : emptyIcon;
        }
    }

    IEnumerator GameLoop()
    {
        float elapsedTime = 0f;

        while (catchCount < fishGoal && elapsedTime < totalTimeLimit)
        {
            // �ð��� ������Ʈ
            if (timeBarFill != null)
                timeBarFill.fillAmount = 1f - (elapsedTime / totalTimeLimit);

            // ���� ����� ��ư ����
            int index = UnityEngine.Random.Range(0, fishSlotButtons.Length);
            Button btn = fishSlotButtons[index];

            // ����� �̹��� & �ִϸ����� ã��
            Transform fishImage = btn.transform.Find("FishImage");
            if (fishImage == null)
            {
                Debug.LogError($"{btn.name} �ȿ� FishImage��� �̸��� �ڽ��� �����ϴ�!");
                yield break;
            }

            Animator anim = fishImage.GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError($"{fishImage.name}�� Animator�� �����ϴ�!");
                yield break;
            }
            bool caught = false;
            bool isFishVisible = true;
            // Ŭ�� ������ �ʱ�ȭ �� ���
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (!caught&& isFishVisible)
                {
                    caught = true;
                    catchCount++;
                    UpdateCatchIcons();
                }
            });

            // ����� ��Ÿ���� �ִϸ��̼� ����
            fishImage.gameObject.SetActive(true);
            anim.ResetTrigger("Pop");
            anim.SetTrigger("Pop");
            SoundManager.Instance.Play("FishingSFX");

            float visibleTime = UnityEngine.Random.Range(minVisibleTime, maxVisibleTime);
            float timer = 0f;

            // ����� �������� ���� ����
            while (timer < visibleTime && elapsedTime < totalTimeLimit)
            {
                float delta = Time.deltaTime;
                timer += delta;
                elapsedTime += delta;

                if (timeBarFill != null)
                    timeBarFill.fillAmount = 1f - (elapsedTime / totalTimeLimit);

                if (caught) break;

                yield return null;
            }

            fishImage.gameObject.SetActive(false);
            isFishVisible = false;

            if (elapsedTime >= totalTimeLimit)
                break;

            // ����� �� ���ð� (������ ���� ������Ʈ)
            float delay = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            float delayTimer = 0f;

            while (delayTimer < delay && elapsedTime < totalTimeLimit)
            {
                float delta = Time.deltaTime;
                delayTimer += delta;
                elapsedTime += delta;

                if (timeBarFill != null)
                    timeBarFill.fillAmount = 1f - (elapsedTime / totalTimeLimit);

                yield return null;
            }
        }

        miniGamePanel.SetActive(false);

        // �÷��̾� �̵� ���� ����
        Player_LYJ player = GameObject.FindWithTag("Player")?.GetComponent<Player_LYJ>();
        player?.MouseVisible(false);
        player?.EndOtherWork();

        // ���� ���� �ݹ� ȣ��
        bool success = (catchCount >= fishGoal);
        onComplete?.Invoke(success);
    }
}
