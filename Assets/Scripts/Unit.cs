using UnityEngine;

public enum UnitType
{
    Knight,
    Mage,
    Archer
}

public struct UnitData
{
    public UnitType _unitType;
    public string _name;
    public int _hp;
    public int _mp;
    public int _attack;

    public UnitData(UnitType unitType, string name, int hp, int mp, int attack)
    {
        _unitType = unitType;
        _name = name;
        _hp = hp;
        _mp = mp;
        _attack = attack;

    }

}

public class Unit : MonoBehaviour
{
    public UnitData _unitData { get; private set; }
    public GameObject _unit { get; private set; }

    public Unit(UnitData unitData, GameObject unit, Vector3 pos, Transform parent)
    {
        _unitData = unitData;
        _unit = Instantiate(unit, pos, Quaternion.identity, parent);
        _unit.name = $"Unit_{unitData._name}";
    }
}
