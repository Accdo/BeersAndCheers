using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float jumpForce = 5f; // ���� ��
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on player!");
        }
    }

    void Update()
    {
        // �̵� �Է�
        float moveX = Input.GetAxisRaw("Horizontal"); // �¿� �̵� (A, D �Ǵ� ȭ��ǥ)
        float moveZ = Input.GetAxisRaw("Vertical");   // �յ� �̵� (W, S �Ǵ� ȭ��ǥ)

        // �̵� ���� ���
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y; // ���� Y �ӵ� ���� (�߷�/���� ����)

        // �̵� ����
        rb.linearVelocity = velocity;

        // ���� �Է�
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // �ٴ� üũ
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}