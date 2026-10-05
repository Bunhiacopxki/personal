using UnityEngine;

public class PlayerController
{
    private PlayerModel model;
    private PlayerView view;
    private PlayerInputHandler inputHandler;

    public event System.Action OnUseSkill;

    public int ManaSkill => model.Skill.ManaCost;
    public float CdSkill => model.Skill.CooldownTime;

    public PlayerController(PlayerModel model, PlayerView view, PlayerInputHandler inputHandler)
    {
        this.model = model;
        this.view = view;
        this.inputHandler = inputHandler;

        this.model.OnHpChanged += this.view.UpdateHp;
        this.inputHandler.OnMoveInput += Move;
        this.view.UpdateHp(this.model.Hp, this.model.MaxHp);

        OnUseSkill += this.model.Skill.SkillEffect;
    }

    public void UseSkill() => OnUseSkill?.Invoke();

    public void BeAttacked(int damage)
    {
        model.TakeDamage(damage);

        if (model.Hp <= 0)
        {
            Die();
        }
    }

    public void Move(Vector2 direction)
    {
        model.MoveByInput(direction);
        view.MoveToPosition(model.GetDirection());
    }

    private void Die()
    {
        if (inputHandler != null)
        {
            inputHandler.OnMoveInput -= Move;
            Object.Destroy(inputHandler.gameObject);
        }

        if (view != null)
        {
            model.OnHpChanged -= view.UpdateHp;
            Object.Destroy(view.gameObject);
        }
    }
}
