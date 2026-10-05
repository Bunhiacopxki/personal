using TMPro;
using UnityEngine;

public class CooldownView : MonoBehaviour
{
    [SerializeField] private TMP_Text _remainingCooldown;

    public void UpdateCooldown(float cooldown)
    {
        if (_remainingCooldown == null || cooldown <= 0) return;

        _remainingCooldown.text = cooldown.ToString();
    }
}
