public class UnitServices
{
    public UnitPerception Perception { get; }
    public UnitNavigator Navigator { get; }
    public UnitCombat Combat { get; }
    public UnitSkillController SkillController { get; }
    public UnitStatus Status { get; }

    public UnitServices(FieldManager fieldManager)
    {
        Perception = new UnitPerception(fieldManager);
        Navigator = new UnitNavigator(fieldManager);
        Combat = new UnitCombat(this);
        SkillController = new UnitSkillController();
        Status = new UnitStatus();
    }

    public void Tick(float dt)
    {


        Status.Tick(this, dt);
    }
}


