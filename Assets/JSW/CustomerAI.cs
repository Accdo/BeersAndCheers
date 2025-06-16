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
    public float timer = 0f; // 타이머
    public float waitingTime = 0f; // 대기 시간
    public float maxWaitingTime = 60f; // 최대 대기 시간
    #endregion

    #region 주문 관련
    public bool hasOrdered = false;
    public bool hasReceivedFood = false;
    public bool hasSpecialRequest = false; // 특별 요청 여부
    public List<FoodItem> orderedItems = new List<FoodItem>();
    public float eatingTime = 10f;
    #endregion

    #region 주문 UI
    public GameObject orderBubblePrefab;
    public Image CircleGaugeUnfill;
    public Image CircleGaugeFill;
    public Image specialRequestImage; // 특별 요청 이미지
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
        LoadDialogue();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // UI
        HideOrderBubble();
        HideGauge();
        HideSpecialRequestImage();
    }

    private void Update()
    {
        stateMachine.currentState?.StateUpdate();

        //음식 받기전 까지 대기 시간 증가 하면서 만족도 낮추기
        if (hasOrdered && !hasReceivedFood)
        {
            WatingMenu();
        }

        // 주문 버블이 활성화되어 있을 때 플레이어를 바라보도록 회전
        if (orderBubblePrefab != null && orderBubblePrefab.activeSelf && player != null)
        {
            // 주문 버블이 플레이어를 바라보도록 회전
            Vector3 direction = player.position - orderBubblePrefab.transform.position;
            direction.y = 0; // Y축 회전만 적용
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                orderBubblePrefab.transform.rotation = Quaternion.Slerp(
                    orderBubblePrefab.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 10f
                );
            }
        }

        if (player != null && hasSpecialRequest)
        {
            NearCheckTalk();
        }
    }

    private void NearCheckTalk()
    {
        // 플레이어와의 거리 체크
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distanceToPlayer <= interactionDistance;

        // 상호작용 가능 여부에 따라 텍스트 표시
        if (DialogueManager.Instance != null)
        {
            if (isPlayerNearby && isSeated )
            {
                DialogueManager.Instance.ShowInteractableText(true, this, distanceToPlayer);
                if (Input.GetKeyDown(interactKey))
                {
                    // 대화 시작
                    StartDialogue();
                }
            }
            else
            {
                DialogueManager.Instance.ShowInteractableText(false, this, distanceToPlayer);
            }
        }
    }

    private void WatingMenu()
    {
        waitingTime += Time.deltaTime;
        
        if (waitingTime <= maxWaitingTime)
        {
            SatisfactionScoreUpDown(-0.01f);
            UpdateGauge(waitingTime);
        }
        else
        {
            // 대기 시간 초과 시 퇴장
            CustormerExit();
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
    private void UpdateOrderBubble(List<FoodItem> items)
    {
        if (orderBubblePrefab == null || items.Count == 0) return;

        Image[] images = orderBubblePrefab.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject.name == "Food Image")
            {
                image.sprite = items[0].foodImage;
                image.enabled = true;
                break;
            }
        }
    }
    public void UpdateGauge(float amount)
    {
        // 현재 대기 시간을 최대 대기 시간으로 나누어 0~1 사이의 값으로 정규화
        float normalizedAmount = amount / maxWaitingTime;
        CircleGaugeFill.fillAmount = Mathf.Clamp(normalizedAmount, 0, 1);
    }
    public void ShowGauge()
    {
        if (CircleGaugeUnfill != null && CircleGaugeFill != null)
        {
            CircleGaugeUnfill.gameObject.SetActive(true);
            CircleGaugeFill.gameObject.SetActive(true);
        }
    }
    public void HideGauge()
    {
        if (CircleGaugeUnfill != null && CircleGaugeFill != null)
        {
            CircleGaugeUnfill.gameObject.SetActive(false);
            CircleGaugeFill.gameObject.SetActive(false);
        }
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
            CustormerExit();
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
        yield return new WaitForSeconds(2f);
        
        FoodItem order = FoodManager.Instance.GetRandomFood();
        if (order != null)
        {
            List<FoodItem> orderList = new List<FoodItem> { order };
            PlaceOrder(orderList);
        }
    }

    public void PlaceOrder(List<FoodItem> items)
    {
        if (!hasOrdered)
        {
            orderedItems = items;
            hasOrdered = true;
            ShowOrderBubble();
            ShowGauge();
            UpdateOrderBubble(items);
        }
    }

    public void ReceiveFood(List<FoodItem> deliveredItems)
    {
        if (hasOrdered && !hasReceivedFood)
        {
            // 현재는 1개씩만 주문
            bool isCorrectOrder = deliveredItems.Count > 0 && 
                                orderedItems.Count > 0 && 
                                deliveredItems[0].foodName == orderedItems[0].foodName;

            if (isCorrectOrder)
            {
                hasReceivedFood = true;
                HideOrderBubble();
                HideGauge();
                HideSpecialRequestImage();
                SatisfactionScoreUpDown(10f);

                CheckRecommendedFood(deliveredItems[0]);
                StartCoroutine(EatingTime());
            }
            else
            {
                SatisfactionScoreUpDown(-15f);
                StartCoroutine(EatingTime());
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



    // 특별 요청 이미지 표시/숨김 함수
    public void ShowSpecialRequestImage()
    {
        if (specialRequestImage != null)
        {
            specialRequestImage.gameObject.SetActive(true);
        }
    }

    public void HideSpecialRequestImage()
    {
        if (specialRequestImage != null)
        {
            specialRequestImage.gameObject.SetActive(false);
        }
    }


    #region 퇴장
    public void CustormerExit()
    {
        HideOrderBubble();
        HideGauge();
        stateMachine.ChangeState(exitState);
    }

    public void DestroyCustomer(int time = 0)
    {
        Destroy(this.gameObject, time);
    }

    private IEnumerator EatingTime()
    {
        yield return new WaitForSeconds(eatingTime);

        // 식사 완료 후 퇴장
        CustormerExit();
    }
    #endregion

    
}