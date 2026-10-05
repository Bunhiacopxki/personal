using UnityEngine;

[CreateAssetMenu(fileName = "defaultEnemy", menuName = "Enemy/Config")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private int _hp;
    [SerializeField] private int _speed;
    [SerializeField] private int _score;
    [SerializeField] private Sprite _image;
    [SerializeField] private EnemyType _type;

    public int hp => _hp;
    public int speed => _speed;
    public int score => _score;
    public Sprite image => _image;
    public EnemyType type => _type;
}
