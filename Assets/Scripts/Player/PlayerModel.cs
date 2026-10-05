using UnityEngine;

public class PlayerModel : CharacterModel
{
    private ISkill _skill;
    private IBuffs _buffs;
    private InputMoveStrategy _move;
    public ISkill Skill => _skill;
    public IBuffs Buffs => _buffs;

    public PlayerModel(int hp, int speed, IAbilityFactory factory, InputMoveStrategy move) : base(hp, speed) 
    {
        _skill = factory.CreateSkill();
        _buffs = factory.CreateBuffs();
        _move = move;
    }

    public void MoveByInput(Vector2 direction)
    {
        _move.UpdateDirection(direction);
    }

    public Vector2 GetDirection()
    {
        return _move.GetDirection(Speed, Time.deltaTime);
    }
}