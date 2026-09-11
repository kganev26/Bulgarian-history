using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    HorseArcher,
    Horseman
}

public class MilitaryCamp : MonoBehaviour
{
    [Header("Camp Settings")]
    public int factionId = 1; // 1 = Bulgarian, 2 = Byzantine
    public CampData campData;

    [Header("Unit Prefabs")]
    public GameObject horseArcherPrefab;
    public GameObject horsemanPrefab;

    [Header("Spawn Configuration")]
    public Transform spawnPoint;

    // Опашка за изчакване
    private Queue<UnitType> trainingQueue = new Queue<UnitType>();
    private bool isTraining = false;
    private float currentProgress = 0f;

    public int QueueCount => trainingQueue.Count;
    public float CurrentProgress => currentProgress;

    private void OnMouseDown()
    {
        // Отваря менюто само за българския лагер (factionId == 1)
        if (factionId == 1 && CampUI.Instance != null)
        {
            CampUI.Instance.SelectCamp(this);
        }
    }

    /// <summary>
    /// Добавя избран тип единица в опашката за трениране
    /// </summary>
    // В MilitaryCamp.cs (вътре в QueueUnit)
public void QueueUnit(UnitType type)
{
    int maxQueue = campData != null ? campData.maxQueueSize : 5;

    if (trainingQueue.Count >= maxQueue)
    {
        Debug.LogWarning("Опашката за трениране е пълна!");
        return;
    }

    // Вземаме цената на единицата
    GameObject targetPrefab = (type == UnitType.HorseArcher) ? horseArcherPrefab : horsemanPrefab;
    Unit unitComponent = targetPrefab != null ? targetPrefab.GetComponent<Unit>() : null;
// Правилен вариант:
int cost = (unitComponent != null && unitComponent.data != null) ? unitComponent.data.goldCost : 0;
    // 1. Проверка за жълтици (само за играча / faction 1)
    if (factionId == 1)
    {
        if (ResourceManager.Instance != null && !ResourceManager.Instance.HasEnoughGold(cost))
        {
            Debug.LogWarning("Нямате достатъчно жълтици за тази единица!");
            return;
        }

        // Вземаме парите веднага при слагане в опашката
        ResourceManager.Instance.SpendGold(cost);
    }

    // 2. Проверка за лимит на населението
    if (ProvinceManager.Instance != null)
    {
        int currentPop = factionId == 1 ? ProvinceManager.Instance.playerUnits.Count : ProvinceManager.Instance.enemyUnits.Count;
        int maxPop = factionId == 1 ? ProvinceManager.Instance.maxPlayerUnits : ProvinceManager.Instance.maxEnemyUnits;

        if (currentPop + trainingQueue.Count >= maxPop)
        {
            Debug.LogWarning("Лимитът на населението е достигнат!");
            // Връщаме парите, ако лимитът е надвишен
            if (factionId == 1 && ResourceManager.Instance != null)
                ResourceManager.Instance.AddGold(cost);
            return;
        }
    }

    trainingQueue.Enqueue(type);

    if (!isTraining)
    {
        StartCoroutine(TrainNextUnit());
    }
}

    private IEnumerator TrainNextUnit()
    {
        isTraining = true;

        while (trainingQueue.Count > 0)
        {
            UnitType currentUnitType = trainingQueue.Peek();
            float timeToTrain = campData != null ? campData.trainingTime : 3f;
            float timer = 0f;

            while (timer < timeToTrain)
            {
                timer += Time.deltaTime;
                currentProgress = timer / timeToTrain;
                yield return null;
            }

            // Завършено обучение - премахваме от опашката и раждаме единицата
            trainingQueue.Dequeue();
            SpawnUnit(currentUnitType);
            currentProgress = 0f;
        }

        isTraining = false;
    }

    private void SpawnUnit(UnitType type)
    {
        GameObject prefabToSpawn = type == UnitType.HorseArcher ? horseArcherPrefab : horsemanPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogError($"Липсва Prefab за тип {type}!");
            return;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject spawnedObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        Unit unitComponent = spawnedObj.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.factionId = this.factionId;
        }

        if (ProvinceManager.Instance != null)
        {
            ProvinceManager.Instance.RegisterUnit(unitComponent);
        }

        Debug.Log($"Успешно обучен: {type}");
    }
}