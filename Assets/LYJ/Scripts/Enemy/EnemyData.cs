using System.Collections.Generic;
using UnityEngine;

public enum FleshType { Hard, Soft }

[CreateAssetMenu(fileName = "EnemyData", menuName = "Datas/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField, Tooltip("공격모션 선딜레이")] private float attackFirstDelay;
    [SerializeField, Tooltip("1회 공격까지 필요한 시간")] private float attackTotalDelay;
    [SerializeField] private float attackPower;
    [SerializeField] private float attackRange;
    [SerializeField, Tooltip("공격 포인트 높이")] private float attackHeight;

    [SerializeField, Tooltip("육질")] private FleshType fleshType;
    [SerializeField] private List<GameObject> dropItems;
    [SerializeField, Tooltip("등장 층수(난이도)")] private Vector2 spawnZone;

    public string EnemyName => enemyName;
    public float Health => health;
    public float MoveSpeed => moveSpeed;
    public float AttackFirstDelay => attackFirstDelay;
    public float AttackTotalDelay => attackTotalDelay;
    public float AttackPower => attackPower;
    public float AttackRange => attackRange;
    public float AttackHeight => attackHeight;
    public FleshType FleshType => fleshType;
    public List<GameObject> DropItems => dropItems;
    public Vector2 SpawnZone => spawnZone;
}