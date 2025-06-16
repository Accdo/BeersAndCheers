using System.Collections.Generic;
using UnityEngine;

public enum FleshType { Hard, Soft }

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField, Tooltip("1회 공격까지 필요한 시간")] private float attackDelay;
    [SerializeField] private float attackPower;
    [SerializeField] private float attackRange;
    [SerializeField, Tooltip("육질")] private FleshType fleshType;
    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private List<GameObject> dropItems;
    [SerializeField, Tooltip("등장 층수(난이도)")] private int spawnZone;

    public string EnemyName => enemyName;
    public float Health => health;
    public float MoveSpeed => moveSpeed;
    public float AttackDelay => attackDelay;
    public float AttackPower => attackPower;
    public float AttackRange => attackRange;
    public FleshType FleshType => fleshType;
    public RuntimeAnimatorController Anim => anim;
    public List<GameObject> DropItems => dropItems;
    public int SpawnZone => spawnZone;
}