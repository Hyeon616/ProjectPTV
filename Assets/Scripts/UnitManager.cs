using System.Collections.Generic;

public class UnitManager
{
    private FieldManager _fieldManager;
    private UnitFactory _unitFactory;
    private List<Unit> _playerUnits = new List<Unit>();
    private List<Unit> _enemyUnits = new List<Unit>();

    public UnitManager()
    {
        _unitFactory = new UnitFactory();
    }

    public Unit SpawnUnit(UnitData unitData, Owner owner, Tile tile)
    {

        Unit unit = _unitFactory.CreateUnit(unitData, owner, tile, tile.transform.parent);

        if (owner == Owner.Player)
            _playerUnits.Add(unit);
        else if (owner == Owner.Enemy)
            _enemyUnits.Add(unit);

        return unit;
    }

    public void DragDrop(Unit unit, Tile targetTile)
    {
        if (unit._unitState._owner != Owner.Player)
            return;


        if (targetTile.IsEmptyTile())
            MoveUnit(unit, targetTile);
        else
            SwapUnit(unit, targetTile);
    }

    private void MoveUnit(Unit unit, Tile targetTile)
    {
        unit._currentTile.RemoveUnit();
        targetTile.PlaceUnit(unit);
    }


    private void SwapUnit(Unit unit, Tile targetTile)
    {
        Unit swapUnit = targetTile._unit;
        Tile currentTile = unit._currentTile;

        currentTile.PlaceUnit(swapUnit);
        targetTile.PlaceUnit(unit);

        swapUnit.SetTile(currentTile);
        unit.SetTile(targetTile);

    }



}
