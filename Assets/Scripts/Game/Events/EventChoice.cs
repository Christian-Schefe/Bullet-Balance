using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EventChoice : MonoBehaviour
{
    [SerializeField] private BetterButton button;
    [SerializeField] private TextMeshProUGUI text;

    public string Text { get => text.text; set => text.text = value; }
    public UnityEvent OnClick => button.onClick;
}
