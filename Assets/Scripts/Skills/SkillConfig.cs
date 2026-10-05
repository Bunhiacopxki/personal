using UnityEngine;

[CreateAssetMenu(fileName = "defaultSkill", menuName = "Skill/Config")]
public class SkillConfig : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private int _mana;
    [SerializeField] private float _cooldown;
    [SerializeField] private SkillType _type;

    public string Name => _name;
    public string Description => _description;
    public int Mana => _mana;
    public float Cooldown => _cooldown;
    public SkillType Type => _type;
}
