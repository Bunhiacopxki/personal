public class EnemyModel : CharacterModel
{
    public EnemyModel(int hp, int speed) : base(hp, speed) {}
    public virtual void UpdatePosition(float deltaTime) { }
}
