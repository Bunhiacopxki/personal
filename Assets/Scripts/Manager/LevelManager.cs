using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelConfig> _levelList;

    private int currentLevel;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentLevel = 1;
    }

    private void Start()
    {
        CheckValidLevel();
    }

    public LevelConfig GetLevel()
    {
        foreach (var level in _levelList)
        {
            if (level.ID == currentLevel) return level;
        }
        return null;
    }

    #region Check Level
    private void CheckValidLevel()
    {
        for (int i = 0; i < _levelList.Count; i++)
        {
            foreach (EnemyType type in Enum.GetValues(typeof(EnemyType))) 
            {
                LogInvalidLevel(i, type);
            }
        }
    }

    private void LogInvalidLevel(int levelIndex, EnemyType type)
    {
        LevelConfig level = _levelList[levelIndex];
        if (level.getRequireAmount(type) != level.getPositionCount(type))
        {
            Debug.Log("Level " + levelIndex + " not enough " + type);
        }
    }
    #endregion
}
