

using UnityEngine;

public class UnitState
{

    public UnitData UnitData { get; private set; }
    public Owner Owner { get; private set; }
    public UnitStats UnitStats { get; private set; }
    public int CurrentGrade { get; private set; }


    public IUnitContainer CurrentSlot { get; private set; }

    public UnitState(UnitData unitData, Owner owner, int grade)
    {
        UnitData = unitData;
        Owner = owner;
        CurrentGrade = grade;

        UnitStats = unitData.GetStats(grade);

    }

    public void PlaceUnit(IUnitContainer slot)
    {
        CurrentSlot = slot;
    }

    public void Upgrade()
    {
        if (CurrentGrade < UnitData.MaxGrade)
        {
            CurrentGrade++;
            UnitStats = UnitData.GetStats(CurrentGrade);
        }
    }

}
