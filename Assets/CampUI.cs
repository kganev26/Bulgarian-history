using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CampUI : MonoBehaviour
{
    public static CampUI Instance { get; private set; }

    [Header("UI Panels & Elements")]
    [SerializeField] private GameObject campPanel;
    [SerializeField] private TextMeshProUGUI campNameText;
    [SerializeField] private TextMeshProUGUI queueText;
    [SerializeField] private TextMeshProUGUI popLimitText;
    [SerializeField] private Image progressBar;
    [SerializeField] private Button trainButton;
    [SerializeField] private Button closeButton; // Нов бутон за затваряне

    private MilitaryCamp selectedCamp;

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

        if (campPanel != null)
        {
            campPanel.SetActive(false);
        }

        // Закачаме функцията за затваряне към closeButton
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseMenu);
        }
    }

    private void Update()
    {
        if (selectedCamp != null && campPanel.activeSelf)
        {
            UpdateUI();
        }
    }

    public void SelectCamp(MilitaryCamp camp)
    {
        selectedCamp = camp;

        if (selectedCamp != null)
        {
            campPanel.SetActive(true);

            if (campNameText != null && selectedCamp.campData != null)
            {
                campNameText.text = selectedCamp.campData.campName;
            }

            if (trainButton != null)
            {
                trainButton.onClick.RemoveAllListeners();
                trainButton.onClick.AddListener(OnTrainButtonClicked);
            }

            UpdateUI();
        }
    }

    public void DeselectCamp()
    {
        selectedCamp = null;
        if (campPanel != null)
        {
            campPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Метод за публично/ръчно затваряне на менюто
    /// </summary>
    public void CloseMenu()
    {
        DeselectCamp();
    }

    private void OnTrainButtonClicked()
    {
        if (selectedCamp != null)
        {
            selectedCamp.QueueUnit();
        }
    }

    private void UpdateUI()
    {
        if (selectedCamp == null) return;

        if (progressBar != null)
        {
            progressBar.fillAmount = selectedCamp.CurrentProgress;
        }

        if (queueText != null)
        {
            queueText.text = $"Опашка: {selectedCamp.QueueCount} / {(selectedCamp.campData != null ? selectedCamp.campData.maxQueueSize : 5)}";
        }

        if (popLimitText != null && ProvinceManager.Instance != null)
        {
            int currentPop = selectedCamp.factionId == 1 ? ProvinceManager.Instance.playerUnits.Count : ProvinceManager.Instance.enemyUnits.Count;
            int maxPop = selectedCamp.factionId == 1 ? ProvinceManager.Instance.maxPlayerUnits : ProvinceManager.Instance.maxEnemyUnits;

            popLimitText.text = $"Войници: {currentPop} / {maxPop}";
        }
    }
}