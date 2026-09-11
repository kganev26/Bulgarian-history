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

    [Header("Action Buttons")]
    [SerializeField] private Button trainHorseArcherButton;
    [SerializeField] private Button trainHorsemanButton;
    [SerializeField] private Button closeButton;

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

            // Настройване на бутона за Horse Archer
            if (trainHorseArcherButton != null)
            {
                trainHorseArcherButton.onClick.RemoveAllListeners();
                trainHorseArcherButton.onClick.AddListener(() => OnTrainUnitClicked(UnitType.HorseArcher));
            }

            // Настройване на бутона за Horseman
            if (trainHorsemanButton != null)
            {
                trainHorsemanButton.onClick.RemoveAllListeners();
                trainHorsemanButton.onClick.AddListener(() => OnTrainUnitClicked(UnitType.Horseman));
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

    public void CloseMenu()
    {
        DeselectCamp();
    }

    private void OnTrainUnitClicked(UnitType type)
    {
        if (selectedCamp != null)
        {
            selectedCamp.QueueUnit(type);
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