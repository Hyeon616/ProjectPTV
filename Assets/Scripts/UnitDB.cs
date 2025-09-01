using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitDB", menuName = "Game/UnitDatabase")]
public class UnitDB : ScriptableObject
{

    public List<UnitData> _units;

    private Dictionary<UnitType, UnitData> _unitDict = new Dictionary<UnitType, UnitData>();

    public void Init()
    {
        foreach (var unit in _units)
        {
            _unitDict[unit._unitType] = unit;
        }

    }

    public UnitData GetUnitData(UnitType unitType)
    {
        return _unitDict[unitType];
    }

}
