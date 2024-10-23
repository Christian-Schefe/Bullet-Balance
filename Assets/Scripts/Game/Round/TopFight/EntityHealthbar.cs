using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityHealthbar : MonoBehaviour
{
    [SerializeField] private Transform healthbar;
    [SerializeField] private Transform whiteScalar;
    [SerializeField] private TextMeshPro text;

    [SerializeField] private float whiteAbsoluteSpeed;
    [SerializeField] private float whiteRelativeSpeed;

    private float whiteCurrent;
    private float whiteTarget;

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
        var whiteScale = whiteScalar.localScale;
        whiteScale.x = scale;
        whiteScalar.localScale = whiteScale;
    }

    public void UpdateHealthBar(int health, int maxHealth)
    {
        if (maxHealth == 0)
        {
            var scale = healthbar.localScale;
            scale.x = 0;
            whiteTarget = 0;
            SetWhiteCurrent(Mathf.Max(whiteCurrent, whiteTarget));

            healthbar.localScale = scale;
            text.text = $"Undefined";

        }
        else
        {
            var scale = healthbar.localScale;
            scale.x = health / (float)maxHealth;
            whiteTarget = scale.x;
            SetWhiteCurrent(Mathf.Max(whiteCurrent, whiteTarget));

            healthbar.localScale = scale;
            text.text = $"{health}/{maxHealth}";

        }
    }
}
