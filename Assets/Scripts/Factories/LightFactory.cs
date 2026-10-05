public class LightFactory : IAbilityFactory
{
    private readonly SkillConfig _skillConfig;

    public LightFactory(SkillConfig skillConfig)
    {
        _skillConfig = skillConfig;
    }

    public ISkill CreateSkill()
    {
        return new LightSkill(_skillConfig);
    }

    public IBuffs CreateBuffs()
    {
        return new LightBuff();
    }
}
