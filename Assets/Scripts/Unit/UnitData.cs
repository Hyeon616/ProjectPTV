using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Knight,
    Mage,
    Archer,
    CamoArcher,
    DarkLord,
    LongBow,
    Paladin,
    Wizard,
    DeathKnight
}

public enum Owner
{
    Player,
    Enemy
}

[System.Serializable]
public class UnitStats
{
    public int _hp;
    public int _mp;
    public int _attack;
    public int _increaseMp;

    public int _cost;
    public int _attackRange;
    public float _attackInterval;
}


[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{

    public UnitType _unitType;
    public GameObject _prefab;
    public Sprite _benchPortrait;
    public Sprite _shopPortrait;
    public string _unitName;

    public List<UnitStats> _stats;


    public UnitStats GetStats(int grade)
    {

        if (grade <= 0 || grade > _stats.Count)
            return _stats[grade - 1];

        return _stats[grade - 1];
    }

    public int MaxGrade => _stats.Count;

}
