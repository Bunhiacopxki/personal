using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private SpriteRenderer _image;
    [SerializeField] private Rigidbody2D _rb;

    private Vector2 _input;

    private void Awake()
    {
        if (_hpSlider == null) _hpSlider = GetComponentInChildren<Slider>();
        if (_image ==  null) _image = GetComponentInChildren<SpriteRenderer>();
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();

        _hpSlider.value = 1f;
        _input = _rb.position;
    }

    public void SetImage(Sprite playerImage)
    {
        if (_image == null) return;
        _image.sprite = playerImage;
    }

    public void UpdateHp(int hp, int maxHp)
    {
        if (_hpSlider == null || maxHp <= 0) return;
        _hpSlider.value = (float)hp / maxHp;
    }

    public void MoveToPosition(Vector2 position)
    {
        _input = _rb.position + position;
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_input);
    }

    private void OnDestroy()
    {
        // cleanup event
    }
}
