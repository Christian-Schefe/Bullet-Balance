using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private SpriteRenderer selectorRenderer;

    public Vector2Int Position { get; private set; }
    public NodeType NodeType { get; private set; }

    public System.Action<Node> onClick;

    private TweenRunner runner;

    private void Awake()
    {
        SetSelected(false);
    }

    public void ConfigureNode(NodeType type, Vector2Int position)
    {
        NodeType = type;
        Position = position;
    }

    public void SetSprite(Sprite sprite, Color color)
    {
        iconRenderer.sprite = sprite;
        iconRenderer.color = color;
    }

    public void SetSelected(bool active)
    {
        selectorRenderer.enabled = active;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onClick?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.TweenScale(selectorRenderer.transform).To(Vector3.one * 1.25f).Duration(0.2f).Ease(Easing.QuadOut).RunImmediate(ref runner);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.TweenScale(selectorRenderer.transform).To(Vector3.one * 1.15f).Duration(0.2f).Ease(Easing.QuadOut).RunImmediate(ref runner);
    }
}
