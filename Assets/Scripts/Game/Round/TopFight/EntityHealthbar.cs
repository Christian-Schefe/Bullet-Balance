using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityHealthbar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer healthbar;
    [SerializeField] private TextMeshPro text;

    [SerializeField] private float whiteAbsoluteSpeed;
    [SerializeField] private float whiteRelativeSpeed;

    private float whiteCurrent;
    private float whiteTarget;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        var whiteDist = whiteTarget - whiteCurrent;
        var dist = Mathf.Abs(whiteDist);
        var step = Mathf.Sign(whiteDist) * Mathf.Min(Time.deltaTime * (whiteAbsoluteSpeed * 0.01f + whiteRelativeSpeed * dist), dist);

        SetWhiteCurrent(whiteCurrent + step);
    }

    private void SetWhiteCurrent(float scale)
    {
        whiteCurrent = scale;
        propertyBlock.SetFloat("_WhitePercentage", whiteCurrent);
        healthbar.SetPropertyBlock(propertyBlock);
    }

    public void UpdateHealthBar(int health, int maxHealth)
    {
        if (maxHealth == 0)
        {
            whiteTarget = 0;
            propertyBlock.SetFloat("_HealthPercentage", 0);
        }
        else
        {
            var healthPercentage = health / (float)maxHealth;
            propertyBlock.SetFloat("_HealthPercentage", healthPercentage);
            whiteTarget = healthPercentage;
        }
        text.text = $"{health}/{maxHealth}";
        SetWhiteCurrent(Mathf.Max(whiteCurrent, whiteTarget));
    }
}
