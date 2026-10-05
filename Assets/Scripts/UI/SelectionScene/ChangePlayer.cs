using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayer : MonoBehaviour
{
    [SerializeField] private SelectionUI selectionUI;
    [SerializeField] private List<PLayerImage> PlayerList;
    [SerializeField] private Image _playerImage;

    private SelectionButton _buff;
    private SelectionButton _skill;

    private void Awake()
    {
        selectionUI.BuffGroup.SelectionChanged += setBuff;
        selectionUI.SkillGroup.SelectionChanged += setSkil;
    }

    private void Start()
    {
        _buff = null;
        _skill = null;
    }

    private void setBuff(SelectionButton buff)
    {
        _buff = buff;
        ChangePlayerImage();
    }

    private void setSkil(SelectionButton skil)
    {
        _skill = skil;
        ChangePlayerImage();
    }

    private void ChangePlayerImage()
    {
        if (_buff == null || _skill == null) return;

        foreach (var playerType in PlayerList)
        {
            if (playerType.Buff == _buff && playerType.Skill == _skill)
            {
                _playerImage.sprite = playerType.Image;
                GameManager.Instance.SetPlayerImage(playerType.Image);
            }
        }
    }
}

[System.Serializable]
public struct PLayerImage
{
    [SerializeField] private SelectionButton _buff;
    [SerializeField] private SelectionButton _skill;
    [SerializeField] private Sprite _image;

    public SelectionButton Buff => _buff;
    public SelectionButton Skill => _skill;
    public Sprite Image => _image;
}
