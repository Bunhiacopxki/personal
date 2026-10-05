using System;
using UnityEngine;

public class BringerModel : EnemyModel
{
    private int level;
    private float scale;
    private NoIMoveStrategy _move;
    public event Action<float> OnLevelUp;
    public Vector2 Position { get; private set; }

    public BringerModel(int hp, int speed, float scale, NoIMoveStrategy move) : base(hp, speed)
    {
        this.scale = scale;
        level = 1;
        OnLevelUp += ScaleMaxHp;
        _move = move;
    }

    public void UpgradeLevel()
    {
        OnLevelUp?.Invoke(++level + scale);
    }

    public void Dispose()
    {
        OnLevelUp -= ScaleMaxHp;
    }
    
    public override void UpdatePosition(float deltaTime)
    {
        Position += _move.GetDirection(Speed, deltaTime);
    }
}
