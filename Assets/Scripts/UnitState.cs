

using UnityEngine;

public class UnitState
{

    public UnitData _unitData { get; private set; }
    public Owner _owner { get; private set; }

    public int _currentHp { get; private set; }
    public int _currentMp { get; private set; }

    public Tile _currentTile { get; private set; }

    public UnitState(UnitData unitData)
    {
        _unitData = unitData;
        _currentHp = _unitData._hp;
        _currentMp = 0;
    }

    public void PlaceUnit(Tile tile)
    {
        _currentTile = tile;

    }

    public void Attack()
    {
        _currentMp += _unitData._increaseMp;

        if (_currentMp >= 100)
        {
            UseSkill();
            _currentMp = 0;
        }
    }

    public void UseSkill()
    {
        Debug.Log($"{_unitData._unitName} 스킬 사용");
    }

    public void Die()
    {
        Debug.Log($"{_unitData._unitName} 사망");
    }

}
