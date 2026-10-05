using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemyController> _enemyPrefabs;
    [SerializeField] private Transform _enemyRoot;

    public static EnemyManager Instance { get; private set; }
    private List<EnemyController> enemyList = new List<EnemyController>();
    private Dictionary<EnemyType, EnemyController> _prefabMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _prefabMap = _enemyPrefabs.ToDictionary(
            x => x.Type,
            x => x
        );
    }

    private void Start()
    {
        Initalise();
    }

    private void Initalise()
    {
        for (int i = 0; i < _prefabMap.Count; i++) 
        {
            EnemyType type = _prefabMap.ElementAt(i).Key;
            LevelConfig currentLevel = LevelManager.Instance.GetLevel();
            int enemyCount = currentLevel.getRequireAmount(type);

            for (int j = 0; j < enemyCount; j++)
            {
                EnemyController enemy = Instantiate(
                    _prefabMap.ElementAt(i).Value, 
                    currentLevel.getPosition(type, j), 
                    Quaternion.identity, 
                    _enemyRoot
                );

                enemyList.Add(enemy);
            }
        }
    }
}
