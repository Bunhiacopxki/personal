using UnityEngine;

public class SkillController : MonoBehaviour
{
    private ManaModel manaModel;
    private ManaView manaView;
    private CooldownModel cooldownModel;
    private CooldownView cooldownView;

    private int _mana;
    private float _cooldown;

    public void Initialize(ManaModel manaModel, ManaView manaView, CooldownModel cooldownModel, CooldownView cooldownView)
    {
        this.manaModel = manaModel;
        this.manaView = manaView;
        this.manaModel.OnUpdateMana += this.manaView.UpdateMana;

        this.cooldownModel = cooldownModel;
        this.cooldownView = cooldownView;
        this.cooldownModel.OnCooldown += this.cooldownView.UpdateCooldown;
    }

    private void Start()
    {
        _mana = PlayerManager.Instance.Player.ManaSkill;
        _cooldown = PlayerManager.Instance.Player.CdSkill;

        PlayerManager.Instance.Player.OnUseSkill += HandleSkill;
    }

    private void Update()
    {
        cooldownModel.UpdateCooldown(1f * Time.deltaTime);
    }

    private bool CanUse()
    {
        if (manaModel == null || cooldownModel == null) return false;
        return manaModel.CurrentMana >= _mana && cooldownModel.CurrentCooldown <= 0;
    }

    public void HandleSkill()
    {
        if (!CanUse()) return;
        manaModel.ConsumeMana(_mana);
        cooldownModel.StartCooldown(_cooldown);
    }

    private void OnDestroy()
    {
        manaModel.OnUpdateMana -= manaView.UpdateMana;
        cooldownModel.OnCooldown -= cooldownView.UpdateCooldown;
        PlayerManager.Instance.Player.OnUseSkill -= HandleSkill;
    }
}
