using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthEvent", menuName = "Events/HealthEvent")]
public class HealthEvent : EventObject
{
    [SerializeField] private List<HealthEventChoice> choices;

    public override void ExecuteChoice(int index)
    {
        var choice = choices[index];
        if (choice.healBy > 0) DataManager.HealPlayer(choice.healBy);
        else if (choice.healBy < 0) DataManager.DamagePlayer(-choice.healBy);

        if (choice.increaseMaxHealthBy > 0) DataManager.IncreaseMaxHealth(choice.increaseMaxHealthBy);
        else if (choice.increaseMaxHealthBy < 0) DataManager.DecreaseMaxHealth(-choice.increaseMaxHealthBy);
    }

    public override int GetChoiceCount()
    {
        return choices.Count;
    }

    public override string GetChoiceText(int index)
    {
        return choices[index].text;
    }

    [Serializable]
    private class HealthEventChoice
    {
        public string text;
        public int healBy;
        public int increaseMaxHealthBy;
    }
}
