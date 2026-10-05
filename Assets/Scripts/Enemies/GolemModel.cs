using UnityEngine;

public class GolemModel : EnemyModel
{
    private int atk;
    private VerticalMoveStrategy _move;
    public Vector2 Position { get; private set; }

    public GolemModel(int hp, int speed, int attack, VerticalMoveStrategy move) : base(hp, speed)
    {
        atk = attack;
        _move = move;
    }

    public void Attack()
    {

    }

    public override void UpdatePosition(float deltaTime)
    {
        _move.UpdateCurrentPosition(Position);
        Position += _move.GetDirection(Speed, deltaTime);
    }
}
