using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    private int health;
    private int maxHealth;

    private void Awake()
    {
        DataManger.PlayerData.healthStore.AddListener(ListenerLifetime.Scene, OnHealthChanged);
        DataManger.PlayerData.maxHealthStore.AddListener(ListenerLifetime.Scene, OnMaxHealthChanged);
    }

    private void OnHealthChanged(bool isPresent, int health)
    {
        this.health = health;
        UpdateText();
    }

    private void OnMaxHealthChanged(bool isPresent, int maxHealth)
    {
        this.maxHealth = maxHealth;
        UpdateText();
    }

    private void UpdateText()
    {
        healthText.text = $"{health}/{maxHealth}";
    }
}
