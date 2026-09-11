using UnityEngine;

public class BattleEvents : MonoBehaviour
{
    [Header("Portraits")]
    public Sprite asparuhPortrait;
    public Sprite constantinePortrait;

    private void Start()
    {
        // Съобщение в началото на битката
        Invoke(nameof(TriggerStartDialogue), 2f);
    }

    private void TriggerStartDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowMessage(
                "Хан Аспарух",
                "Братя българи! Врагът пристига по море и суша. Вкрепете лагера при Онгъла и не отстъпвайте нито педя земя!",
                asparuhPortrait,
                6f
            );
        }
    }

    // Пример: Може да повикате този метод, когато Византийците пуснат голяма вълна
    public void TriggerByzantineAttackDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowMessage(
                "Император Константин IV",
                "Разгромете варварите! Империята няма да търпи натрапници южно от Дунава!",
                constantinePortrait,
                5f
            );
        }
    }
}