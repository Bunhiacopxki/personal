using System;
using UnityEngine;

public abstract class CharacterModel
{
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Speed { get; private set; }

    public event Action<int, int> OnHpChanged;

    protected CharacterModel(int hp, int speed)
    {
        Hp = hp;
        Speed = speed;
        MaxHp = hp;
    }

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(Hp - damage, 0);
        OnHpChanged?.Invoke(Hp, MaxHp);
    }

    public void HealHp(int hpRestore)
    {
        Hp = Math.Min(Hp + hpRestore, MaxHp);
        OnHpChanged?.Invoke(Hp, MaxHp);
    }

    public void ScaleMaxHp(float scale)
    {
        if (MaxHp <= 0 || scale <= 0) return;
        float ratio = (float) Hp / MaxHp;
        MaxHp = Mathf.RoundToInt(scale * MaxHp);
        Hp = Mathf.RoundToInt(ratio * MaxHp);
    }
}
