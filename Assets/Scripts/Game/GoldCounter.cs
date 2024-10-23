using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        DataManger.PlayerData.goldStore.AddListener(ListenerLifetime.Scene, OnGoldChanged);
    }

    private void OnGoldChanged(bool isPresent, int gold)
    {
        goldText.text = gold.ToString();
    }
}
