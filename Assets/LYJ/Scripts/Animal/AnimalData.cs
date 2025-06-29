using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Datas/AnimalData")]
public class AnimalData : ScriptableObject
{
    [SerializeField] private string animalName;
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Item dropItem;
    [SerializeField] private int indexOnPool;

    public string AnimalName => animalName;
    public float Health => health;
    public float MoveSpeed => moveSpeed;
    public Item DropItem => dropItem;
    public int IndexOnPool => indexOnPool;
}