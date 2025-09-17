using UnityEngine;

public class UnitFactory
{

    public Unit CreateUnit(UnitData unitData, Owner owner, int unitLayer, int grade)
    {
        if (unitData == null)
        {
            Debug.LogError(" UnitFactory: unitData is null!");
            return null;
        }
        if (unitData._prefab == null)
        {
            Debug.LogError($" UnitFactory: Prefab not assigned for {unitData._unitName}");
            return null;
        }

        GameObject unitPrefab = Object.Instantiate(unitData._prefab);
        unitPrefab.layer = unitLayer;


        Unit unit = unitPrefab.GetComponent<Unit>();
        unit.Init(unitData, owner, grade);

        return unit;

    }



}
