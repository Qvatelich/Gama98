using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Настройки здоровья")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("События")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    public UnityEvent<int> OnHeal; // Изменено: теперь передает количество лечения
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnFullHealth; // Новое событие при попытке лечения при полном здоровье

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Вызываем события
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke();

        Debug.Log($"Игрок получил {damage} урона. Осталось здоровья: {currentHealth}");

        // Проверяем смерть
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool Heal(int healAmount)
    {
        // Проверяем, можно ли лечиться с этим количеством лечения
        if (!CanHeal(healAmount))
        {
            Debug.Log($"Не могу вылечиться на {healAmount}: недостаточно места для лечения");

            // Если здоровье уже полное, вызываем специальное событие
            if (IsHealthFull())
            {
                OnFullHealth?.Invoke();
            }
            return false; // Лечение не удалось
        }

        int healthBeforeHeal = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        int actualHealAmount = currentHealth - healthBeforeHeal;

        // Вызываем события
        OnHealthChanged?.Invoke(currentHealth);
        OnHeal?.Invoke(actualHealAmount);

        Debug.Log($"Игрок вылечен на {actualHealAmount}. Текущее здоровье: {currentHealth}");
        return true; // Лечение успешно
    }

    // Метод для проверки, полное ли здоровье
    public bool IsHealthFull()
    {
        return currentHealth >= maxHealth;
    }

    // Метод для проверки можно ли лечиться на определенное количество
    public bool CanHeal(int healAmount = 1)
    {
        // Если здоровье уже максимальное
        if (currentHealth >= maxHealth)
            return false;

        // Если передано конкретное количество - проверяем, поместится ли оно
        if (healAmount > 0)
        {
            return currentHealth + healAmount <= maxHealth;
        }

        // Если количество не указано - проверяем, есть ли вообще место для лечения
        return currentHealth < maxHealth;
    }

    // Метод для получения максимально возможного лечения
    public int GetMaxPossibleHeal(int desiredHealAmount)
    {
        int missingHealth = maxHealth - currentHealth;
        return Mathf.Min(desiredHealAmount, missingHealth);
    }

    // Метод для безопасного лечения (возвращает фактическое количество лечения)
    public int SafeHeal(int healAmount)
    {
        if (!CanHeal(healAmount))
        {
            int possibleHeal = GetMaxPossibleHeal(healAmount);
            if (possibleHeal > 0)
            {
                return HealWithReturn(possibleHeal);
            }
            return 0;
        }

        return HealWithReturn(healAmount);
    }

    // Вспомогательный метод лечения с возвратом количества
    private int HealWithReturn(int healAmount)
    {
        int healthBefore = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        int actualHeal = currentHealth - healthBefore;

        OnHealthChanged?.Invoke(currentHealth);
        OnHeal?.Invoke(actualHeal);

        return actualHeal;
    }

    private void Die()
    {
        Debug.Log("Игрок умер!");
        OnDeath?.Invoke();

        // Здесь можно добавить логику смерти:
        // Time.timeScale = 0; // Остановка игры
        // Включение экрана смерти и т.д.
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public int GetMissingHealth() => maxHealth - currentHealth;
}