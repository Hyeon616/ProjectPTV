public class SkillState : IUnitState
{
    public void Enter(Unit u)
    {
        u.TriggerSkill();
        u.Services.SkillController.Activate(u);
    }

    public void Execute(Unit u)
    {

    }

    public void Exit(Unit u)
    {

    }
}
