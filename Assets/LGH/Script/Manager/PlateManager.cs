using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlateManager : MonoBehaviour
{
    public static PlateManager instance;

    [Header("접시 정보")]
    public Stack<GameObject> plateStack; // 접시 스택
    public GameObject platePrefab; // 프리팹을 할당
    public int maxPlateCount = 20; // 최대 접시 개수
    public int currentPlateCount => plateStack.Count; // 현재 접시 개수

    [Header("만족도 정보")]
    public TextMeshProUGUI satisfactionText;
    public int Satisfaction = 100; // 만족도

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        plateStack = new Stack<GameObject>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            PushPlateStack();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PopPlateStack();
        }
    }

    // 손님이 음식을 먹고 나가면 Push
    public void PushPlateStack()
    {
        if (plateStack.Count >= maxPlateCount)
        {
            Debug.LogWarning("접시가 최대 개수에 도달했습니다.");
            return;
        }
        Vector3 PlateHeight = new Vector3(0, 0.1f * currentPlateCount, 0);
        plateStack.Push(Instantiate(platePrefab, transform.position + PlateHeight, Quaternion.identity));

        RemoveSatisfaction(5); // 접시를 쌓을 때마다 만족도 감소
    }

    // 설거지통에서 설거지 완료하면 Pop
    public void PopPlateStack()
    {
        if (plateStack.Count > 0)
        {
            GameObject plate = plateStack.Pop();
            Destroy(plate);

            AddSatisfaction(5); // 접시를 제거할 때마다 만족도 증가
        }
        else
        {
            Debug.LogWarning("접시가 없습니다.");
        }
    }

    public void AddSatisfaction(int amount)
    {
        Satisfaction += amount;
        UpdateGoldText();
    }
    public void RemoveSatisfaction(int amount)
    {
        Satisfaction -= amount;
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        if (satisfactionText != null)
        {
            satisfactionText.text = "만족도 : " + Satisfaction.ToString();
        }
        else
        {
            Debug.LogWarning("만족도가 할당 되지 않았습니다.");
        }
    }
}
