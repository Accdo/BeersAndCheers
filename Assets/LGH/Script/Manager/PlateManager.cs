using System.Collections.Generic;
using UnityEngine;

public class PlateManager : MonoBehaviour
{
    public static PlateManager instance;

    public Stack<GameObject> plateStack;
    public GameObject platePrefab; // 프리팹을 할당
    public int maxPlateCount = 10; // 최대 접시 개수

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void PushPlateStack()
    {
        plateStack.Push(Instantiate(platePrefab, transform.position, Quaternion.identity));
    }
}
