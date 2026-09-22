using UnityEngine;

public class ByzantineSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public MilitaryCamp byzantineCamp;
    public float waveInterval = 12f;
    public int unitsPerWave = 3;

    private float timer;

    private void Update()
    {
        if (byzantineCamp == null) return;

        timer += Time.deltaTime;
        if (timer >= waveInterval)
        {
            timer = 0f;
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < unitsPerWave; i++)
        {
            UnitType typeToSpawn = (Random.value > 0.5f) ? UnitType.Horseman : UnitType.HorseArcher;
            byzantineCamp.QueueUnit(typeToSpawn);
        }

        // Задействане на историческия диалог на Константин IV
        BattleEvents events = FindFirstObjectByType<BattleEvents>();
        if (events != null)
        {
            events.TriggerByzantineAttackDialogue();
        }
    }
}