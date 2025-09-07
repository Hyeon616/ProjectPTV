using UnityEngine;

public class UnitFactory
{

    public Unit CreateUnit(UnitData unitData, Owner owner, int unitLayer)
    {
        GameObject unitPrefab = Object.Instantiate(unitData._prefab);
        Unit unit = unitPrefab.GetComponent<Unit>() ?? unitPrefab.AddComponent<Unit>();

        unitPrefab.layer = unitLayer;

        UnitState state = new UnitState(unitData, owner);
        unit.Init(state);

        return unit;

    }



}
