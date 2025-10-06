using UnityEngine;

public enum UnitActionState
{
    Idle,
    Chase,
    Attack,
    Skill,
    Die
}

public class UnitState
{

    public UnitData UnitData { get; private set; }
    public Owner Owner { get; private set; }
    public UnitStats UnitStats { get; private set; }

    // Grade
    public int CurrentGrade { get; private set; }

    public IUnitContainer CurrentSlot { get; private set; }
    public UnitActionState CurrentState { get; private set; } = UnitActionState.Idle;

    public int _currentHp;
    public int _currentMp;
    public bool IsDead => _currentHp <= 0;

    public UnitState(UnitData unitData, Owner owner, int grade)
    {
        UnitData = unitData;
        Owner = owner;
        CurrentGrade = grade;
        UnitStats = unitData.GetStats(grade);

        _currentHp = UnitStats._hp;
        _currentMp = 0;
    }


    public void PlaceUnit(IUnitContainer slot) => CurrentSlot = slot;
    public void ChangeState(UnitActionState next) => CurrentState = next;

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        _currentHp = Mathf.Max(0, _currentHp - amount);
    }

    public void GainMp(int amount)
    {
        _currentMp += amount;
        if (_currentMp > 100)
            _currentMp = 100;
    }

    public void ResetForWave()
    {
        _currentHp = UnitStats._hp;
        _currentMp = 0;
        ChangeState(UnitActionState.Idle);
    }


}
