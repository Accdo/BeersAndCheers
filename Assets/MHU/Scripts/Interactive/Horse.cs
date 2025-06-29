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
    [SerializeField] private float soundInterval = 0.5f; // �ٴ� ���� ����

    private Player_LYJ ridingPlayer;
    private bool isRiding;
    private float currentCamRotationX;
    private Rigidbody rb;
    private Animator animator;
    private float soundTimer; // ���� ��� Ÿ�̸�
    private bool wasMovingLastFrame; // ���� �����ӿ��� �̵� ���̾����� Ȯ��

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        soundTimer = 0f; // Ÿ�̸� �ʱ�ȭ
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
        soundTimer = 0f; // Ÿ�̸� ����
        wasMovingLastFrame = false; // �̵� ���� ����
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

        // �ִϸ��̼� ���� ������Ʈ
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

        // ���� ó��
        if (isMoving)
        {
            if (!wasMovingLastFrame || soundTimer <= 0f)
            {
                SoundManager.Instance.Play("HorseRunSFX");
                soundTimer = soundInterval; // Ÿ�̸Ӹ� ������ �������� �ʱ�ȭ
            }
            soundTimer -= Time.deltaTime; // Ÿ�̸� ����
        }
        else
        {
            soundTimer = 0f; // �̵��� ���߸� Ÿ�̸� ����
        }

        wasMovingLastFrame = isMoving; // ���� �������� �̵� ���� ����
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