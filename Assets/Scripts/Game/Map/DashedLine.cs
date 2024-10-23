using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashedLine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private MaterialPropertyBlock materialPropertyBlock;

    [SerializeField] private Vector2 start, end;
    [SerializeField] private float width;
    [SerializeField] private float dashRatio;
    [SerializeField] private float cornerRadius;
    [SerializeField] private float dashCountFactor;

    public void SetPositions(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
        UpdateLine();
    }

    private void UpdateLine()
    {
        var diff = end - start;
        var length = diff.magnitude;

        transform.position = (start + end) * 0.5f;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, diff));

        spriteRenderer.transform.localScale = new(length, width);

        var dashCount = Mathf.CeilToInt(length * dashCountFactor);

        materialPropertyBlock ??= new();
        materialPropertyBlock.SetFloat("_Width", length);
        materialPropertyBlock.SetFloat("_Height", width);
        materialPropertyBlock.SetFloat("_DashRatio", Mathf.Clamp01(dashRatio));
        materialPropertyBlock.SetFloat("_DashCount", dashCount);
        materialPropertyBlock.SetFloat("_CornerRadius", Mathf.Max(cornerRadius, 0));
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    private void OnValidate()
    {
        UpdateLine();
    }
}
