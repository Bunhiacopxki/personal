public class LevelModel
{
    public int Level { get; private set; }
    public LevelConfig Config { get; private set; }

    public LevelModel(LevelConfig LevelConfig)
    {
        Level = 1;
        Config = LevelConfig;
    }

    public void LevelUp(LevelConfig LevelConfig)
    {
        this.Level++;
        Config = LevelConfig;
    }
}
