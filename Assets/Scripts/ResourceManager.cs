using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Economy Settings")]
    public int startingGold = 100;
    private int currentGold;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI goldText;

    public int CurrentGold => currentGold;

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

        currentGold = startingGold;
        UpdateUI();
    }

    public bool HasEnoughGold(int amount)
    {
        return currentGold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (HasEnoughGold(amount))
        {
            currentGold -= amount;
            UpdateUI();
            return true;
        }
        Debug.LogWarning("Нямате достатъчно жълтици!");
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Жълтици: {currentGold}";
        }
    }
}