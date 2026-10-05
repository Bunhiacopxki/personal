using UnityEngine;
using UnityEngine.UI;

public class ManaView : MonoBehaviour
{
    [SerializeField] private Slider _manaSlider;

    public void UpdateMana(int mana, int maxMana)
    {
        if (_manaSlider == null || maxMana <= 0) return;

        _manaSlider.value = (float)mana / maxMana;
    }
}
