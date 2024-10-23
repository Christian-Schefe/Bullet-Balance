using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChoiceSystem : MonoBehaviour
{
    [SerializeField] private Transform choiceParent;
    [SerializeField] private EventChoice choicePrefab;
    [SerializeField] private int choiceCount;

    private List<EventChoice> choiceList;

    private void Start()
    {
        choiceList = new();

        for (int i = 0; i < choiceCount; i++)
        {
            var choice = Instantiate(choicePrefab, choiceParent);
            var index = i;
            choice.Text = $"Choice {i + 1}";
            choice.OnClick.AddListener(() => Choose(index));
            choiceList.Add(choice);
        }
    }

    private void Choose(int index)
    {
        print($"Choice {index + 1} was chosen.");
        Globals<RunManager>.Instance.LoadScene(SceneType.Map);
    }
}
