using UnityEngine;
using UnityEngine.UI;

public class EnemyView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _enemyImage;

    public void SetEnemyImage(Sprite sprite)
    {
        _enemyImage.sprite = sprite;
    }
}
