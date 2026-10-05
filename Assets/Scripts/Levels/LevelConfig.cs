using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "defaultLevel", menuName = "Level/Config")]
public class LevelConfig : ScriptableObject
{
    [Header("EnemyList")]
    [SerializeField] private int id;
    [SerializeField] private List<LevelEnemyList> enemyList;
    [Header("BatParameter")]
    [SerializeField] private float batRatio;
    [SerializeField] private List<Vector2> _batPosition;
    [Header("GolemParameter")]
    [SerializeField] private int golemAtk;
    [SerializeField] private List<Vector2> _golemPosition;
    [Header("BringerParameter")]
    [SerializeField] private float bringerScale;
    [SerializeField] private List<Vector2> _bringerPosition;

    public int ID => id;

    public int getRequireAmount(EnemyType type)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].Type == type) return enemyList[i].Amount;
        }
        return 0;
    }

    public float BatRatio => batRatio;
    public float BringerScale => bringerScale;
    public int GolemAtk => golemAtk;

    public Vector2 getPosition(EnemyType type, int index)
    {
        switch (type)
        {
            case EnemyType.Bat:
                return _batPosition[index];
            case EnemyType.Golem:
                return _golemPosition[index];
            case EnemyType.Bringer:
                return _bringerPosition[index];
            default:
                break;
        }

        return new Vector2(0, 0);
    }

    public int getPositionCount(EnemyType type)
    {
        switch (type) 
        {
            case EnemyType.Bat:
                return _batPosition.Count;
            case EnemyType.Golem:
                return _golemPosition.Count;
            case EnemyType.Bringer:
                return _bringerPosition.Count;
            default:
                break;
        }
        return 0;
    }
}

[Serializable]
public class LevelEnemyList
{
    [SerializeField] private EnemyType _type;
    [SerializeField] private int _amount;

    public EnemyType Type => _type;
    public int Amount => _amount;
}