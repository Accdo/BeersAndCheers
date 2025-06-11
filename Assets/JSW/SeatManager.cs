using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager Instance;

    private List<Seat> allSeats = new List<Seat>();
    public List<SeatGroup> allSeatGroups = new List<SeatGroup>();
    public List<Transform> queuePoints = new List<Transform>(); // 대기 위치들
    public List<CustomerAI> waitingQueue = new List<CustomerAI>();
    public Transform exitPoint;

    private void Awake()
    {
        Instance = this;
        allSeats = FindObjectsOfType<Seat>().ToList();
        allSeatGroups = FindObjectsOfType<SeatGroup>().ToList();
    }
    void Start()
    {
    }

    // 팀 단위로 빈 테이블(SeatGroup) 반환
    public SeatGroup GetAvailableSeatGroup(int teamSize)
    {
        foreach (var group in allSeatGroups)
            if (group.IsAvailable(teamSize))
                return group;
        return null;
    }

    // 대기열 추가
    public int AddToQueue(CustomerAI customer)
    {
        waitingQueue.Add(customer);
        TrySeatWaitingCustomers();
        return waitingQueue.Count - 1;
    }

    // 대기열에서 좌석 생기면 자리로 이동
    public void TrySeatWaitingCustomers()
    {
        for (int i = 0; i < waitingQueue.Count; i++)
    {
        var customer = waitingQueue[i];
        var group = GetAvailableSeatGroup(customer.teamSize);
        if (group != null)
        {
            var seats = group.ReserveSeats(customer.teamSize);
            if (seats.Count > 0)
            {
                customer.AssignSeats(seats);
                waitingQueue.RemoveAt(i);
                i--;
            }
            else
            {
            }
        }
    }
    }
}