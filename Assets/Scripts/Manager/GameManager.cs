using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Sprite _playerImage { get; private set; }
    public SkillType _skillChosen { get; private set; }
    public BuffType _buffChosen { get; private set; }

    public void SetBuff(BuffType buffChosen) => _buffChosen = buffChosen;
    public void SetSkill(SkillType skillChosen) => _skillChosen = skillChosen;
    public void SetPlayerImage(Sprite playerImage) => _playerImage = playerImage;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
