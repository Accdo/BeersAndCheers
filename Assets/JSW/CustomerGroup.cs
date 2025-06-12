using System.Collections.Generic;
using UnityEngine;

public class CustomerGroup
{
    public List<CustomerAI> members;
    public int teamSize => members.Count;
    public SeatGroup assignedSeatGroup;
    // 기타 그룹 정보(주문, 만족도 등)
}