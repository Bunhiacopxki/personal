using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionGroup : MonoBehaviour
{
    [SerializeField] private List<SelectionButton> _buttons;
    [SerializeField] private bool _allowDeselect = true;

    private SelectionButton _currentSelected = null;

    public SelectionButton CurrentSelected => _currentSelected;

    public event Action<SelectionButton> SelectionChanged;
    public event Action<bool> OnSelectionChanged;

    private void OnEnable()
    {
        foreach (var button in _buttons)
        {
            if (button != null)
                button.Clicked += HandleButtonClicked;
        }
    }

    private void OnDisable()
    {
        foreach (var button in _buttons)
        {
            if (button != null)
                button.Clicked -= HandleButtonClicked;
        }
    }

    private void HandleButtonClicked(SelectionButton clickedButton)
    {
        if (clickedButton == null)
            return;

        if (_currentSelected == clickedButton)
        {
            if (!_allowDeselect)
                return;

            _currentSelected.SetSelected(false);
            _currentSelected = null;
            OnSelectionChanged?.Invoke(false);
            SelectionChanged?.Invoke(null);
            return;
        }

        if (_currentSelected != null)
            _currentSelected.SetSelected(false);

        _currentSelected = clickedButton;
        _currentSelected.SetSelected(true);
        OnSelectionChanged?.Invoke(true);
        SelectionChanged?.Invoke(_currentSelected);
    }
}