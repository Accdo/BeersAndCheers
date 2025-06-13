using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region 상수
    private const float GROUND_CHECK_LEEWAY = 0.1f;
    #endregion
    [SerializeField, Tooltip("기본값 100")] private float maxHealth;
    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = value;
            // EventManager.Instance.TriggerEvent("PlayerHealthChanged", currentHealth);
            // ui필요함
            if (currentHealth <= 0)
            {
                // EventManager.Instance.TriggerEvent("PlayerDied");
                // ui필요함
            }
        }
    }

    [Header("이동 속도")]
    [SerializeField, Tooltip("기본값 5")] private float walkSpeed;
    [SerializeField, Tooltip("기본값 15")] private float runSpeed;
    private float appliedSpeed;

    [Header("점프력")]
    [SerializeField, Tooltip("기본값 4")] private float jumpForce;

    #region 동작 상태
    private bool isGrounded;
    private bool isRun;
    private bool doingOtherWork;
    Coroutine currentOtherWork;
    [HideInInspector] public bool IsRun => isRun;
    #endregion

    [Header("카메라")]
    [SerializeField, Tooltip("기본값 2")] private float camSensitivityVertical;
    [SerializeField, Tooltip("기본값 3")] private float camSensitivityHorizontal;
    [SerializeField, Tooltip("기본값 45")] private float camRotationLimit;
    private float currentCamRotationX;

    #region 컴포넌트
    private Rigidbody rb;
    private CapsuleCollider coll;
    private Camera cam;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        cam = GetComponentInChildren<Camera>();

        SetDefaultValues();
    }

    private void SetDefaultValues()
    {
        appliedSpeed = walkSpeed;
        currentCamRotationX = 0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        RotateCam();
        RotatePlayer();
        CheckGround();
        TryJump();
        TryRun();
        Move();
    }

    private void RotateCam() // 상하
    {
        if (doingOtherWork) { return; }
        float rotationX = Input.GetAxisRaw("Mouse Y");

        float camRotX = rotationX * camSensitivityVertical;
        currentCamRotationX -= camRotX;
        currentCamRotationX = Mathf.Clamp(currentCamRotationX, -camRotationLimit, camRotationLimit);

        cam.transform.localEulerAngles = new Vector3(currentCamRotationX, 0f, 0f);
    }

    private void RotatePlayer() // 좌우
    {
        if (doingOtherWork) { return; }
        float rotationY = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, rotationY, 0f) * camSensitivityHorizontal;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, coll.bounds.extents.y + GROUND_CHECK_LEEWAY);
    }

    private void TryJump()
    {
        if (doingOtherWork) { return; }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = transform.up * jumpForce;
        }
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !doingOtherWork) // 공중에서도 대쉬속도 나오게 할지말지
        {
            isRun = true;
            appliedSpeed = runSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun = false;
            appliedSpeed = walkSpeed;
        }
    }

    private void Move()
    {
        if (doingOtherWork) { return; }

        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveX = transform.right * moveDirX;
        Vector3 moveZ = transform.forward * moveDirZ;

        Vector3 moveVel = (moveX + moveZ).normalized * appliedSpeed;

        rb.linearVelocity = new Vector3(moveVel.x, rb.linearVelocity.y, moveVel.z);
    }


    #region 다른일 컨트롤 (움직임 및 마우스 제어)

    public void StartOtherWorkForTime(float time = 0f)
    {
        if (currentOtherWork != null)
        {
            StopCoroutine(currentOtherWork);
        }

        currentOtherWork = StartCoroutine(nameof(OtherWorkCoroutine), time);
    }

    IEnumerator OtherWorkCoroutine(float time)
    {
        doingOtherWork = true;

        yield return new WaitForSeconds(time);

        doingOtherWork = false;
        currentOtherWork = null;
    }

    public void StartOtherWork()
    {
        doingOtherWork = true;
    }

    public void EndOtherWork()
    {
        if (currentOtherWork != null)
        {
            StopCoroutine(currentOtherWork);
        }
        doingOtherWork = false;
        currentOtherWork = null;
    }

    #endregion
}
