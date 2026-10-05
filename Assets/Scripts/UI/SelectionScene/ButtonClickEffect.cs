using DG.Tweening;
using UnityEngine;

public class ButtonClickEffect : MonoBehaviour
{
    [SerializeField] private SelectionButton _selectionButton;
    [SerializeField] private float _selectedSize = 1.15f;
    [SerializeField] private float _duration = 0.2f;

    private readonly float _normalSize = 1f;

    private void Awake()
    {
        if (_selectionButton == null)
            _selectionButton = GetComponentInParent<SelectionButton>();
    }

    private void OnEnable()
    {
        if (_selectionButton != null)
            _selectionButton.SelectedChanged += HandleSelectedChanged;
    }

    private void OnDisable()
    {
        if (_selectionButton != null)
            _selectionButton.SelectedChanged -= HandleSelectedChanged;

        transform.DOKill();
    }

    private void HandleSelectedChanged(bool selected)
    {
        transform.DOKill();

        transform.DOScale(
            selected ? _selectedSize : _normalSize,
            _duration
        ).SetEase(
            selected ? Ease.OutBack : Ease.OutQuad
        );
    }
}