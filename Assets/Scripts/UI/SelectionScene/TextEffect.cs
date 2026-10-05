using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private SelectionButton _selectionButton;

    [Header("Color")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _hoverColor;
    [SerializeField] private Color _selectedColor;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();

        if (_selectionButton == null)
            _selectionButton = GetComponentInParent<SelectionButton>();
    }

    private void OnEnable()
    {
        if (_selectionButton != null)
        {
            _selectionButton.SelectedChanged += HandleStateChanged;
            _selectionButton.HoverChanged += HandleStateChanged;
        }

        UpdateColor();
    }

    private void OnDisable()
    {
        if (_selectionButton != null)
        {
            _selectionButton.SelectedChanged -= HandleStateChanged;
            _selectionButton.HoverChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged(bool _)
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_text == null || _selectionButton == null)
            return;

        if (_selectionButton.IsSelected)
        {
            _text.color = _selectedColor;
        }
        else if (_selectionButton.IsHovering)
        {
            _text.color = _hoverColor;
        }
        else _text.color = _normalColor;
    }
}