using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIGiveItem : MonoBehaviour
{
    [System.Serializable]
    public class ItemIndicator
    {
        public string itemID; // Уникальный ID предмета
        public Image indicatorImage; // Квадратик в UI
        public Color collectedColor = Color.green; // Цвет при подборе
        public Color defaultColor = Color.gray; // Исходный цвет
    }

    [Header("Индикаторы предметов")]
    [SerializeField] private List<ItemIndicator> itemIndicators = new List<ItemIndicator>();

    [Header("Настройки")]
    [SerializeField] private bool useSmoothColorChange = true;
    [SerializeField] private float colorChangeSpeed = 5f;

    private Dictionary<string, ItemIndicator> indicatorDictionary = new Dictionary<string, ItemIndicator>();

    void Start()
    {
        InitializeIndicators();
    }

    void Update()
    {
        if (useSmoothColorChange)
        {
            UpdateColorAnimations();
        }
    }

    private void InitializeIndicators()
    {
        // Заполняем словарь для быстрого доступа
        foreach (var indicator in itemIndicators)
        {
            if (!string.IsNullOrEmpty(indicator.itemID) && indicator.indicatorImage != null)
            {
                indicatorDictionary[indicator.itemID] = indicator;

                // Устанавливаем начальный цвет
                indicator.indicatorImage.color = indicator.defaultColor;
            }
        }
    }

    // Метод для обновления индикатора при подборе предмета
    public void UpdateIndicator(string itemID, bool isCollected)
    {
        if (indicatorDictionary.TryGetValue(itemID, out ItemIndicator indicator))
        {
            if (isCollected)
            {
                if (useSmoothColorChange)
                {
                    // Плавное изменение цвета
                    StartCoroutine(ChangeColorSmoothly(indicator.indicatorImage, indicator.collectedColor));
                }
                else
                {
                    // Мгновенное изменение цвета
                    indicator.indicatorImage.color = indicator.collectedColor;
                }

                Debug.Log($"Индикатор для предмета '{itemID}' изменён на цвет: {indicator.collectedColor}");
            }
            else
            {
                // Сброс цвета (если предмет потерян или используется)
                indicator.indicatorImage.color = indicator.defaultColor;
            }
        }
        else
        {
            Debug.LogWarning($"Индикатор для предмета с ID '{itemID}' не найден!");
        }
    }

    // Корутина для плавного изменения цвета
    private System.Collections.IEnumerator ChangeColorSmoothly(Image image, Color targetColor)
    {
        Color startColor = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * colorChangeSpeed;
            image.color = Color.Lerp(startColor, targetColor, elapsedTime);
            yield return null;
        }

        image.color = targetColor;
    }

    // Метод для анимации в Update (альтернатива корутине)
    private void UpdateColorAnimations()
    {
        // Здесь можно добавить анимации пульсации и т.д.
    }

    // Получить текущий цвет индикатора
    public Color GetIndicatorColor(string itemID)
    {
        if (indicatorDictionary.TryGetValue(itemID, out ItemIndicator indicator))
        {
            return indicator.indicatorImage.color;
        }
        return Color.clear;
    }

    // Сбросить все индикаторы
    public void ResetAllIndicators()
    {
        foreach (var indicator in itemIndicators)
        {
            if (indicator.indicatorImage != null)
            {
                indicator.indicatorImage.color = indicator.defaultColor;
            }
        }
    }
}