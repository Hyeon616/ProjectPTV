public class SkillState : IUnitState
{
    public void Enter(Unit u)
    {
        u.TriggerSkill();
    }

    public void Execute(Unit u)
    {
        
    }

    public void Exit(Unit u)
    {
        
    }
}
