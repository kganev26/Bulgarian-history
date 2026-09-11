using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public UnitType type;
    
    [Header("Economy")]
    public int goldCost = 20;      // Цена за покупка
    public int goldReward = 10;    // Награда при убиване на тази единица

    [Header("Stats")]
    public float maxHealth = 100f;
    public float attackDamage = 15f;
    public float attackRange = 2f;
    public float attackSpeed = 1.5f;
    public float moveSpeed = 5f;
}