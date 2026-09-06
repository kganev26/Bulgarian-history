using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float smoothSpeed = 10f; // Скорост на движение на скалата

    private Camera mainCamera;
    private float targetValue;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // Камерата продължава да следи ориентацията
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }

        // Анимираме движението на Slider-а до новата стойност
        if (slider != null && Mathf.Abs(slider.value - targetValue) > 0.01f)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            targetValue = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Ако е при стартиране на играта, задаваме стойността веднага
            if (slider.value == 0 && currentHealth > 0)
            {
                slider.value = currentHealth;
            }
        }
    }
}