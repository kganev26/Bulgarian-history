using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceManager : MonoBehaviour
{
    public static ProvinceManager Instance { get; private set; }

    [Header("Population Limits")]
    public int maxPlayerUnits = 20;
    public int maxEnemyUnits = 20;

    [Header("Faction Units Trace")]
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Проверява дали фракцията е достигнала лимита за население
    /// </summary>
    public bool CanSpawnUnit(int factionId)
    {
        if (factionId == 1) // Играч / Българи
        {
            return playerUnits.Count < maxPlayerUnits;
        }
        else if (factionId == 2 || factionId == 0) // Враг / Византия (поддържа 0 и 2)
        {
            return enemyUnits.Count < maxEnemyUnits;
        }

        return false;
    }

    // Регистрация на единица при появяване
    public void RegisterUnit(Unit unit)
    {
        if (unit.factionId == 1 && !playerUnits.Contains(unit))
        {
            playerUnits.Add(unit);
            Debug.Log($"[ProvinceManager] Български войник добавен. Общо: {playerUnits.Count}/{maxPlayerUnits}");
        }
        else if ((unit.factionId == 2 || unit.factionId == 0) && !enemyUnits.Contains(unit))
        {
            enemyUnits.Add(unit);
            Debug.Log($"[ProvinceManager] Византийски войник добавен. Общо: {enemyUnits.Count}/{maxEnemyUnits}");
        }
    }

    // Премахване на единица при премахване/смърт
    public void UnregisterUnit(Unit unit)
    {
        if (unit.factionId == 1)
        {
            playerUnits.Remove(unit);
            Debug.Log($"[ProvinceManager] Български войник загина. Общо остават: {playerUnits.Count}/{maxPlayerUnits}");
        }
        else if (unit.factionId == 2 || unit.factionId == 0)
        {
            enemyUnits.Remove(unit);
            Debug.Log($"[ProvinceManager] Византийски войник загина. Общо остават: {enemyUnits.Count}/{maxEnemyUnits}");
        }
    }
}