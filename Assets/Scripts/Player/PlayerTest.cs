using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        Debug.Log(
            $"Player va chạm với {collision.gameObject.name}, " +
            $"normal = {contact.normal}"
        );
    }
}
