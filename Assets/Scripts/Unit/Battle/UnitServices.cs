public class UnitServices
{
    public UnitPerception Perception { get; }
    public UnitNavigator Navigator { get; }

    public UnitServices(FieldManager fieldManager)
    {
        Perception = new UnitPerception(fieldManager);
        Navigator = new UnitNavigator(fieldManager);
    }

}
