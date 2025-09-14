using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitDB", menuName = "Game/UnitDatabase")]
public class UnitDB : ScriptableObject
{

    public List<UnitData> _units;

    private Dictionary<UnitType, UnitData> _unitDict = new Dictionary<UnitType, UnitData>();

    private void OnEnable()
    {
        foreach (var unit in _units)
        {
            if (!_unitDict.ContainsKey(unit._unitType))
                _unitDict.Add(unit._unitType, unit);
        }
    }


    public UnitData GetUnitData(UnitType unitType)
    {
        if (_unitDict.TryGetValue(unitType, out var data))
            return data;

        return null;
    }

    public List<UnitData> GetAllUnits() => _units;

    public UnitData GetRandomUnit()
    {
        if (_units == null || _units.Count == 0)
            return null;

        int index = Random.Range(0, _units.Count);
        return _units[index];
    }

}
