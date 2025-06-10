using System.Collections.Generic;
using UnityEngine;

public class SeatGroup : MonoBehaviour
{
    public List<Seat> seats;

    public bool IsAvailable(int teamSize)
    {
        int available = 0;
        foreach (var seat in seats)
            if (!seat.IsOccupied) available++;
        return available >= teamSize;
    }

    public List<Seat> ReserveSeats(int teamSize)
    {
        List<Seat> reserved = new List<Seat>();
        foreach (var seat in seats)
        {
            if (!seat.IsOccupied)
            {
                seat.Reserve();
                reserved.Add(seat);
                if (reserved.Count == teamSize) break;
            }
        }
        return reserved;
    }
}