using UnityEngine;

public class Horse : MonoBehaviour, IInteractable
{
    public string GetCursorType() => "Horse";
    public string GetInteractionID() => "Horse";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;

    [SerializeField] private float ridingSpeed = 2f;
    [SerializeField] private Transform ridingPoint;
    [SerializeField] private float camSensitivityVertical = 2f;
    [SerializeField] private float camSensitivityHorizontal = 2f;
    [SerializeField] private float camRotationLimit = 80f;
    [SerializeField] private float soundInterval = 0.5f; // 뛰는 사운드 간격

    private Player_LYJ ridingPlayer;
    private bool isRiding;
    private float currentCamRotationX;
    private Rigidbody rb;
    private Animator animator;
    private float soundTimer; // 사운드 재생 타이머
    private bool wasMovingLastFrame; // 이전 프레임에서 이동 중이었는지 확인

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        soundTimer = 0f; // 타이머 초기화
    }

    public void Interact()
    {
        if (!isRiding) StartRiding();
        else StopRiding();
    }

    private void StartRiding()
    {
        SoundManager.Instance.Play("HorseRidingSFX");
        ridingPlayer = GH_GameManager.instance.player;
        if (ridingPlayer == null) return;

        currentCamRotationX = ridingPlayer.GetComponent<Player_LYJ>().currentCamRotationX;
        ridingPlayer.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        ridingPlayer.transform.position = ridingPoint.position;
        ridingPlayer.transform.SetParent(ridingPoint, true);

        isRiding = true;
    }

    private void StopRiding()
    {
        if (ridingPlayer == null) return;

        ridingPlayer.GetComponent<Player_LYJ>().currentCamRotationX = currentCamRotationX;
        ridingPlayer.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        Vector3 dismountPosition = transform.position + transform.right * 1.5f;
        dismountPosition.y = ridingPlayer.transform.position.y;
        ridingPlayer.transform.SetParent(null);
        ridingPlayer.transform.position = dismountPosition;

        isRiding = false;
        ridingPlayer = null;
        soundTimer = 0f; // 타이머 리셋
        wasMovingLastFrame = false; // 이동 상태 리셋
    }

    private void Update()
    {
        if (isRiding && ridingPlayer != null)
        {
            ridingPlayer.transform.position = ridingPoint.position;
            HandleMovement();
            RotateCam();
            RotatePlayer();
        }

        // 애니메이션 상태 업데이트
        UpdateAnimation();
    }

    private void HandleMovement()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");
        bool isMoving = Mathf.Abs(moveDirX) > 0 || Mathf.Abs(moveDirZ) > 0;

        Vector3 moveDirection = new Vector3(moveDirX, 0f, moveDirZ).normalized;
        Vector3 moveVelocity = transform.TransformDirection(moveDirection) * ridingSpeed;

        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);

        // 사운드 처리
        if (isMoving)
        {
            if (!wasMovingLastFrame || soundTimer <= 0f)
            {
                SoundManager.Instance.Play("HorseRunSFX");
                soundTimer = soundInterval; // 타이머를 설정된 간격으로 초기화
            }
            soundTimer -= Time.deltaTime; // 타이머 감소
        }
        else
        {
            soundTimer = 0f; // 이동이 멈추면 타이머 리셋
        }

        wasMovingLastFrame = isMoving; // 현재 프레임의 이동 상태 저장
    }

    private void UpdateAnimation()
    {
        if (isRiding && ridingPlayer != null)
        {
            float moveInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical"));
            animator.SetInteger("animation", moveInput > 0 ? 2 : 0);
        }
    }

    private void RotateCam()
    {
        float rotationX = Input.GetAxisRaw("Mouse Y");
        float camRotX = rotationX * camSensitivityVertical;
        currentCamRotationX -= camRotX;
        currentCamRotationX = Mathf.Clamp(currentCamRotationX, -camRotationLimit, camRotationLimit);

        Camera.main.transform.localEulerAngles = new Vector3(currentCamRotationX, 0f, 0f);
    }

    private void RotatePlayer()
    {
        float rotationY = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, rotationY, 0f) * camSensitivityHorizontal;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(characterRotationY));
        ridingPlayer.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}