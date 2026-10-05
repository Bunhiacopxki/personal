using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    private void Update()
    {
        Vector2 direction = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (direction != Vector2.zero)
        {
            OnMoveInput?.Invoke(direction);
        }
    }

    private void OnDestroy()
    {
        OnMoveInput = null;
    }
}
