using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Datas/AnimalData")]
public class AnimalData : ScriptableObject
{
    [SerializeField] private string animalName;
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<GameObject> dropItems;

    public string AnimalName => animalName;
    public float Health => health;
    public float MoveSpeed => moveSpeed;
    public List<GameObject> DropItems => dropItems;
}