using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class BetterSlider : UIBehaviour, IPointerClickHandler, IDragHandler
{
    [SerializeField] private RectTransform handle;
    protected abstract Vector2 MinPosition { get; }
    protected abstract Vector2 MaxPosition { get; }

    [SerializeField] private float minValue, maxValue;
    [SerializeField] private int steps;

    public UnityEvent<float> onValueChanged;
    public UnityEvent<float> onProgressChanged;

    [SerializeField] private float progress;

    protected override void Awake()
    {
        SetProgress(progress);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (!Application.isPlaying) SetProgress(progress);
    }
#endif

    public float MinValue
    {
        get => minValue;
        set => SetMinValue(value);
    }

    public float MaxValue
    {
        get => maxValue;
        set => SetMaxValue(value);
    }

    public int Steps
    {
        get => steps;
        set => SetSteps(value);
    }

    public float Progress
    {
        get => progress;
        set => SetProgress(value);
    }
    public float Value
    {
        get => Mathf.Lerp(minValue, maxValue, progress);
        set => SetValue(value);
    }

    public void AddClickListener(UnityAction<float> action) => onValueChanged.AddListener(action);
    public void RemoveClickListener(UnityAction<float> action) => onValueChanged.RemoveListener(action);

    public void OnDrag(PointerEventData eventData)
    {
        var worldPos = ToLocalPosition(eventData);
        var projectedAlpha = ProjectOntoLine(MinPosition, MaxPosition, worldPos);
        SetProgress(projectedAlpha);
    }

    private float ProjectOntoLine(Vector2 start, Vector2 end, Vector2 pos)
    {
        var line = end - start;
        return Vector2.Dot(pos - start, line) / line.sqrMagnitude;
    }

    private Vector2 ToLocalPosition(PointerEventData data)
    {
        var rt = GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, data.position, data.pressEventCamera, out var mousePosInLocal))
        {
            return mousePosInLocal;
        }
        else return Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void SetMinValue(float value)
    {
        var curValue = Value;
        minValue = value;
        SetValue(curValue);
    }

    public void SetMaxValue(float value)
    {
        var curValue = Value;
        maxValue = value;
        SetValue(curValue);
    }

    public void SetSteps(int steps)
    {
        this.steps = Mathf.Max(0, steps);
        SetProgress(progress);
    }

    public void SetValue(float val)
    {
        SetProgress(Mathf.InverseLerp(minValue, maxValue, val));
    }

    public void SetProgress(float val)
    {
        progress = QuantizeToStep(Mathf.Clamp01(val));

        if (handle != null)
        {
            handle.localPosition = Vector2.Lerp(MinPosition, MaxPosition, progress);
        }
        OnProgressChanged(progress);
    }

    public float QuantizeToStep(float val)
    {
        if (steps <= 0) return val;
        return Mathf.Round(val * steps) / steps;
    }

    protected virtual void OnProgressChanged(float progress)
    {
        onProgressChanged?.Invoke(progress);
        onValueChanged?.Invoke(Value);
    }
}
