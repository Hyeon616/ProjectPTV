using UnityEngine;

public class UnitFactory
{


    public Unit CreateUnit(UnitData unitData, Owner owner, Tile tile, Transform parent, int unitLayer)
    {
        GameObject unitPrefab = Object.Instantiate(unitData._prefab, parent);
        Unit unit = unitPrefab.GetComponent<Unit>() ?? unitPrefab.AddComponent<Unit>();

        unitPrefab.layer = unitLayer;

        UnitState state = new UnitState(unitData, owner);
        unit.Init(state, tile);

        return unit;

    }



}
