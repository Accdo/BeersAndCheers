using UnityEngine;

public class GH_Player : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeedX = 180f; // 회전속도
    private Rigidbody rigid;

    public InventoryManager inventory;

    // 현재 착용 장비
    public GameObject currentEquipment;
    public Transform weaponHoldPoint; // 무기를 장착할 위치

    // 시작 무기 아이템
    public Item swordItem;
    public Item exeItem;

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();

    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        //Cursor.visible = false;

        inventory.Add("Hotbar", swordItem);
        inventory.Add("Hotbar", exeItem);
    }

    private void Update()
    {
        PlayerControl();

        //inventory.hotbar.selectedSlot
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

    public void EquipWeapon()
    {
        Debug.Log("EquipWeapon called");
        if (currentEquipment != null)
        {
            Destroy(currentEquipment);
        }
        if(inventory.hotbar.selectedSlot.UseItem() == null)
        {
            return;
        }
        currentEquipment = Instantiate(inventory.hotbar.selectedSlot.UseItem(), weaponHoldPoint.position, transform.rotation);
        currentEquipment.transform.SetParent(transform);
    }
}
