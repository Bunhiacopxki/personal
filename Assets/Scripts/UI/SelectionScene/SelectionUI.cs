using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionGroup _buffGroup;
    [SerializeField] private SelectionGroup _skillGroup;
    [SerializeField] private Button _startButton;

    public event Action OnStartGame;
    private bool _buffChosen;
    private bool _skillChosen;

    public SelectionGroup BuffGroup => _buffGroup;
    public SelectionGroup SkillGroup => _skillGroup;

    private void Awake()
    {
        _buffGroup.OnSelectionChanged += SetBuff;
        _skillGroup.OnSelectionChanged += SetSkill;
        _startButton.gameObject.SetActive(false);
    }

    private void SetBuff(bool status)
    {
        _buffChosen = status;
        ActiveStartButton();
    }

    private void SetSkill(bool status)
    {
        _skillChosen = status;
        ActiveStartButton();
    }

    private void ActiveStartButton()
    {
        if (_buffChosen == true && _skillChosen == true)
        {
            _startButton.gameObject.SetActive(true);
        }
        else 
        {
            _startButton.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (_startButton != null)
            _startButton.onClick.AddListener(HandleStartClicked);
    }

    private void OnDisable()
    {
        if (_startButton != null)
            _startButton.onClick.RemoveListener(HandleStartClicked);
    }

    private void HandleStartClicked()
    {
        OnStartGame?.Invoke();
        SceneTransition.LoadScene("InGameScene");
    }
}