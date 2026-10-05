using DG.Tweening;
using UnityEngine;

public class StartButtonClickEffect : MonoBehaviour
{
    [SerializeField] private SelectionUI _selectionUI;
    [SerializeField] private float _selectedSize = 1.15f;
    [SerializeField] private float _duration = 0.2f;

    private float _normalSize = 1f;

    private void Start()
    {
        _selectionUI.OnStartGame += PlayClickEffect;
        _selectionUI.OnStartGame += SetBuff;
        _selectionUI.OnStartGame += SetSkill;
    }

    private void OnDestroy()
    {
        if (_selectionUI != null)
        {
            _selectionUI.OnStartGame -= PlayClickEffect;
            _selectionUI.OnStartGame -= SetBuff;
            _selectionUI.OnStartGame -= SetSkill;
        }

        transform.DOKill();
    }

    private void PlayClickEffect()
    {
        transform.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            transform.DOScale(_selectedSize, _duration)
                .SetEase(Ease.OutBack)
        );

        sequence.Append(
            transform.DOScale(_normalSize, _duration)
                .SetEase(Ease.OutQuad)
        );

        sequence.SetUpdate(true);
    }

    private void SetBuff()
    {
        int index = _selectionUI.BuffGroup.CurrentSelected.Index;
        if (index < 0) return;
        GameManager.Instance.SetBuff((BuffType) index);
    }

    private void SetSkill()
    {
        int index = _selectionUI.SkillGroup.CurrentSelected.Index;
        if (index < 0) return;
        GameManager.Instance.SetSkill((SkillType)index);
    }
}