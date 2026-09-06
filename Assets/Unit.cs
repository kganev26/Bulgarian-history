using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("Data & Faction")]
    public UnitData data;
    public int factionId;

    [Header("Visual Selection & UI")]
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private HealthBarUI healthBar;

    [Header("Targeting Limits")]
    public int maxTargetingUnits = 2; // Максимален брой нападатели върху тази единица
    private List<MonoBehaviour> currentAttackers = new List<MonoBehaviour>();

    private float currentHealth;
    private NavMeshAgent agent;
    private Unit targetEnemy;
    private float nextAttackTime;

    public bool IsSelected => selectionIndicator != null && selectionIndicator.activeSelf;

    public bool CanBeTargeted()
    {
        currentAttackers.RemoveAll(attacker => attacker == null);
        return currentAttackers.Count < maxTargetingUnits;
    }

    public bool RegisterAttacker(MonoBehaviour attacker)
    {
        currentAttackers.RemoveAll(a => a == null);
        if (currentAttackers.Count < maxTargetingUnits)
        {
            if (!currentAttackers.Contains(attacker))
            {
                currentAttackers.Add(attacker);
            }
            return true;
        }
        return false;
    }

    public void UnregisterAttacker(MonoBehaviour attacker)
    {
        if (currentAttackers.Contains(attacker))
        {
            currentAttackers.Remove(attacker);
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (data != null)
        {
            currentHealth = data.maxHealth;
            agent.speed = data.moveSpeed;
            
            if (healthBar != null)
            {
                healthBar.UpdateHealth(currentHealth, data.maxHealth);
            }
        }

        SetSelected(false);

        if (ProvinceManager.Instance != null)
        {
            ProvinceManager.Instance.RegisterUnit(this);
        }
    }

    private void Update()
    {
        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance <= data.attackRange)
            {
                agent.ResetPath();
                if (Time.time >= nextAttackTime)
                {
                    AttackTarget();
                    nextAttackTime = Time.time + data.attackSpeed;
                }
            }
            else
            {
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
    }

    public void MoveTo(Vector3 destination)
    {
        targetEnemy = null;
        agent.SetDestination(destination);
    }

    public void AttackOrder(Unit enemy)
    {
        if (enemy != null && enemy.factionId != this.factionId)
        {
            targetEnemy = enemy;
        }
    }

    private void AttackTarget()
    {
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(data.attackDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (healthBar != null && data != null)
        {
            healthBar.UpdateHealth(currentHealth, data.maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(isSelected);
        }
    }

    private void OnDestroy()
    {
        if (ProvinceManager.Instance != null)
        {
            ProvinceManager.Instance.UnregisterUnit(this);
        }
    }
}