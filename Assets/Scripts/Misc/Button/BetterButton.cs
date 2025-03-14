using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BetterButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool interactable = true;
    private bool isPointerInside;
    private bool isPointerDown;

    private State currentState;
    public State CurrentState => currentState;

    public UnityEvent onClick;
    public UnityEvent<bool> onHover;

    public bool Interactable { get => interactable; set => SetInteractable(value); }

    public void AddClickListener(UnityAction action) => onClick.AddListener(action);
    public void RemoveClickListener(UnityAction action) => onClick.RemoveListener(action);

    public void AddHoverListener(UnityAction<bool> action) => onHover.AddListener(action);
    public void RemoveHoverListener(UnityAction<bool> action) => onHover.RemoveListener(action);

    public void SetInteractable(bool value)
    {
        if (interactable == value) return;
        interactable = value;
        if (isPointerInside)
        {
            onHover?.Invoke(interactable);
        }
        UpdateInteractableState();
    }

    protected override void OnDisable()
    {
        if (interactable && isPointerInside)
        {
            onHover?.Invoke(false);
        }
    }

    protected override void OnEnable()
    {
        if (isPointerInside && interactable)
        {
            onHover?.Invoke(true);
        }
        UpdateInteractableState();
    }

    private void UpdateInteractableState()
    {
        var newState = interactable ? State.Interactable : State.Disabled;
        if (interactable && isPointerInside)
        {
            newState = isPointerDown ? State.Pressed : State.Hovered;
        }
        ToState(newState);
    }

    protected virtual void ToState(State state)
    {
        currentState = state;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        isPointerDown = true;
        if (interactable) ToState(State.Pressed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (interactable && isPointerInside) onClick?.Invoke();
        isPointerDown = false;
        if (interactable) ToState(isPointerInside ? State.Hovered : State.Interactable);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        if (interactable)
        {
            ToState(isPointerDown ? State.Pressed : State.Hovered);
            onHover?.Invoke(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        isPointerDown = false;
        if (interactable)
        {
            ToState(State.Interactable);
            onHover?.Invoke(false);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (Application.isPlaying) return;
        UpdateInteractableState();
    }
#endif

    public enum State
    {
        Interactable, Disabled, Pressed, Hovered
    }
}
