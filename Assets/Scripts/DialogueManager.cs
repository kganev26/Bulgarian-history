using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Settings")]
    [SerializeField] private float defaultDuration = 5f;

    private Coroutine activeDialogueCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Показва съобщение от лидер с портрет и текст за определено време.
    /// </summary>
    public void ShowMessage(string speakerName, string message, Sprite portrait, float duration = 0f)
    {
        if (activeDialogueCoroutine != null)
        {
            StopCoroutine(activeDialogueCoroutine);
        }

        activeDialogueCoroutine = StartCoroutine(DisplayRoutine(speakerName, message, portrait, duration > 0 ? duration : defaultDuration));
    }

    private IEnumerator DisplayRoutine(string speakerName, string message, Sprite portrait, float duration)
    {
        dialoguePanel.SetActive(true);

        speakerNameText.text = speakerName;
        dialogueText.text = message;

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(duration);

        dialoguePanel.SetActive(false);
        activeDialogueCoroutine = null;
    }
}