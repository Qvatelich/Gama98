using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    [Header("Настройки урона")]
    [SerializeField] private int damageAmount = 2; // Количество урона
    [SerializeField] private bool destroyOnCollision = false; // Уничтожаться ли при столкновении

    [Header("Настройки триггера")]
    [SerializeField] private bool useTrigger = true; // Использовать триггер вместо коллизии

    [Header("Эффекты")]
    [SerializeField] private GameObject hitEffect; // Эффект при столкновении
    [SerializeField] private AudioClip hitSound; // Звук при столкновении

    private void OnCollisionEnter(Collision collision)
    {
        if (!useTrigger)
        {
            ProcessCollision(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger)
        {
            ProcessCollision(other.gameObject);
        }
    }

    private void ProcessCollision(GameObject target)
    {
        // Проверяем, является ли объект игроком
        if (target.CompareTag("Player"))
        {
            // Получаем компонент здоровья игрока
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Наносим урон
                playerHealth.TakeDamage(damageAmount);

                // Воспроизводим эффекты
                PlayEffects();

                // Уничтожаем объект если нужно
                if (destroyOnCollision)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void PlayEffects()
    {
        // Воспроизводим звук
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // Создаем эффект
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
}
