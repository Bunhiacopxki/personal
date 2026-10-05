using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private EnemyConfig _enemyConfig;
    [SerializeField] private EnemyView _view;

    private EnemyModel enemyModel;

    public EnemyType Type => _enemyType;

    private void Start()
    {
        Initalise(_enemyType);
    }

    #region Initalise
    private void Initalise(EnemyType enemyType)
    {
        switch (enemyType) 
        {
            case EnemyType.Bat:
                enemyModel = InitBat();
                break;
            case EnemyType.Golem:
                enemyModel = InitGolem();
                break;
            case EnemyType.Bringer:
                enemyModel = InitBringer();
                break;
            default:
                break;
        }

        if (enemyModel == null)
        {
            Debug.LogError("Enemy Model null");
        }
    }

    private BatModel InitBat()
    {
        if (_enemyConfig.type != EnemyType.Bat) return null;

        HorizontalMoveStrategy moveStrategy = new HorizontalMoveStrategy(leftLimit: -7f, rightLimit: 7f);
        _view.SetEnemyImage(_enemyConfig.image);

        return new BatModel(
            _enemyConfig.hp,
            _enemyConfig.speed,
            LevelManager.Instance.GetLevel().BatRatio,
            moveStrategy
        );
    }

    private GolemModel InitGolem()
    {
        if (_enemyConfig.type != EnemyType.Golem) return null;

        VerticalMoveStrategy moveStrategy = new VerticalMoveStrategy(topLimit: 7f, bottomLimit: -7f);
        _view.SetEnemyImage(_enemyConfig.image);

        return new GolemModel(
            _enemyConfig.hp,
            _enemyConfig.speed,
            LevelManager.Instance.GetLevel().GolemAtk,
            moveStrategy
        );
    }

    private BringerModel InitBringer()
    {
        if (_enemyConfig.type != EnemyType.Bringer) return null;

        NoIMoveStrategy moveStrategy = new NoIMoveStrategy();
        _view.SetEnemyImage(_enemyConfig.image);

        return new BringerModel(
           _enemyConfig.hp,
           _enemyConfig.speed,
           LevelManager.Instance.GetLevel().BringerScale,
           moveStrategy
       );

    }
    #endregion

    public void beAttacked(int damage)
    {
        enemyModel.TakeDamage(damage);
        if (enemyModel.Hp == 0) Die();
    }

    public void Die()
    {

    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        enemyModel.UpdatePosition(Time.deltaTime);
        // Update View

    }
}
