using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryCamp : MonoBehaviour
{
    [Header("Camp Configuration")]
    public CampData campData;
    public Transform spawnPoint;      // Точка, където се появяват новите войници
    public Transform rallyPoint;      // Точка (по избор), към която войниците тръгват след раждане

    [Header("Faction Settings")]
    public int factionId = 0;         // 0 или 2 за Византия, 1 за България

    [Header("Auto Train Settings")]
    public bool autoTrainEveryMinute = true; // Дали да тренира автоматично
    public float autoTrainInterval = 120f;   // Интервал в секунди

    [Header("Training State")]
    private Queue<CampData> trainingQueue = new Queue<CampData>();
    private float currentTrainingTimer = 0f;
    private bool isTraining = false;

    // Публични свойства за UI компонента
    public int QueueCount => trainingQueue.Count;
    public float CurrentProgress => isTraining && campData != null ? (currentTrainingTimer / campData.trainingTime) : 0f;

    void Start()
    {
        if (autoTrainEveryMinute)
        {
            InvokeRepeating(nameof(AutoQueueUnit), autoTrainInterval, autoTrainInterval);
        }
    }

    void Update()
    {
        HandleTrainingQueue();
    }

    /// <summary>
    /// Автоматично извикване
    /// </summary>
    private void AutoQueueUnit()
    {
        Debug.Log($"[Лагер {gameObject.name}] Измина интервалът. Заявка за нов войник...");
        QueueUnit();
    }

    /// <summary>
    /// Извиква се от бутон или от таймера, за да добави единица в опашката
    /// </summary>
    public bool QueueUnit()
    {
        if (campData == null)
        {
            Debug.LogWarning("CampData не е назначен на " + gameObject.name);
            return false;
        }

        // 1. Проверка за пълна опашка на лагера
        if (trainingQueue.Count >= campData.maxQueueSize)
        {
            Debug.Log($"[Лагер {gameObject.name}] Опашката за обучение е пълна!");
            return false;
        }

        // 2. Проверка за лимита на населението през ProvinceManager
        if (ProvinceManager.Instance != null && !ProvinceManager.Instance.CanSpawnUnit(factionId))
        {
            Debug.Log($"[Лагер {gameObject.name}] Лимитът на населението е достигнат! Пропускане на заявката.");
            return false;
        }

        trainingQueue.Enqueue(campData);
        Debug.Log($"Добавена единица в опашката на {gameObject.name}. Общо в опашка: {trainingQueue.Count}");

        if (!isTraining)
        {
            StartNextTraining();
        }

        return true;
    }

    private void HandleTrainingQueue()
    {
        if (!isTraining || trainingQueue.Count == 0) return;

        currentTrainingTimer += Time.deltaTime;

        if (currentTrainingTimer >= campData.trainingTime)
        {
            // Финална проверка за лимита преди раждане
            if (ProvinceManager.Instance == null || ProvinceManager.Instance.CanSpawnUnit(factionId))
            {
                SpawnUnit();
            }
            else
            {
                Debug.Log($"[Лагер {gameObject.name}] Лимитът е достигнат точно преди раждането! Обучението е отложено.");
            }

            trainingQueue.Dequeue();

            if (trainingQueue.Count > 0)
            {
                StartNextTraining();
            }
            else
            {
                isTraining = false;
                currentTrainingTimer = 0f;
            }
        }
    }

    private void StartNextTraining()
    {
        isTraining = true;
        currentTrainingTimer = 0f;
    }

    private void SpawnUnit()
    {
        if (campData == null || campData.unitPrefab == null) return;

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * 2f;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject newUnitObj = Instantiate(campData.unitPrefab, spawnPos, spawnRot);
        
        // Настройка на фракцията
        Unit unitScript = newUnitObj.GetComponent<Unit>();
        if (unitScript != null)
        {
            unitScript.factionId = this.factionId;
        }

        // Ако лагерът има Rally Point, изпращаме войника натам
        if (rallyPoint != null && unitScript != null)
        {
            unitScript.MoveTo(rallyPoint.position);
        }

        Debug.Log($"Войникът от тип {campData.campName} беше обучен успешно!");
    }

    void OnDisable()
    {
        CancelInvoke(nameof(AutoQueueUnit));
    }
    
    private void OnMouseDown()
    {
        // Отваря UI само за Българския лагер (factionId == 1)
        if (factionId == 1 && CampUI.Instance != null)
        {
            CampUI.Instance.SelectCamp(this);
        }
    }
}