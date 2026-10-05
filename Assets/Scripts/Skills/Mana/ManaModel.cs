using System;
using UnityEngine;

public class ManaModel
{
    public int MaxMana { get; private set; }

    public int CurrentMana { get; private set; }

    public event Action<int, int> OnUpdateMana;

    public ManaModel(int maxMana)
    {
        MaxMana = maxMana;
        CurrentMana = 0;
    }

    public void ConsumeMana(int require)
    {
        CurrentMana = Mathf.Max(CurrentMana - require, 0);
        OnUpdateMana?.Invoke(CurrentMana, MaxMana);
    }

    public void RecoverMana(int amount)
    {
        CurrentMana = Mathf.Min(CurrentMana +  amount, MaxMana);
        OnUpdateMana?.Invoke(CurrentMana, MaxMana);
    }
}
