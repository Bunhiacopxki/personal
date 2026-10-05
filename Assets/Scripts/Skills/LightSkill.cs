public class LightSkill : ISkill
{
    private readonly SkillConfig _config;

    public LightSkill(SkillConfig config)
    {
        _config = config;
    }

    public int ManaCost => _config.Mana;
    public float CooldownTime => _config.Cooldown;

    public void SkillEffect()
    {

    }
}
