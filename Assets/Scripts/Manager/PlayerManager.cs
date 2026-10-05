using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;

    private IAbilityFactory _factory;
    private PlayerView _view;
    private PlayerInputHandler _inputHandler;
    public PlayerController Player;
    public InputMoveStrategy _move;
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Initialize()
    {
        if (_factory == null) return;
        _move = new InputMoveStrategy();
        PlayerModel model = new PlayerModel(_config.Hp, _config.Speed, _factory, _move);
        _view.SetImage(GameManager.Instance._playerImage);
        Player = new PlayerController(model, _view, _inputHandler);
    }

    public void SetUpPlayer(IAbilityFactory factory, PlayerView view, PlayerInputHandler input)
    {
        _factory = factory;
        _view = view;
        _inputHandler = input;
        Initialize();
    }
}
