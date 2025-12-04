using UnityEngine;
using System.Collections.Generic;

public class PlayerItemCollector : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private UIGiveItem uiIndicator;

    [Header("Настройки")]
    [SerializeField] private bool showDebugMessages = true;

    private List<string> collectedItems = new List<string>();

    void Start()
    {
        // Автопоиск UI индикатора если не назначен
        if (uiIndicator == null)
        {
            uiIndicator = FindObjectOfType<UIGiveItem>();
        }
    }

    // Добавить предмет в инвентарь
    public bool AddItem(string itemID, string itemName = "")
    {
        if (!collectedItems.Contains(itemID))
        {
            collectedItems.Add(itemID);

            // Обновляем UI индикатор
            if (uiIndicator != null)
            {
                uiIndicator.UpdateIndicator(itemID, true);
            }

            if (showDebugMessages)
            {
                if (!string.IsNullOrEmpty(itemName))
                {
                    Debug.Log($"Предмет добавлен: {itemName} (ID: {itemID})");
                }
                else
                {
                    Debug.Log($"Предмет добавлен: {itemID}");
                }
            }

            return true;
        }

        if (showDebugMessages)
        {
            Debug.Log($"Предмет {itemID} уже собран!");
        }

        return false;
    }

    // Проверить наличие предмета
    public bool HasItem(string itemID)
    {
        return collectedItems.Contains(itemID);
    }

    // Использовать предмет
    public bool UseItem(string itemID)
    {
        if (collectedItems.Contains(itemID))
        {
            collectedItems.Remove(itemID);

            // Обновляем UI (возвращаем к исходному цвету)
            if (uiIndicator != null)
            {
                uiIndicator.UpdateIndicator(itemID, false);
            }

            if (showDebugMessages)
            {
                Debug.Log($"Предмет использован: {itemID}");
            }

            return true;
        }

        return false;
    }

    // Получить список всех собранных предметов
    public List<string> GetAllItems()
    {
        return new List<string>(collectedItems);
    }

    // Очистить инвентарь
    public void ClearInventory()
    {
        // Сбрасываем все индикаторы
        if (uiIndicator != null)
        {
            uiIndicator.ResetAllIndicators();
        }

        collectedItems.Clear();

        if (showDebugMessages)
        {
            Debug.Log("Инвентарь очищен!");
        }
    }
}

