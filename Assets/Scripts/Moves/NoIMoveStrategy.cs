using UnityEngine;

public class NoIMoveStrategy : IMove
{
    public Vector2 GetDirection(float speed, float deltaTime)
    {
        return Vector2.zero;
    }
}
