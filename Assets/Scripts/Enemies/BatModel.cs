using UnityEngine;

public class BatModel : EnemyModel
{
    private float _cooldown;
    private float _ratio;
    private HorizontalMoveStrategy _move;
    public Vector2 Position { get; private set; }

    public BatModel(int hp, int speed, float ratio, HorizontalMoveStrategy move) : base(hp, speed) 
    { 
        _ratio = ratio;
        _move = move;
    }

    private bool checkCooldown()
    {
        return _cooldown <= 0;
    }

    public void SuckBlood(int hpRestore)
    {
        if (!checkCooldown() && Random.Range(1, 10) < _ratio) return;
        PlayerManager.Instance.Player.BeAttacked(hpRestore);
        HealHp(hpRestore);
    }

    public override void UpdatePosition(float deltaTime)
    {
        _move.UpdateCurrentPosition(Position);
        Position += _move.GetDirection(Speed, deltaTime);
    }
}
