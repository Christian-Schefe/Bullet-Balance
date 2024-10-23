using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    private PlayerData playerData;

    [SerializeField] private EntityHealthbar healthbar;

    private void Awake()
    {
        playerData = DataManger.PlayerData;

        playerData.healthStore.AddListener(ListenerLifetime.Scene, (_, _) => UpdateHealthBar());
        playerData.maxHealthStore.AddListener(ListenerLifetime.Scene, (_, _) => UpdateHealthBar());
    }

    private void UpdateHealthBar()
    {
        healthbar.UpdateHealthBar(playerData.Health, playerData.MaxHealth);
    }
}
