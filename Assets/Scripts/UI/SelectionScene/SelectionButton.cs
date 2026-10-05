using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    [SerializeField] private int _index = -1;

    public bool IsSelected { get; private set; }
    public bool IsHovering { get; private set; }

    public event Action<SelectionButton> Clicked;
    public event Action<bool> SelectedChanged;
    public event Action<bool> HoverChanged;

    public int Index => _index;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        Clicked?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        if (IsSelected == selected)
            return;

        IsSelected = selected;
        SelectedChanged?.Invoke(IsSelected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovering = true;
        HoverChanged?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovering = false;
        HoverChanged?.Invoke(false);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClick);
    }
}