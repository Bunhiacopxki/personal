public class DarkFactory : IAbilityFactory
{
    private readonly SkillConfig _skillConfig;

    public DarkFactory(SkillConfig skillConfig)
    {
        _skillConfig = skillConfig;
    }

    public ISkill CreateSkill()
    {
        return new DarkSkill(_skillConfig);
    }

    public IBuffs CreateBuffs()
    {
        return new DarkBuff();
    }
}
