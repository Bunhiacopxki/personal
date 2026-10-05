using UnityEngine;

public class VerticalMoveStrategy : IMove
{
    private float topLimit;
    private float bottomLimit;
    private Vector2 currentPosition;

    public VerticalMoveStrategy(float topLimit, float bottomLimit)
    {
        this.topLimit = topLimit;
        this.bottomLimit = bottomLimit;
        this.currentPosition = Vector2.zero;
    }

    public void UpdateCurrentPosition(Vector2 currentPosition)
    {
        this.currentPosition = currentPosition;
    }

    public Vector2 GetDirection(float speed, float deltaTime)
    {
        Vector2 direction = Vector2.zero;

        if (currentPosition.y >= bottomLimit)
        {
            direction = Vector2.down;
        }
        else if (currentPosition.y <= topLimit)
        {
            direction = Vector2.up;
        }

        return direction * speed * deltaTime;
    }
}
