using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventChoiceSystem : MonoBehaviour
{
    [SerializeField] private EventRegistry eventRegistry;
    [SerializeField] private Transform choiceParent;
    [SerializeField] private EventChoice choicePrefab;

    private EventObject eventObject;
    private SeededRandom rng;

    private void Start()
    {
        rng = new SeededRandom(DataManager.MapData.CurrentNodeInfo.sceneSeed);
        eventObject = GetRandomEvent();

        var choiceCount = eventObject.GetChoiceCount();
        var indices = Enumerable.Range(0, choiceCount).ToList();
        if (eventObject.shouldShuffleChoices) rng.Shuffle(indices);

        foreach (var i in indices)
        {
            var choice = Instantiate(choicePrefab, choiceParent);
            var index = i;
            choice.Text = eventObject.GetChoiceText(i);
            choice.OnClick.AddListener(() => Choose(index));
        }
    }

    private void Choose(int index)
    {
        print($"Choice {index + 1} was chosen.");
        eventObject.ExecuteChoice(index);
        Globals<RunManager>.Instance.LoadScene(SceneType.Map);
    }

    private EventObject GetRandomEvent()
    {
        var randomIndex = rng.IntRange(0, eventRegistry.Objects.Count);
        return eventRegistry.Objects[randomIndex];
    }
}
