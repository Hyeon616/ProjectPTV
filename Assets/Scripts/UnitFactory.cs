using UnityEngine;

public class UnitFactory
{

    public Unit CreateUnit(UnitData unitData, Owner owner, Tile tile, Transform parent)
    {
        GameObject unitPrefab = Object.Instantiate(unitData._prefab, parent);
        Unit unit = unitPrefab.GetComponent<Unit>() ?? unitPrefab.AddComponent<Unit>();

        var state = new UnitState(unitData);
        unit.Init(state, owner ,tile);

        tile.PlaceUnit(unit);
        return unit;

    }



}
