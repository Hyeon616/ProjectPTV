using System.Collections.Generic;
using UnityEngine;


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

    public Unit SpawnUnit(UnitData unitData, Owner owner, Tile tile, int unitLayer)
    {

        Unit unit = _unitFactory.CreateUnit(unitData, owner, tile, tile.transform.parent, unitLayer);

        AddUnit(unit, owner);

        return unit;
    }

    public void AddUnit(Unit unit, Owner owner)
    {
        if (owner == Owner.Player)
            _playerUnits.Add(unit);
        else if (owner == Owner.Enemy)
            _enemyUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit._unitState.Owner == Owner.Player)
            _playerUnits.Remove(unit);
        else if (unit._unitState.Owner == Owner.Enemy)
            _enemyUnits.Remove(unit);
    }


    public void DragDrop(Unit unit, Tile targetTile)
    {
        if (unit._unitState.Owner != Owner.Player)
            return;

        if (targetTile == null)
        {
            MoveToInventory(unit);
            return;
        }

        if (targetTile.Unit != null)
        {
            SwapUnit(unit, targetTile.Unit);
            return;
        }

        MoveUnit(unit, targetTile);
    }

    private void MoveToInventory(Unit unit)
    {

    }

    private void MoveUnit(Unit unit, Tile targetTile)
    {

        if (unit._currentTile != null)
            unit._currentTile.ClearUnit();

        unit._currentTile = targetTile;
        targetTile.SetUnit(unit);
    }


    private void SwapUnit(Unit currentUnit, Unit swapUnit)
    {
        Tile currentTile = currentUnit._currentTile;
        Tile swapTile = swapUnit._currentTile;

        if (currentTile != null)
            currentTile.SetUnit(swapUnit);
        if (swapTile != null)
            swapTile.SetUnit(currentUnit);

        currentUnit._currentTile = swapTile;
        swapUnit._currentTile = currentTile;

    }



}
