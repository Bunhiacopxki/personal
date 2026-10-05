using System;
using UnityEngine;

public class CooldownModel
{
    public float CurrentCooldown { get; private set; }
    public event Action<float> OnCooldown;

    public CooldownModel()
    {
        CurrentCooldown = 0;
    }

    public void StartCooldown(float cooldown)
    {
        CurrentCooldown = cooldown;
        OnCooldown?.Invoke(CurrentCooldown);
    }

    public void UpdateCooldown(float amount)
    {
        CurrentCooldown = Mathf.Max(CurrentCooldown - amount, 0);
        OnCooldown?.Invoke(CurrentCooldown);
    }
}
