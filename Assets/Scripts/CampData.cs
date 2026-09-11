using UnityEngine;

[CreateAssetMenu(fileName = "NewCampData", menuName = "ScriptableObjects/CampData")]
public class CampData : ScriptableObject
{
    public string campName = "Military Camp";
    public GameObject unitPrefab;      // Prefab на единицата, която ще се ражда (напр. PF_Byzantine_Infantry)
    public UnitData unitData;          // Характеристики на единицата (за цена, име и т.н.)
    public float trainingTime = 5.0f;  // Време за обучение в секунди
    public int maxQueueSize = 5;       // Максимален брой войници в опашката за обучение
}