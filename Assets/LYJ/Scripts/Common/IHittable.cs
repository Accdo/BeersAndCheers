using UnityEngine;

public interface IHittable
{
    public abstract void Damage(float damageAmount);
    // 상속해서 더 깔끔하게 만드려면 시간이 너무많이 들거같네요............................... 
}
