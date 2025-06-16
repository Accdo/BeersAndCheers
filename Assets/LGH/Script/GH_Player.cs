using UnityEngine;

public class GH_Player : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeedX = 180f; // 회전속도
    private Rigidbody rigid;

    public InventoryManager inventory;
    

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        //Cursor.visible = false;
    }

    private void Update()
    {
        PlayerControl();
    }

    private void PlayerControl()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 velocity = new Vector3(inputX, 0, inputZ);
        velocity *= speed;
        rigid.linearVelocity = velocity;

        float mouseMoveX = Input.GetAxis("Mouse X");
        transform.Rotate(0, mouseMoveX * rotateSpeedX * Time.deltaTime, 0);
    }
}
