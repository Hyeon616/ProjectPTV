using UnityEngine;

public class ShopManager
{
    private readonly UnitManager _unitManager;
    private UnitDB _unitDB;

    private int _currentLevel = 1;
    private int _playerGold = 500;

    public ShopManager(UnitManager unitManager, UnitDB unitDB)
    {
        _unitManager = unitManager;
        _unitDB = unitDB;
    }

    public bool BuyUnit(UnitData unitData, int grade, int price)
    {
        if (_playerGold < price)
        {
            return false;
        }

        _unitManager.SpawnUnit(unitData, Owner.Player, (int)LayerNum.Unit, grade);
        
        _playerGold -= price;
        return true;
    }


    public (UnitData, int) GetShopUnit()
    {
        float[] probs = ShopProbability.GetProbabilities(_currentLevel);
        int grade = GetUnitGrade(probs);

        UnitData unitData = _unitDB.GetRandomUnit();
        return (unitData, grade);
    }

    private int GetUnitGrade(float[] probs)
    {
        float grade = Random.Range(0, 100f);
        float cumulative = 0f;

        for (int i = 0; i < probs.Length; i++)
        {
            cumulative += probs[i];
            if (grade <= cumulative)
                return i + 1;
        }
        return 1;

    }

}
