

using UnityEngine;

public class UnitState
{

    public UnitData UnitData { get; private set; }
    public Owner Owner { get; private set; }

    public int CurrentHp { get; private set; }
    public int CurrentMp { get; private set; }

    public IUnitContainer CurrentSlot { get; private set; }

    public UnitState(UnitData unitData, Owner owner)
    {
        UnitData = unitData;
        Owner = owner;
        CurrentHp = UnitData._hp;
        CurrentMp = 0;
    }

    public void PlaceUnit(IUnitContainer slot)
    {
        CurrentSlot = slot;

    }

    public void Attack()
    {
        CurrentMp += UnitData._increaseMp;

        if (CurrentMp >= 100)
        {
            UseSkill();
            CurrentMp = 0;
        }
    }

    public void UseSkill()
    {
        Debug.Log($"{UnitData._unitName} 스킬 사용");
    }

    public void Die()
    {
        Debug.Log($"{UnitData._unitName} 사망");
    }

}
