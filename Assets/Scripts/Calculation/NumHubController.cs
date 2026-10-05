using UnityEngine;
using UnityEngine.UI;

public class NumHubController : MonoBehaviour
{
    [Header("Init Config")]
    [SerializeField] private bool useExactOnStart = true;
    [SerializeField] private bool demoteOnOverflow = false;
    [SerializeField] private long maxExactBits = 1 << 16;

    [Header("UI")]
    [SerializeField] private Toggle modeToggle;
    [SerializeField] private Text modeLabel;

    private void Awake()
    {
        NumHub.DemoteOnOverflow = demoteOnOverflow;
        NumHub.MaxExactBits = maxExactBits;
        NumHub.SetUseExact(useExactOnStart);
    }

    private void OnEnable()
    {
        NumHub.ModeChanged += OnModeChanged;
        if (modeToggle != null)
        {
            modeToggle.SetIsOnWithoutNotify(NumHub.UseExact);
            modeToggle.onValueChanged.AddListener(SetUseExact);
        }
        RefreshLabel();
    }

    private void OnDisable()
    {
        NumHub.ModeChanged -= OnModeChanged;
        if (modeToggle != null) modeToggle.onValueChanged.RemoveListener(SetUseExact);
    }

    public void SetUseExact(bool useExact) => NumHub.SetUseExact(useExact);

    public void ToggleMode() => NumHub.Toggle();

    public void SetDemoteOnOverflow(bool value) => NumHub.DemoteOnOverflow = value;
    
    private void OnModeChanged(bool useExact)
    {
        if (modeToggle != null) modeToggle.SetIsOnWithoutNotify(useExact);
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (modeLabel != null) modeLabel.text = NumHub.UseExact ? "BigInteger" : "Mantissa + Exponent";
    }
}