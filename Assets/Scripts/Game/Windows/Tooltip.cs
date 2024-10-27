using System;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI damageText;

    public void SetData(TooltipData data)
    {
        titleText.text = data.title;
        descriptionText.text = data.description;
        damageText.text = data.damage?.ToString() ?? "";
    }

    public void ShowAt(Vector2 position)
    {
        transform.position = position;
        SetOpen(true);
    }

    public void SetOpen(bool open)
    {
        container.SetActive(open);
    }
}

[Serializable]
public class TooltipData
{
    public string title;
    public string description;
    public int? damage;

    public TooltipData(string title, string description, int? damage)
    {
        this.title = title;
        this.description = description;
        this.damage = damage;
    }
}
