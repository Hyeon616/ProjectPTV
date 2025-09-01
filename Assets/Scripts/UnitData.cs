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
    DarkKnight
}

public enum Owner
{
    Player,
    Enemy
}



[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{

    public UnitType _unitType;
    public GameObject _prefab;
    public Sprite _portrait;
    public string _unitName;


    public int _hp;
    public int _mp;
    public int _attack;
    public int _increaseMp;


}
