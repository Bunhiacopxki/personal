using UnityEngine;

public interface IMove
{
    Vector2 GetDirection(float speed, float deltaTime);
}
