public interface ISkill
{
    void SkillEffect();
    int ManaCost { get; }
    float CooldownTime { get; }
}
