using UnityEngine;

public enum AttackType { Slash, Smash, Nothing }

[CreateAssetMenu(fileName = "Hand", menuName = "Datas/Hand")]
public class Hand : ScriptableObject
{
    [SerializeField] private string handName;
    [SerializeField, Tooltip("Slash : 참격\nSmash : 타격\nNothing : 고정딜 or 기타 아이템")] private AttackType attackType;
    [SerializeField, Tooltip("초당 공격횟수")] private float attackSpeed;
    [SerializeField] private float damage;
    [SerializeField, Tooltip("공격 범위의 중심점")] private float distance;
    [SerializeField] private float radius;
    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private Sprite icon;

    public string HandName => handName;
    public AttackType AttackType => attackType;
    public float AttackSpeed => attackSpeed;
    public float Damage => damage;
    public float Distance => distance;
    public float Radius => radius;
    public RuntimeAnimatorController Anim => anim;
}
