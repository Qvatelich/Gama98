using UnityEngine;

public class ItemColl : MonoBehaviour
{
    [Header("Настройки предмета")]
    [SerializeField] private string itemID = "item_01"; // Уникальный ID предмета
    [SerializeField] private string itemName = "Предмет"; // Название для отображения

    [Header("Эффекты")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;

    [Header("Настройки сбора")]
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private bool canBeCollected = true;

    private void OnTriggerEnter(Collider other)
    {
        if (canBeCollected && other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canBeCollected && other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }
    }

    private void CollectItem(GameObject player)
    {
        // Получаем инвентарь игрока
        PlayerItemCollector collector = player.GetComponent<PlayerItemCollector>();

        if (collector != null)
        {
            // Добавляем предмет в инвентарь
            if (collector.AddItem(itemID, itemName))
            {
                // Эффекты
                PlayPickupEffects();

                // Сообщение в консоль
                Debug.Log($"Подобран предмет: {itemName} (ID: {itemID})");

                // Уничтожаем объект если нужно
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                else
                {
                    canBeCollected = false;
                    gameObject.SetActive(false); // Или делаем невидимым
                }
            }
        }
    }

    private void PlayPickupEffects()
    {
        // Звук
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Визуальный эффект
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }
    }

    // Метод для ручного сбора (например, по нажатию кнопки)
    public void ManualCollect(GameObject player)
    {
        if (canBeCollected)
        {
            CollectItem(player);
        }
    }
}