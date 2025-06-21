using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.UI;

#region 손님 타입, 개성
public enum CustomerType
{
    King,       // 왕
    Queen,      // 여왕
    Citizen,    // 시민
    Peasant,     // 농부
    Rich        // 부자
}

public enum CustomerPersonality
{
    Standard,   // 보통
    Impatient,  // 성급함
    Patient,    // 인내심 많음
    Generous,   // 관대함
    Stingy      // 인색함
}
#endregion
public class CustomerAI : MonoBehaviour
{
    [Header("NavMeshAgent 설정")]
    public float agentAngularSpeed = 200f;

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

    [Header("Satisfaction UI")]
    public GameObject satisfactionUI; // 만족도 UI 오브젝트
    public GameObject happyIcon;
    public GameObject neutralIcon;
    public GameObject angryIcon;
    public float satisfactionFeedbackDuration = 2.5f; // 만족도 피드백 표시 시간
    #endregion

    #region 주문 관련
    public bool hasOrdered = false;
    public bool hasReceivedFood = false;
    public bool hasSpecialRequest = false; // 특별 요청 여부
    public bool hasTalkedAboutSpecialRequest = false; // 특별 요청에 대해 대화했는지 여부
    public List<FoodData> orderedItems = new List<FoodData>();
    public FoodData hintedFood; // 대화에서 언급된 음식
    public float eatingTime = 10f;
    private InventoryManager inventory; // 인벤토리 매니저
    #endregion

    #region 주문 UI
    public GameObject orderBubblePrefab;
    public Image CircleGaugeUnfill;
    public Image CircleGaugeFill;
    public GameObject specialOrderPrefab;
    public Image specialRequestImage; // 특별 요청 이미지 (대화 전)
    public Image specialRequestTalkedImage; // 특별 요청 이미지 (대화 후)
    #endregion

    #region State
    public CustomerWaiting waitingState { get; private set; }
    public CustomerWalk walkState { get; private set; }
    public CustomerSeat seatState { get; private set; }
    public CustomerExit exitState { get; private set; }
    #endregion

    [Header("Customer Type")]
    public CustomerType customerType;

    [Header("Customer Personality")]
    public CustomerPersonality personality = CustomerPersonality.Standard;

    private DialogueScript currentDialogue;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;  // 상호작용 키
    public float interactionDistance = 2f;   // 상호작용 가능 거리
    public bool showInteractionGizmo = true; // 기즈모 표시 여부
    private bool isPlayerNearby = false;
    private Transform player;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<CustomerStateMachine>();
        anim = GetComponent<Animator>();

        // NavMeshAgent 회전 속도 설정
        agent.angularSpeed = agentAngularSpeed;

        //스테이트
        waitingState = new CustomerWaiting(this, "Wait", stateMachine, agent);
        walkState = new CustomerWalk(this, "Walk", stateMachine, agent);
        seatState = new CustomerSeat(this, "Sit", stateMachine, agent);
        exitState = new CustomerExit(this, "Walk", stateMachine, agent);
        inventory = FindAnyObjectByType<InventoryManager>();

    }

    void Start()
    {
        stateMachine.ChangeState(waitingState);
        LoadDialogue();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        AssignRandomPersonality();

        // UI
        HideOrderBubble();
        HideGauge();
        HideSpecialRequestImage();
        HideSpecialRequestTalkedImage();
        HideSatisfactionIcons();
    }

    private void Update()
    {
        stateMachine.currentState?.StateUpdate();

        //음식 받기전 까지 대기 시간 증가 하면서 만족도 낮추기
        if (hasOrdered && !hasReceivedFood)
        {
            WatingMenu();
        }

        // UI가 활성화되어 있을 때 플레이어를 바라보도록 회전
        UILookAtPlayer(orderBubblePrefab, player);
        UILookAtPlayer(specialOrderPrefab, player);
        UILookAtPlayer(satisfactionUI, player);

        // 특별주문이 있거나 음식을 기다리는 상태일 때 상호작용 체크
        if (player != null && (hasSpecialRequest || (hasOrdered && !hasReceivedFood)))
        {
            NearCheckTalk();
        }
    }

    private void NearCheckTalk()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool wasPlayerNearby = isPlayerNearby; // 이전 상태 저장
        isPlayerNearby = distanceToPlayer <= interactionDistance;

        if (DialogueManager.Instance != null)
        {
            // 플레이어가 상호작용 범위를 벗어났을 때 텍스트 숨기기
            if (wasPlayerNearby && !isPlayerNearby)
            {
                DialogueManager.Instance.ShowInteractableText(false, this, distanceToPlayer);
                return;
            }

            if (isPlayerNearby && isSeated)
            {
                // 1. 특별주문 상태 (대화 전)
                if (hasSpecialRequest && !hasTalkedAboutSpecialRequest)
                {
                    DialogueManager.Instance.ShowInteractableText(true, this, distanceToPlayer);
                    if (Input.GetKeyDown(interactKey))
                    {
                        StartDialogue();
                    }
                }
                // 2. 특별주문 상태 (대화 후)
                else if (hasSpecialRequest && hasTalkedAboutSpecialRequest)
                {
                    DialogueManager.Instance.ShowInteractableText(true, this, distanceToPlayer);
                    if (Input.GetKeyDown(interactKey))
                    {
                        GiveFoodToCustomer();
                    }
                }
                // 3. 일반 음식 대기 상태
                else if (hasOrdered && !hasReceivedFood)
                {
                    DialogueManager.Instance.ShowInteractableText(true, this, distanceToPlayer);
                    if (Input.GetKeyDown(interactKey))
                    {
                        GiveFoodToCustomer();
                    }
                }
                // 4. 그 외
                else
                {
                    DialogueManager.Instance.ShowInteractableText(false, this, distanceToPlayer);
                }
            }
            else
            {
                DialogueManager.Instance.ShowInteractableText(false, this, distanceToPlayer);
            }
        }
    }

    //프리팹 내부에 있는 스크립터블 오브젝트인 푸드데이터를 가져오는 함수
    private FoodData GetFoodData(GameObject foodPrefab)
    {
        if (foodPrefab == null) return null;
        // 프리팹에서 FoodData 컴포넌트 찾기
        Item item = foodPrefab.GetComponent<Item>();
        if (item != null)
        {
            // FoodData가 있으면 반환
            if (item.data is FoodData foodData)
            {
                return foodData;
            }
        }
        // FoodData가 없으면 경고 메시지 출력
        return null;
    }

    // 실제로 음식을 전달하는 함수
    private void GiveFoodToCustomer()
    {
        // 이미 음식을 받았다면 리턴
        if (hasReceivedFood)
        {
            Debug.Log("이미 음식을 받았습니다!");
            return;
        }

        // 주문한 음식이 있는지 먼저 확인
        if (orderedItems == null || orderedItems.Count == 0)
        {
            Debug.LogWarning("손님이 주문한 음식이 없습니다.");
            return;
        }

        // 주문한 음식 이름 가져오기
        string orderedFoodName = orderedItems[0].itemName;
        Debug.Log($"주문한 음식: {orderedFoodName}");

        // 핫바에서 먼저 찾기
        GameObject foodPrefab = inventory.GetItemPrefab(orderedFoodName);
        FoodData reciveFood = GetFoodData(foodPrefab);
        if (foodPrefab != null)
        {
            Debug.Log($"찾은 음식 프리팹: {foodPrefab.name}");

            // 대소문자 구분 없이 비교
            bool isCorrectOrder = reciveFood.itemName.Equals(orderedFoodName, StringComparison.OrdinalIgnoreCase);

            if (isCorrectOrder)
            {
                Debug.Log("올바른 음식 전달!");

                // 특별주문이고 대화를 했고, 추측한 음식이 맞는 경우
                if (hasSpecialRequest && hasTalkedAboutSpecialRequest && hintedFood != null)
                {
                    bool isCorrectGuess = reciveFood.itemName.Equals(hintedFood.itemName, StringComparison.OrdinalIgnoreCase);
                    if (isCorrectGuess)
                    {
                        Debug.Log("추측한 음식이 정확합니다! 추가 만족도 획득!");
                        // 추측 성공 시 추가 만족도
                        SatisfactionScoreUpDown(25f);
                    }
                }

                // 주문한 음식의 FoodData를 사용
                ReceiveFood(new List<FoodData> { orderedItems[0] });
                // 음식 아이템 제거
                inventory.RemoveItem(orderedFoodName);
            }
            else
            {
                Debug.Log($"잘못된 음식 전달! 주문: {orderedFoodName}, 전달: {foodPrefab.name}");
                // 잘못된 음식 전달 시 만족도 감소
                StartCoroutine(EatingTime());
            }
        }
        else
        {
            Debug.Log($"주문한 음식 {orderedFoodName}이(가) 인벤토리에 없습니다!");
        }
    }

    private void WatingMenu()
    {
        waitingTime += Time.deltaTime;

        float decreaseAmount = -0.01f;
        float maxWait = maxWaitingTime;

        switch (personality)
        {
            case CustomerPersonality.Impatient:
                decreaseAmount *= 2f; // 더 빨리 감소
                maxWait *= 0.7f;      // 대기시간 짧음
                break;
            case CustomerPersonality.Patient:
                decreaseAmount *= 0.5f; // 더 천천히 감소
                maxWait *= 1.5f;        // 대기시간 김
                break;
        }

        if (waitingTime <= maxWait)
        {
            SatisfactionScoreUpDown(decreaseAmount);
            UpdateGauge(waitingTime, maxWait);
        }
        else
        {
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
            var sitPoint = mySeats[0].SitPoint.position;

            // NavMeshAgent 설정
            agent.updateRotation = true;
            agent.angularSpeed = agentAngularSpeed;

            // 경로 탐색 설정
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, sitPoint, NavMesh.AllAreas, path))
            {
                nextDestination = sitPoint;
                agent.SetPath(path);

                onArriveCallback = () =>
                {
                    stateMachine.ChangeState(seatState);
                };
                stateMachine.ChangeState(walkState);
            }
            else
            {
                Debug.LogWarning("좌석까지의 유효한 경로를 찾을 수 없습니다!");
            }
        }
    }

    // 대기열 위치로 이동
    public void MoveToQueuePoint(int index)
    {
        var queuePoints = SeatManager.Instance.queuePoints;
        if (index >= 0 && index < queuePoints.Count)
        {
            var point = queuePoints[index];

            // NavMeshAgent 설정
            agent.updateRotation = true;
            agent.angularSpeed = agentAngularSpeed;

            // 경로 탐색 설정
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, point.position, NavMesh.AllAreas, path))
            {
                // 경로가 유효한 경우에만 이동
                agent.SetPath(path);
            }
            else
            {
                Debug.LogWarning("유효한 경로를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogWarning($"[CustomerAI] queueIndex {index} is out of range! queuePoints.Count={queuePoints.Count}");
            if (queuePoints.Count > 0)
            {
                var point = queuePoints[queuePoints.Count - 1];
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, point.position, NavMesh.AllAreas, path))
                {
                    agent.SetPath(path);
                }
            }
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
    private void UpdateOrderBubble(List<FoodData> items)
    {
        if (orderBubblePrefab == null || items.Count == 0) return;

        Image[] images = orderBubblePrefab.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject.name == "Food Image")
            {
                image.sprite = items[0].icon;
                image.enabled = true;
                break;
            }
        }
    }
    public void UpdateGauge(float amount, float maxWaitTimeValue)
    {
        // 현재 대기 시간을 최대 대기 시간으로 나누어 0~1 사이의 값으로 정규화
        float normalizedAmount = amount / maxWaitTimeValue;
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

    // 특별 요청 이미지 표시/숨김 함수 (대화 전)
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

    // 특별 요청 이미지 표시/숨김 함수 (대화 후)
    public void ShowSpecialRequestTalkedImage()
    {
        if (specialRequestTalkedImage != null)
        {
            specialRequestTalkedImage.gameObject.SetActive(true);
        }
    }

    public void HideSpecialRequestTalkedImage()
    {
        if (specialRequestTalkedImage != null)
        {
            specialRequestTalkedImage.gameObject.SetActive(false);
        }
    }

    // 플레이어를 바라보도록 UI 오브젝트 회전
    private void UILookAtPlayer(GameObject uiObject, Transform player, float lerpSpeed = 10f)
    {
        if (uiObject != null && uiObject.activeSelf && player != null)
        {
            Vector3 direction = player.position - uiObject.transform.position;
            direction.y = 0; // Y축 회전만 적용

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                uiObject.transform.rotation = Quaternion.Slerp(
                    uiObject.transform.rotation,
                    targetRotation,
                    Time.deltaTime * lerpSpeed
                );
            }
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
        if (!hasReceivedFood) return;
        if (orderedItems.Count == 0) return;

        int basePrice = orderedItems[0].price;
        int finalPrice = basePrice;

        float tipRate = 0f;
        if (satisfactionScore >= 90)
            tipRate = 0.2f;
        else if (satisfactionScore >= 70)
            tipRate = 0.1f;

        switch (personality)
        {
            case CustomerPersonality.Generous:
                tipRate *= 1.5f; // 팁 많이 줌
                break;
            case CustomerPersonality.Stingy:
                tipRate *= 0.2f; // 팁 거의 안 줌
                break;
        }

        if (satisfactionScore >= 30)
            finalPrice = basePrice + Mathf.RoundToInt(basePrice * tipRate);
        else if (satisfactionScore < 30)
            finalPrice = 0;

        GiveMoneyToPlayer(finalPrice);
    }

    public void GiveMoneyToPlayer(int amount)
    {
        var moneyManager = MoneyManager.instance;

        if (moneyManager != null)
        {
            moneyManager.AddMoney(amount);
            Debug.Log($"돈이 추가되었습니다: {amount}. 현재 돈: {moneyManager.currentMoney}");
        }
        else
        {
            Debug.LogWarning("MoneyManager가 초기화되지 않았습니다!");
        }

    }

    private void AssignRandomPersonality()
    {
        Array values = Enum.GetValues(typeof(CustomerPersonality));
        personality = (CustomerPersonality)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    #endregion

    #region 주문 시스템
    public void StartOrdering()
    {
        StartCoroutine(WaitAndOrder());
    }

    private IEnumerator WaitAndOrder()
    {
        yield return new WaitForSeconds(1f);

        // 일반 주문만 처리 (특별주문은 CustomerSeat에서 처리)
        FoodData order = FoodManager.Instance.GetRandomFood();
        if (order != null)
        {
            List<FoodData> orderList = new List<FoodData> { order };
            PlaceOrder(orderList);
        }
    }

    public void PlaceOrder(List<FoodData> items)
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

    public void ReceiveFood(List<FoodData> deliveredItems)
    {
        if (hasOrdered && !hasReceivedFood)
        {
            // 현재는 1개씩만 주문하므로 첫 번째 아이템만 확인
            if (deliveredItems.Count > 0 && orderedItems.Count > 0)
            {
                string orderedFoodName = orderedItems[0].itemName;
                string deliveredFoodName = deliveredItems[0].itemName;

                // 주문한 음식과 전달된 음식이 같은지 확인
                bool isCorrectOrder = deliveredFoodName.Equals(orderedFoodName, StringComparison.OrdinalIgnoreCase);

                if (isCorrectOrder)
                {
                    hasReceivedFood = true;
                    HideOrderBubble();
                    HideGauge();
                    HideSpecialRequestImage();
                    HideSpecialRequestTalkedImage();

                    // 상호작용 텍스트 숨기기
                    if (DialogueManager.Instance != null)
                    {
                        DialogueManager.Instance.ShowInteractableText(false, this, 0f);
                    }

                    // 만족도 증가
                    SatisfactionScoreUpDown(10f);

                    // 추천 메뉴 확인
                    CheckRecommendedFood(deliveredItems[0]);

                    // ★ 애니메이션 반응 적용
                    StartCoroutine(ShowSatisfactionFeedbackForDuration());

                    StartCoroutine(EatingTime());
                }
                else
                {
                    // 잘못된 음식 전달 시 만족도 감소
                    SatisfactionScoreUpDown(-15f);
                    StartCoroutine(ShowSatisfactionFeedbackForDuration());
                    StartCoroutine(EatingTime());
                }
            }
        }
    }

    private IEnumerator ShowSatisfactionFeedbackForDuration()
    {
        // 1. 만족도에 맞는 아이콘 표시
        HideSatisfactionIcons(); 

        if (satisfactionScore >= 70)
        {
            if (happyIcon != null) happyIcon.SetActive(true);
        }
        else if (satisfactionScore >= 40)
        {
            if (neutralIcon != null) neutralIcon.SetActive(true);
        }
        else
        {
            if (angryIcon != null) angryIcon.SetActive(true);
        }

        // 2. 만족도와 성격에 맞는 애니메이션 재생
        float sitMotionValue = 0f;

        if (satisfactionScore >= 80)
            sitMotionValue = 1f; // thumbs up
        else if (satisfactionScore >= 60)
            sitMotionValue = 0.66f; // clap
        else if (satisfactionScore >= 40)
            sitMotionValue = 0.33f; // talk
        else
            sitMotionValue = 0f; // idle

        if (personality == CustomerPersonality.Generous && sitMotionValue < 1f)
            sitMotionValue = 0.66f;
        if (personality == CustomerPersonality.Impatient && sitMotionValue > 0.33f)
            sitMotionValue = 0.33f;

        anim.SetFloat("SitMotion", sitMotionValue);
        
        // 3. 설정된 시간만큼 기다림
        yield return new WaitForSeconds(satisfactionFeedbackDuration);

        // 4. 아이콘 숨기기
        HideSatisfactionIcons();
    }

    //앉은 상태에서 모션 컨트롤
    private void SetSitMotionByPersonalityAndSatisfaction()
    {
        float sitMotionValue = 0f;

        // 1. 만족도 기준
        if (satisfactionScore >= 80)
            sitMotionValue = 1f; // thumbs up
        else if (satisfactionScore >= 60)
            sitMotionValue = 0.66f; // clap
        else if (satisfactionScore >= 40)
            sitMotionValue = 0.33f; // talk
        else
            sitMotionValue = 0f; // idle

        // 2. 성격별로 추가 가중치
        if (personality == CustomerPersonality.Generous && sitMotionValue < 1f)
            sitMotionValue = 0.66f; // 관대한 손님은 박수라도 침
        if (personality == CustomerPersonality.Impatient && sitMotionValue > 0.33f)
            sitMotionValue = 0.33f; // 성급한 손님은 만족해도 talk까지만

        anim.SetFloat("SitMotion", sitMotionValue);
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

            // 특별주문 대화 후 처리
            if (hasSpecialRequest && !hasTalkedAboutSpecialRequest)
            {
                hasTalkedAboutSpecialRequest = true;
                HideSpecialRequestImage();
                ShowSpecialRequestTalkedImage();

                // 대화에서 힌트 음식 설정 (스크립터블 오브젝트에서 가져오기)
                SetHintedFoodFromDialogue();

                Debug.Log($"특별주문 대화 완료! 힌트 음식: {hintedFood?.itemName}");
            }
        }
    }

    // 대화 스크립터블 오브젝트에서 힌트 음식 설정
    private void SetHintedFoodFromDialogue()
    {
        if (currentDialogue != null && currentDialogue.lines != null)
        {
            // 대화 라인에서 recommendedFood가 있는 첫 번째 라인 찾기
            foreach (var line in currentDialogue.lines)
            {
                if (line.recommendedFood != null)
                {
                    hintedFood = line.recommendedFood;

                    // 특별주문 음식 설정 (주문 버블은 보여주지 않음 - 추측해야 하므로)
                    orderedItems = new List<FoodData> { hintedFood };
                    hasOrdered = true;
                    ShowGauge(); // 게이지만 보여줌
                    // ShowOrderBubble() 제거 - 추측해야 하므로
                    // UpdateOrderBubble(orderedItems) 제거 - 추측해야 하므로

                    Debug.Log($"대화에서 힌트 음식 설정: {hintedFood.itemName}");
                    return;
                }
            }

            // recommendedFood가 없으면 일반 주문으로 변경
            Debug.LogWarning("대화에서 힌트 음식을 찾을 수 없어 일반 주문으로 변경합니다.");
            hasSpecialRequest = false;
            hasTalkedAboutSpecialRequest = false;
            HideSpecialRequestTalkedImage();

            // 일반 주문으로 변경
            FoodData order = FoodManager.Instance.GetRandomFood();
            if (order != null)
            {
                List<FoodData> orderList = new List<FoodData> { order };
                PlaceOrder(orderList);
            }
        }
        else
        {
            // 대화 스크립트가 없으면 일반 주문으로 변경
            hasSpecialRequest = false;
            hasTalkedAboutSpecialRequest = false;
            HideSpecialRequestTalkedImage();

            FoodData order = FoodManager.Instance.GetRandomFood();
            if (order != null)
            {
                List<FoodData> orderList = new List<FoodData> { order };
                PlaceOrder(orderList);
            }
        }
    }

    // 추천 메뉴 확인
    public void CheckRecommendedFood(FoodData deliveredFood)
    {
        if (currentDialogue == null) return;

        // 첫 번째 추천 메뉴만 확인
        foreach (var line in currentDialogue.lines)
        {
            if (line.recommendedFood != null)
            {
                // 첫 번째 추천 메뉴와 일치하는지 확인
                if (line.recommendedFood.itemName == deliveredFood.itemName)
                {
                    // 특별주문이고 추측 성공한 경우 추가 만족도
                    if (hasSpecialRequest && hasTalkedAboutSpecialRequest)
                    {
                        SatisfactionScoreUpDown(25f);
                        Debug.Log($"{customerType} 손님이 추측한 음식 {deliveredFood.itemName}이 정확해서 추가 만족도를 획득했습니다!");
                    }
                    else
                    {
                        // 일반 추천 메뉴 만족도
                        SatisfactionScoreUpDown(20f);
                        Debug.Log($"{customerType} 손님이 추천 메뉴 {deliveredFood.itemName}을(를) 받아 만족도가 증가했습니다!");
                    }
                }
                // 첫 번째 추천 메뉴를 찾았으므로 더 이상 확인하지 않음
                return;
            }
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!showInteractionGizmo) return;

        // 상호작용 거리를 원으로 표시
        Gizmos.color = isPlayerNearby ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // 플레이어가 근처에 있을 때 연결선 표시
        if (player != null && isPlayerNearby)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // 상호작용 가능한 상태일 때 추가 표시
        if (isSeated && (hasSpecialRequest || (hasOrdered && !hasReceivedFood)))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, interactionDistance + 0.2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showInteractionGizmo) return;

        // 상호작용 거리를 원으로 표시 (선택된 상태)
        Gizmos.color = isPlayerNearby ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // 플레이어가 근처에 있을 때 연결선 표시
        if (player != null && isPlayerNearby)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);

#if UNITY_EDITOR
            float distance = Vector3.Distance(transform.position, player.position);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
                $"Distance: {distance:F1}\nInteraction: {interactionDistance:F1}");
#endif
        }

        // 상호작용 가능한 상태일 때 추가 표시
        if (isSeated && (hasSpecialRequest || (hasOrdered && !hasReceivedFood)))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionDistance + 0.3f);

            // 상태 정보 표시
#if UNITY_EDITOR
            string status = "";
            if (hasSpecialRequest && !hasTalkedAboutSpecialRequest)
                status = "Special Request (Before Talk)";
            else if (hasSpecialRequest && hasTalkedAboutSpecialRequest)
                status = "Special Request (After Talk)";
            else if (hasOrdered && !hasReceivedFood)
                status = "Waiting for Food";

            UnityEditor.Handles.Label(transform.position + Vector3.up * 3f, status);
#endif
        }
    }
    #endregion

    #region 퇴장
    public void CustormerExit()
    {
        HideOrderBubble();
        HideGauge();
        HideSpecialRequestImage();
        HideSpecialRequestTalkedImage();
        HideSatisfactionIcons();

        // 상호작용 텍스트 숨기기
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowInteractableText(false, this, 0f);
        }

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

    private void HideSatisfactionIcons()
    {
        if (happyIcon != null) happyIcon.SetActive(false);
        if (neutralIcon != null) neutralIcon.SetActive(false);
        if (angryIcon != null) angryIcon.SetActive(false);
    }
}