using UnityEngine;

[CreateAssetMenu(fileName = "defaultPlayer", menuName = "Player/Config")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private int _hp;
    [SerializeField] private int _speed;

    public int Hp => _hp;
    public int Speed => _speed;
}
