using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.UI;

//임시로 음식

public enum CustomerType
{
    King,       // 왕
    Queen,      // 여왕
    Citizen,    // 시민
    Peasant,     // 농부
    Rich        // 부자
}

public class CustomerAI : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public int teamSize = 1;
    public CustomerGroup myGroup;
    public List<Seat> mySeats = new List<Seat>();
    public bool isSeated = false;
    public int queueIndex = -1;
    public Animator anim;
    private CustomerStateMachine stateMachine;
    public Vector3 nextDestination;
    public Action onArriveCallback;

    #region 만족도
    public float satisfactionScore = 100;
    public float waitingTime = 0f; // 대기 시간
    public float maxWaitingTime = 60f; // 최대 대기 시간
    #endregion

    #region 주문 관련
    public bool hasOrdered = false;
    public bool hasReceivedFood = false;
    public float orderWaitTime = 0f;
    public float maxOrderWaitTime = 30f;
    public List<FoodItem> orderedItems = new List<FoodItem>();
    public float eatingTime = 10f;
    #endregion

    #region 주문 UI
    public GameObject orderBubblePrefab;
    #endregion

    #region State
    public CustomerWaiting waitingState { get; private set; }
    public CustomerWalk walkState { get; private set; }
    public CustomerSeat seatState { get; private set; }
    public CustomerExit exitState { get; private set; }
    #endregion

    [Header("Customer Type")]
    public CustomerType customerType;

    private DialogueScript currentDialogue;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;  // 상호작용 키
    public float interactionDistance = 3f;   // 상호작용 가능 거리
    private bool isPlayerNearby = false;
    private Transform player;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<CustomerStateMachine>();
        anim = GetComponent<Animator>();

        //스테이트
        waitingState = new CustomerWaiting(this, "Wait", stateMachine, agent);
        walkState = new CustomerWalk(this, "Walk", stateMachine, agent);
        seatState = new CustomerSeat(this, "Sit", stateMachine, agent);
        exitState = new CustomerExit(this, "Walk", stateMachine, agent);
    }

    void Start()
    {
        stateMachine.ChangeState(waitingState);
        HideOrderBubble();
        LoadDialogue();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        stateMachine.currentState?.StateUpdate();

        //음식 받기전 까지 대기 시간 증가 하면서 만족도 낮추기
        if (hasOrdered && !hasReceivedFood)
        {
            WatingMenu();
        }

        if (player != null)
        {
            // 플레이어와의 거리 체크
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            isPlayerNearby = distanceToPlayer <= interactionDistance;

            // 상호작용 가능 여부에 따라 텍스트 표시
            if (DialogueManager.Instance != null)
            {
                // 상호작용 키 입력 체크
                if (isPlayerNearby && isSeated)
                {
                    DialogueManager.Instance.ShowInteractableText(true, this, distanceToPlayer);
                    if (Input.GetKeyDown(interactKey))
                    {
                        StartDialogue();
                    }
                }
                else
                {
                    DialogueManager.Instance.ShowInteractableText(false, this, distanceToPlayer);
                }
            }
        }
    }

    private void WatingMenu()
    {
        if (waitingTime <= maxWaitingTime)
        {
            waitingTime += Time.deltaTime;
            SatisfactionScoreUpDown(-waitingTime / 10f);

        }
    }

    #region 이동
    // 좌석 배정 시 상태머신에서 호출
    public void AssignSeats(List<Seat> seats)
    {
        mySeats = seats;
        if (mySeats.Count > 0)
        {
            nextDestination = mySeats[0].SitPoint.position;
            onArriveCallback = () =>
            {
                // 도착 후 착석 상태로 전이
                stateMachine.ChangeState(seatState);
            };
            stateMachine.ChangeState(walkState);
        }
    }

    // 대기열 위치로 이동
    public void MoveToQueuePoint(int index)
    {
        var queuePoints = SeatManager.Instance.queuePoints;
        if (index >= 0 && index < queuePoints.Count)
        {
            var point = queuePoints[index];
            agent.SetDestination(point.position);
        }
        else
        {
            Debug.LogWarning($"[CustomerAI] queueIndex {index} is out of range! queuePoints.Count={queuePoints.Count}");
            // 대기 위치가 부족할 때는 마지막 위치로 이동하거나, 적당한 대체 위치로 이동
            if (queuePoints.Count > 0)
                agent.SetDestination(queuePoints[queuePoints.Count - 1].position);
        }
    }
    #endregion

    #region Customer UI
    public void ShowOrderBubble()
    {
        if (orderBubblePrefab != null)
            orderBubblePrefab.SetActive(true);
    }
    public void HideOrderBubble()
    {
        if (orderBubblePrefab != null)
            orderBubblePrefab.SetActive(false);
    }
    #endregion

    #region 만족도
    public void SatisfactionScoreUpDown(float amount)
    {
        satisfactionScore += amount;
        satisfactionScore = Mathf.Clamp(satisfactionScore, 0, 100);
        if (satisfactionScore <= 0)
        {
            // 만족도가 0 이하가 되면 퇴장 상태로 전이
            RequestExit();
        }

    }

    //만족도에 따라서 결과처리
    public void ResultOfStisfaciton()
    {
        if (satisfactionScore >= 90)
        {
            //팁주기 로직
            // ex) 20% 팁 주기

        }
        else if (satisfactionScore >= 70)
        {
            //팁주기 로직
            // ex) 10% 팁 주기
        }
        else if (satisfactionScore >= 30)
        {
            // 음식가격만 지불하기

        }
        else
        {
            //디메리트 주기
            // ex) 음식 가격 안주기

        }

    }
    #endregion

    #region 주문 시스템
    public void StartOrdering()
    {
        StartCoroutine(WaitAndOrder());
    }

    private IEnumerator WaitAndOrder()
    {
        // 잠시 대기 후 주문
        yield return new WaitForSeconds(2f);
        
        if (!hasOrdered)
        {
            // 하나의 음식만 주문
            FoodItem order = FoodManager.Instance.GetRandomFood();
            if (order != null)
            {
                List<FoodItem> orderList = new List<FoodItem> { order };
                PlaceOrder(orderList);
            }
        }
    }

    public void StartExiting()
    {
        StartCoroutine(WaitAndExit());
    }

    private IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(10f); // 10초 후 퇴장
        stateMachine.ChangeState(exitState);
    }

    public void PlaceOrder(List<FoodItem> items)
    {
        if (!hasOrdered)
        {
            orderedItems = items;
            hasOrdered = true;
            ShowOrderBubble();
            // 주문 UI에 주문한 음식 표시
            UpdateOrderBubble(items);
        }
    }

    public void ReceiveFood(List<FoodItem> deliveredItems)
    {
        if (hasOrdered && !hasReceivedFood)
        {
            // 주문한 음식과 배달된 음식이 일치하는지 확인
            bool isCorrectOrder = CheckOrderCorrectness(deliveredItems);

            if (isCorrectOrder)
            {
                hasReceivedFood = true;
                HideOrderBubble();
                SatisfactionScoreUpDown(10f); // 정확한 주문으로 만족도 증가

                // 각 배달된 음식이 추천 메뉴인지 확인
                foreach (var food in deliveredItems)
                {
                    CheckRecommendedFood(food);
                }

                // 식사 시작
                StartCoroutine(EatingTime());
            }
            else
            {
                SatisfactionScoreUpDown(-15f); // 잘못된 주문으로 만족도 감소
            }
        }
    }

    private bool CheckOrderCorrectness(List<FoodItem> deliveredItems)
    {
        if (deliveredItems.Count != orderedItems.Count)
            return false;

        // 주문한 음식과 배달된 음식이 일치하는지 확인
        foreach (var orderedItem in orderedItems)
        {
            bool found = false;
            foreach (var deliveredItem in deliveredItems)
            {
                if (orderedItem.foodName == deliveredItem.foodName)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                return false;
        }
        return true;
    }

    private void UpdateOrderBubble(List<FoodItem> items)
    {
        if (orderBubblePrefab == null) return;

        // 주문 버블의 모든 이미지 컴포넌트 찾기
        Image[] images = orderBubblePrefab.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            // 이미지의 이름이나 태그를 확인하여 음식 이미지를 표시할 이미지 컴포넌트 찾기
            if (image.gameObject.name == "Food Image")
            {
                if (items.Count > 0)
                {
                    image.sprite = items[0].foodImage;
                    image.enabled = true;
                }
                break;
            }
        }
    }
    #endregion

    #region 대화 시스템
    private void LoadDialogue()
    {
        string dialoguePath = $"Customer/Dialogues/{customerType}Dialogue";
        currentDialogue = Resources.Load<DialogueScript>(dialoguePath);
        
        if (currentDialogue == null)
        {
            Debug.LogWarning($"Dialogue not found at path: {dialoguePath}");
        }
    }

    public void StartDialogue()
    {
        if (currentDialogue != null && isSeated == true)
        {
            DialogueManager.Instance.StartDialogue(currentDialogue);
        }
    }

    // 추천 메뉴 확인
    public void CheckRecommendedFood(FoodItem deliveredFood)
    {
        if (currentDialogue == null) return;

        // 첫 번째 추천 메뉴만 확인
        foreach (var line in currentDialogue.lines)
        {
            if (line.recommendedFood != null)
            {
                // 첫 번째 추천 메뉴와 일치하는지 확인
                if (line.recommendedFood.foodName == deliveredFood.foodName)
                {
                    // 추천 메뉴가 맞으면 만족도 증가
                    SatisfactionScoreUpDown(20f);
                    Debug.Log($"{customerType} 손님이 추천 메뉴 {deliveredFood.foodName}을(를) 받아 만족도가 증가했습니다!");
                }
                // 첫 번째 추천 메뉴를 찾았으므로 더 이상 확인하지 않음
                return;
            }
        }
    }
    #endregion

    public void RequestExit()
    {
        HideOrderBubble();
        stateMachine.ChangeState(exitState);
    }

    public void DestroyCustomer(int time = 0)
    {
        Destroy(this.gameObject, time);
    }

    private IEnumerator EatingTime()
    {
        // 식사 시간 (30초)
        yield return new WaitForSeconds(eatingTime);
        
        // 식사 완료 후 퇴장
        StartExiting();
    }
}