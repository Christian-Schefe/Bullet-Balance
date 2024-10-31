using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventObject : ScriptableObject, IRegistryObject
{
    public string eventId;
    public bool shouldShuffleChoices;

    public string Id => eventId;

    public abstract int GetChoiceCount();
    public abstract string GetChoiceText(int index);
    public abstract void ExecuteChoice(int index);
}
