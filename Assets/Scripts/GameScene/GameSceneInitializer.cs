using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameSceneInitializer : MonoBehaviour
{
    [SerializeField] private List<SkillConfig> _listSkills;
    [SerializeField] private GameObject _player;

    private void Start()
    {
        InitPlayer();
    }

    #region InitPlayer
    private void InitPlayer()
    {
        PlayerView view = _player.GetComponent<PlayerView>();
        PlayerInputHandler input = _player.GetComponent<PlayerInputHandler>();
        IAbilityFactory factory = CreateFactory();
        PlayerManager.Instance.SetUpPlayer(factory, view, input);
    }

    private IAbilityFactory CreateFactory()
    {
        SkillConfig skill = GetPlayerSkill();
        switch(GameManager.Instance._buffChosen)
        {
            case BuffType.Monster:
                return new DarkFactory(skill);
            case BuffType.Knight:
                return new LightFactory(skill);
            default:
                return null;
        }
    }

    private SkillConfig GetPlayerSkill()
    {
        foreach (SkillConfig skillType in _listSkills)
        {
            if (skillType.Type == GameManager.Instance._skillChosen)
            {
                return skillType;
            }
        }

        return null;
    }
    #endregion
}
