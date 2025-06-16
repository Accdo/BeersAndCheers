using UnityEngine;

public class State_LYJ<T> : MonoBehaviour
{
    public virtual void Enter(T owner) { }
    public virtual void Run() { }
    public virtual void Exit() { }
}
