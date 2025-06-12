using UnityEngine;
public class Seat : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    public void Reserve()
    {
        IsOccupied = true;
    }

    public void Vacate()
    {
        IsOccupied = false;
        SeatManager.Instance.TrySeatWaitingCustomers();
    }

    public Transform SitPoint; // 앉는 위치

    //private bool isSeated = false;

   
}
