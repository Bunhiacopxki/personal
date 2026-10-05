using UnityEngine;

public class HorizontalMoveStrategy : IMove
{
    private float leftLimit;
    private float rightLimit;
    private Vector2 currentPosition;

    public HorizontalMoveStrategy(float leftLimit, float rightLimit)
    {
        this.leftLimit = leftLimit;
        this.rightLimit = rightLimit;
        this.currentPosition = Vector2.zero;
    }

    public void UpdateCurrentPosition(Vector2 currentPosition)
    {
        this.currentPosition = currentPosition;
    }

    public Vector2 GetDirection(float speed, float deltaTime)
    {
        Vector2 direction = Vector2.zero;

        if (currentPosition.x >= rightLimit)
        {
            direction = Vector2.left;
        }
        else if (currentPosition.x <= leftLimit)
        {
            direction = Vector2.right;
        }

        return direction * speed * deltaTime;
    }
}
