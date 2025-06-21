using UnityEngine;
public class Seat : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;
    public Transform SitPoint; // 앉는 위치

    private void Awake()
    {
        if (SitPoint == null)
        {
            SitPoint = transform; // 기본적으로 자신의 위치를 사용
        }
    }
    public void Reserve()
    {
        IsOccupied = true;
    }

    public void Vacate()
    {
        IsOccupied = false;
        SeatManager.Instance.TrySeatWaitingCustomers();
    }

    //private bool isSeated = false;

}
