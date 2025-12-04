using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthFillImage;

    void Start()
    {
        // Автопоиск компонентов если не назначены
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (healthFillImage == null)
        {
            healthFillImage = GetComponent<Image>();
        }

        // Проверяем что Image имеет тип Filled
        if (healthFillImage != null && healthFillImage.type != Image.Type.Filled)
        {
            healthFillImage.type = Image.Type.Filled;
            healthFillImage.fillMethod = Image.FillMethod.Horizontal;
        }

        // Инициализируем HP бар
        UpdateHealthBar(playerHealth.GetCurrentHealth());

        // Подписываемся на событие изменения здоровья
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
        }
    }

    void OnDestroy()
    {
        // Отписываемся при уничтожении
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }

    // Метод обновления HP бара
    private void UpdateHealthBar(int currentHealth)
    {
        if (healthFillImage != null && playerHealth != null)
        {
            // Вычисляем заполнение (от 0 до 1)
            float fillAmount = (float)currentHealth / playerHealth.GetMaxHealth();
            healthFillImage.fillAmount = fillAmount;
        }
    }
}
