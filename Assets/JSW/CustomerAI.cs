using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

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
    public float satisfactionFeedbackDuration = 2.5f; // 만족도 피드백 표시 시간

    [Header("Satisfaction UI")]
    public GameObject satisfactionUI; // 만족도 UI 오브젝트
    public GameObject happyIcon;
    public GameObject neutralIcon;
    public GameObject angryIcon;
    #endregion

    #region 주문 관련
    [Header("주문 관련")]
    public bool hasOrdered = false;
    public bool hasReceivedFood = false;
    public bool hasSpecialRequest = false; // 특별 요청 여부
    public bool hasTalkedAboutSpecialRequest = false; // 특별 요청에 대해 대화했는지 여부
    public List<FoodData> orderedItems = new List<FoodData>();
    public FoodData hintedFood; // 대화에서 언급된 음식
    public float eatingTime = 10f;
    private InventoryManager inventory; // 인벤토리 매니저
    public GameObject spawnedFoodPrefab;
    #endregion

    #region 주문 UI
    public GameObject orderBubblePrefab;
    public Image CircleGaugeUnfill;
    public Image CircleGaugeFill;
    public GameObject specialOrderPrefab;
    public Image specialRequestImage; // 특별 요청 이미지 (대화 전)
    public Image specialRequestTalkedImage; // 특별 요청 이미지 (대화 후)
    #endregion

    #region 퀘스트
    [Header("퀘스트")]
    public bool hasGivenQuest = false;
    public bool isQuestCustomer = false; // 퀘스트 손님 여부
    private bool hasOfferedQuest = false; // 🔥 추가: 이번 방문에서 퀘스트 제안을 이미 했는가?
    #endregion

    #region State
    public CustomerWaiting waitingState { get; private set; }
    public CustomerWalk walkState { get; private set; }
    public CustomerSeat seatState { get; private set; }
    public CustomerExit exitState { get; private set; }
    public CustomerWandering wanderingState { get; private set; }
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
        wanderingState = new CustomerWandering(this, "Walk", stateMachine, agent);
        waitingState = new CustomerWaiting(this, "Wait", stateMachine, agent);
        walkState = new CustomerWalk(this, "Walk", stateMachine, agent);
        seatState = new CustomerSeat(this, "Sit", stateMachine, agent);
        exitState = new CustomerExit(this, "Walk", stateMachine, agent);

    }

    void Start()
    {
        stateMachine.ChangeState(wanderingState);
        LoadDialogue();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        inventory = FindAnyObjectByType<InventoryManager>();

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

        // 퀘스트 상호작용은 퀘스트를 제안한 후에만 가능
        if (isQuestCustomer && hasOfferedQuest && !hasGivenQuest)
        {
            if (player != null)
                NearCheckQuest();
            return; 
        }

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

    // 퀘스트 상호작용 함수 수정
    private void NearCheckQuest()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distanceToPlayer <= interactionDistance;

        if (DialogueManager.Instance == null) return;
        
        // 🔥 수정: hasGivenQuest 플래그를 정확히 확인하여 중복 제안 차단
        if (isQuestCustomer && !hasGivenQuest && isPlayerNearby && isSeated)
        {
            DialogueManager.Instance.ShowInteractableText(true, this, distanceToPlayer, "퀘스트 받기");
            if (Input.GetKeyDown(interactKey))
            {
                // TryGiveQuest는 UI를 열어주는 역할만 
                TryGiveQuest();
            }
        }
        else
        {
            // 그 외의 모든 경우엔 상호작용 텍스트를 끔
            DialogueManager.Instance.ShowInteractableText(false, this, distanceToPlayer);
        }
    }

    // 실제로 음식을 전달하는 함수
    private void GiveFoodToCustomer()
    {
        // --- 1. 사전 조건 확인 ---
        if (hasReceivedFood)
        {
            Debug.Log("이미 음식을 받았습니다!");
            return;
        }
        if (inventory == null)
        {
            Debug.LogError("InventoryManager를 찾을 수 없습니다!");
            return;
        }
        if (orderedItems == null || orderedItems.Count == 0)
        {
            Debug.LogWarning("손님이 주문한 음식이 없습니다.");
            return;
        }

        // --- 2. 아이템 검색 ---
        string orderedFoodName = orderedItems[0].itemName;
        GameObject foundPrefab = inventory.GetItemPrefab(orderedFoodName);
      
        // --- 3. 검색 결과에 따라 처리 ---
        if (foundPrefab == null)
        {
            Debug.LogWarning($"[GiveFoodToCustomer] 인벤토리에서 '{orderedFoodName}'을(를) 찾지 못했습니다.");
            return; // 아이템이 없으면 여기서 즉시 종료
        }
        
        // --- 4. 찾은 아이템으로 로직 수행 ---
        Debug.Log($"[GiveFoodToCustomer] 인벤토리에서 '{foundPrefab.name}'(을)를 찾았습니다.");
        FoodData receivedFoodData = GetFoodData(foundPrefab);

        if (receivedFoodData == null)
        {
            Debug.LogWarning($"'{foundPrefab.name}'은(는) 유효한 음식이 아닙니다.");
            return;
        }

        // 주문이 맞는지 확인 (대소문자, 공백 등 무시)
        bool isCorrectOrder = receivedFoodData.itemName.Trim().Equals(orderedFoodName.Trim(), System.StringComparison.OrdinalIgnoreCase);

        if (isCorrectOrder)
        {
            Debug.Log("올바른 음식을 전달했습니다.");
            Vector3 spawnPos = transform.position + transform.forward * 0.6f + Vector3.up * 1f;
            spawnedFoodPrefab = Instantiate(foundPrefab, spawnPos, Quaternion.identity);

            ReceiveFood(new List<FoodData> { orderedItems[0] });
            
            // 인벤토리에서 아이템을 제거할 때는, 실제로 받은 아이템의 정확한 이름으로 요청
            inventory.RemoveItem(receivedFoodData.itemName);
        }
        else
        {
            Debug.Log($"잘못된 음식을 전달했습니다! (주문: {orderedFoodName}, 전달: {receivedFoodData.itemName})");
            SatisfactionScoreUpDown(-15f);
            StartCoroutine(ShowSatisfactionFeedbackForDuration());
            StartCoroutine(EatingTime());
        }
    }

    // 아이템 프리팹에서 FoodData를 안전하게 추출하는 함수
    private FoodData GetFoodData(GameObject foodPrefab)
    {
        if (foodPrefab == null)
        {
            return null;
        }
        
        Item itemComponent = foodPrefab.GetComponent<Item>();
        if (itemComponent != null && itemComponent.data is FoodData foodData)
        {
            return foodData;
        }

        return null;
    }

    // `ReceiveFood` 함수도 혹시 모를 문제를 방지하기 위해 조금 더 안전하게 만듭니다.
    public void ReceiveFood(List<FoodData> deliveredItems)
    {
        if (!hasOrdered || hasReceivedFood) return;

        // 현재는 1개씩만 주문하므로 첫 번째 아이템만 확인
        if (deliveredItems != null && deliveredItems.Count > 0 && orderedItems != null && orderedItems.Count > 0)
        {
            string orderedFoodName = orderedItems[0].itemName;
            FoodData deliveredFood = deliveredItems[0]; // 전달받은 음식 데이터

            // 주문한 음식과 전달된 음식이 같은지 확인
            bool isCorrectOrder = deliveredFood.itemName.Trim().Equals(orderedFoodName.Trim(), System.StringComparison.OrdinalIgnoreCase);

            if (isCorrectOrder)
            {
                hasReceivedFood = true;
                HideOrderBubble();
                HideGauge();
                HideSpecialRequestImage();
                HideSpecialRequestTalkedImage();
                HideSatisfactionIcons();

                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowInteractableText(false, this, 0f);
                }

                // --- 신선도에 따른 만족도 추가 계산 (0-100 기준) ---
                float freshness = deliveredFood.freshPoint; // FoodData에서 신선도 가져오기 (0 ~ 100)
                float freshnessBonus = 0f;

                if (freshness >= 90)
                {
                    freshnessBonus = 15f; // 매우 신선함
                    Debug.Log($"<color=green>매우 신선한 음식 (신선도: {freshness:F0})! 만족도 +{freshnessBonus}</color>");
                }
                else if (freshness >= 70)
                {
                    freshnessBonus = 5f; // 신선함
                    Debug.Log($"<color=cyan>신선한 음식 (신선도: {freshness:F0}). 만족도 +{freshnessBonus}</color>");
                }
                else if (freshness < 40)
                {
                    freshnessBonus = -15f; // 신선하지 않음
                    Debug.Log($"<color=red>신선하지 않은 음식 (신선도: {freshness:F0}). 만족도 {freshnessBonus}</color>");
                }
                SatisfactionScoreUpDown(freshnessBonus);
                // ------------------------------------

                CheckRecommendedFood(deliveredFood);
                StartCoroutine(ShowSatisfactionFeedbackForDuration());
                StartCoroutine(EatingTime());
            }
            // `else` (잘못된 음식 전달) 부분은 GiveFoodToCustomer에서 이미 처리하므로 여기서는 생략
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
    
    #region 퀘스트
    public bool TryGiveQuest()
    {
        if (hasGivenQuest || QuestManager.Instance == null) return false;

        QuestData questData = QuestManager.Instance.GetRandomQuest();
        if (questData == null) return false;
        
        if (QuestUI.Instance != null)
        {
            // 퀘스트 창을 열고, 퀘스트 데이터와 '나 자신(this)'의 정보를 전달
            QuestUI.Instance.ToggleQuestPanel(); 
            QuestUI.Instance.ShowQuestDetail(questData, null, this);

            // 퀘스트 창을 띄운 후에는 상호작용 텍스트를 즉시 숨깁니다.
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowInteractableText(false, this, 0f);
            }
        }
        
        return true;
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
        // isExiting 변수는 이 함수 내에서만 사용되므로 멤버 변수로 만들 필요가 없습니다.
        // 기존 코드에 isExiting 체크가 없다면 추가해주는 것이 안전합니다.
        // if (isExiting) return; // 만약 이와 유사한 체크가 이미 있다면 그대로 두세요.

        // 손님이 나갈 때 퀘스트 관련 상태 초기화
        if (isQuestCustomer)
        {
            CustomerSpawnManager.Instance?.OnQuestCustomerLeft();
        }
        hasGivenQuest = false; // 다음 방문을 위해 초기화
        hasOfferedQuest = false; 

        stateMachine.ChangeState(exitState);
        HideOrderBubble();
        HideGauge();
        HideSpecialRequestImage();
        HideSpecialRequestTalkedImage();
        HideSatisfactionIcons();

        // 생성된 음식 프리팹 삭제
        if (spawnedFoodPrefab != null)
        {
            Destroy(spawnedFoodPrefab);
            spawnedFoodPrefab = null;
        }

        // 상호작용 텍스트 숨기기
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowInteractableText(false, this, 0f);
        }
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

    private void HideSatisfactionIcons()
    {
        if (happyIcon != null) happyIcon.SetActive(false);
        if (neutralIcon != null) neutralIcon.SetActive(false);
        if (angryIcon != null) angryIcon.SetActive(false);
    }

    public void UpdateQuestIcon()
    {
        if (specialRequestImage != null)
        {
            // 퀘스트 손님이고 아직 퀘스트를 받지 않았으면 아이콘 표시
            bool shouldShow = isQuestCustomer && !hasGivenQuest;
            specialRequestImage.gameObject.SetActive(shouldShow);
        }
    }

    // 🔥 추가: 퀘스트를 제안하는 로직
    public void TryOfferQuest()
    {
        // 중복 제안 방지
        if (hasOfferedQuest) return;

        // 퀘스트 매니저에 나를 "퀘스트 제공자"로 등록 시도
        if (QuestManager.Instance.TrySetQuestGiver(this))
        {
            hasOfferedQuest = true; // 제안 완료!
            UpdateQuestIcon(); // 퀘스트 아이콘 표시
            Debug.Log($"<color=cyan>{name} 손님이 퀘스트 제안을 시작합니다.</color>");
        }
    }

    // QuestManager가 호출할 함수
    public void ConfirmQuestGiven()
    {
        this.hasGivenQuest = true;
        UpdateQuestIcon(); // 퀘스트 제안 아이콘 숨기기
    }
}