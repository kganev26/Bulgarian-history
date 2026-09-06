using UnityEngine;

public enum UnitType { Infantry, Cavalry, Archers }

[CreateAssetMenu(fileName = "NewUnitData", menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public UnitType type;
    public float maxHealth = 100f;
    public float attackDamage = 15f;
    public float attackRange = 2f;
    public float attackSpeed = 1.5f; // Секунди между атаките
    public float moveSpeed = 5f;
}