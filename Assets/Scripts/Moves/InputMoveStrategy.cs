using UnityEngine;

public class InputMoveStrategy : IMove
{
    private Vector2 direction;

    public void UpdateDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    public Vector2 GetDirection(float speed, float deltaTime)
    {
        return direction * speed * deltaTime;
    }
}
