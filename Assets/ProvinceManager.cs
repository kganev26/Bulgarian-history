using System.Collections.Generic;
using UnityEngine;

public class ProvinceManager : MonoBehaviour
{
    public static ProvinceManager Instance { get; private set; }

    [Header("Faction Units Trace")]
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    private void Awake()
    {
        // Настройка на Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // --- Тук продължава вашият оригинален код в Awake (ако има такъв) ---
    }

    // Регистрация на единица при появяване
    public void RegisterUnit(Unit unit)
    {
        if (unit.factionId == 1 && !playerUnits.Contains(unit))
        {
            playerUnits.Add(unit);
        }
        else if (unit.factionId == 2 && !enemyUnits.Contains(unit))
        {
            enemyUnits.Add(unit);
        }
    }

    // Премахване на единица при премахване/смърт
    public void UnregisterUnit(Unit unit)
    {
        if (unit.factionId == 1)
        {
            playerUnits.Remove(unit);
        }
        else if (unit.factionId == 2)
        {
            enemyUnits.Remove(unit);
        }
    }
}