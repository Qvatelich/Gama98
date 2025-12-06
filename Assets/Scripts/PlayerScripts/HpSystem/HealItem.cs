using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour
{
    [Header("Настройки лечения")]
    [SerializeField] private int healAmount = 25;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private bool checkCanHeal = true; // Проверять ли возможность лечения

    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject pickupEffect;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip fullHealthSound; // Звук когда здоровье полное

    [Header("Сообщения")]
    [SerializeField] private bool showMessages = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup(other.GetComponent<PlayerHealth>());
        }
    }

    public void TryPickup(PlayerHealth playerHealth)
    {
        if (playerHealth == null) return;

        // Вариант 1: С проверкой CanHeal
        if (checkCanHeal)
        {
            if (playerHealth.CanHeal(healAmount))
            {
                // Лечение возможно
                bool healed = playerHealth.Heal(healAmount);

                if (healed)
                {
                    HandleSuccessfulPickup();
                }
            }
            else
            {
                // Лечение невозможно
                HandleFailedPickup(playerHealth);
            }
        }
        // Вариант 2: Без проверки (автоматически берет сколько можно)
        else
        {
            int actualHeal = playerHealth.SafeHeal(healAmount);

            if (actualHeal > 0)
            {
                HandleSuccessfulPickup();
            }
            else
            {
                HandleFailedPickup(playerHealth);
            }
        }
    }

    private void HandleSuccessfulPickup()
    {
        // Визуальные эффекты
        PlayEffect(pickupEffect, pickupSound);

        // Уничтожаем предмет если нужно
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void HandleFailedPickup(PlayerHealth playerHealth)
    {
        // Определяем причину
        if (playerHealth.IsHealthFull())
        {
            if (showMessages) Debug.Log("Здоровье уже максимальное!");
            PlayEffect(null, fullHealthSound);
        }
        else if (!playerHealth.CanHeal(healAmount))
        {
            int possibleHeal = playerHealth.GetMaxPossibleHeal(healAmount);
            if (showMessages) Debug.Log($"Можно вылечить только на {possibleHeal} единиц");
        }
    }

    private void PlayEffect(GameObject visualEffect, AudioClip sound)
    {
        if (visualEffect != null)
        {
            Instantiate(visualEffect, transform.position, Quaternion.identity);
        }

        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }
}