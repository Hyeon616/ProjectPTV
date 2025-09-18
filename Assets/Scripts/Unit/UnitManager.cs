using System.Collections.Generic;
using UnityEngine;

public class UnitManager
{
    private readonly UnitFactory _unitFactory = new UnitFactory();
    private readonly FieldManager _fieldManager;
    private readonly BenchUI _benchUI;

    private readonly Dictionary<UnitType, Dictionary<int, List<Unit>>> _allUnits = new Dictionary<UnitType, Dictionary<int, List<Unit>>>();

    public UnitManager(FieldManager fieldManager, BenchUI benchUI)
    {
        _fieldManager = fieldManager;
        _benchUI = benchUI;

    }

    public Unit SpawnUnit(UnitData unitData, Owner owner, int unitLayer, int grade)
    {
        BenchSlot emptySlot = _benchUI.GetEmptySlot();
        if (emptySlot != null)
            return CreateUnit(unitData, owner, emptySlot, unitLayer, grade);

        Tile emptyTile = _fieldManager.FindTilePriority();
        if (emptyTile != null)
            return CreateUnit(unitData, owner, emptyTile, unitLayer, grade);

        return null;
    }

    public Unit SpawnUnitCoordinate(UnitData unitData, Owner owner, Tile tile, int unitLayer, int grade)
    {
        Tile targetTile = tile;

        return CreateUnit(unitData, owner, targetTile, unitLayer, grade);

    }


    private Unit CreateUnit(UnitData unitData, Owner owner, IUnitContainer slot, int unitLayer, int grade)
    {

        Unit unit = _unitFactory.CreateUnit(unitData, owner, unitLayer, grade);
        MoveUnit(unit, slot);
        UpgradeUnits();

        return unit;
    }



    public void UpgradeUnits()
    {
        bool upgrade;

        do
        {
            upgrade = false;
            CollectUnits();

            foreach (var unitPair in _allUnits)
            {
                foreach (var gradePair in unitPair.Value)
                {
                    if (gradePair.Value.Count >= 3 && gradePair.Key < 3)
                    {
                        ExcuteUpgrade(gradePair.Value, gradePair.Key + 1);
                        upgrade = true;
                        break;
                    }
                }
                if (upgrade)
                    break;
            }

        }
        while (upgrade);
    }

    private void ExcuteUpgrade(List<Unit> units, int nextGrade)
    {

        Unit[] toUpgrade = new Unit[3];
        for (int i = 0; i < 3; i++)
            toUpgrade[i] = units[i];

        Unit mainUnit = null;
        int minDist = int.MaxValue;

        for (int i = 0; i < 3; i++)
        {
            Tile tile = toUpgrade[i].UnitState.CurrentSlot as Tile;
            if (tile != null && tile.IsPlayerField)
            {
                int dist = tile.X + tile.Y;
                if (dist < minDist)
                {
                    minDist = dist;
                    mainUnit = toUpgrade[i];
                }
            }
        }

        if (mainUnit == null)
            mainUnit = toUpgrade[0];

        IUnitContainer spawnSlot = mainUnit.UnitState.CurrentSlot;

        for (int i = 0; i < 3; i++)
        {
            toUpgrade[i].UnitState.CurrentSlot.ClearUnit();
            Object.Destroy(toUpgrade[i].gameObject);
        }

        Unit newUnit = _unitFactory.CreateUnit(mainUnit.UnitState.UnitData, Owner.Player, (int)LayerNum.Unit, nextGrade);

        newUnit.UnitState.PlaceUnit(spawnSlot);
        spawnSlot.SetUnit(newUnit);

        UpgradeEffect(toUpgrade, newUnit);
    }

    private void UpgradeEffect(Unit[] toUpgrade, Unit newUnit)
    {
        // TODO Effect
    }

    private void CollectUnits()
    {
        _allUnits.Clear();

        foreach (var tile in _fieldManager.GetAllUnits())
        {
            if (tile.Unit != null)
                CheckUnit(tile.Unit);
        }

        foreach (var slot in _benchUI.GetAllUnits())
        {
            if (slot.Unit != null)
                CheckUnit(slot.Unit);
        }

    }

    private void CheckUnit(Unit unit)
    {
        UnitType type = unit.UnitState.UnitData._unitType;
        int grade = unit.UnitState.CurrentGrade;

        if (!_allUnits.TryGetValue(type, out var gradeDict))
        {
            gradeDict = new Dictionary<int, List<Unit>>();
            _allUnits[type] = gradeDict;
        }

        if (!gradeDict.TryGetValue(grade, out List<Unit> list))
        {
            list = new List<Unit>();
            gradeDict[grade] = list;
        }

        list.Add(unit);

    }

    public bool DragDrop(Unit unit, IUnitContainer target)
    {
        if (unit.UnitState.Owner != Owner.Player)
            return false;

        if (unit == null || target == null)
            return false;

        if (target.IsField && target is Tile tile && !tile.IsPlayerField)
            return false;

        if (target.Unit != null)
            SwapUnit(unit, target.Unit);
        else
            MoveUnit(unit, target);

        return true;
    }


    private void MoveUnit(Unit unit, IUnitContainer target)
    {

        if (unit.UnitState.CurrentSlot != null)
            unit.UnitState.CurrentSlot.ClearUnit();

        target.SetUnit(unit);
        unit.UnitState.PlaceUnit(target);
    }


    private void SwapUnit(Unit currentUnit, Unit swapUnit)
    {
        IUnitContainer currentSlot = currentUnit.UnitState.CurrentSlot;
        IUnitContainer swapSlot = swapUnit.UnitState.CurrentSlot;

        if (currentSlot == null || swapSlot == null)
            return;

        currentSlot.SetUnit(swapUnit);
        swapSlot.SetUnit(currentUnit);

        currentUnit.UnitState.PlaceUnit(swapSlot);
        swapUnit.UnitState.PlaceUnit(currentSlot);


    }



}
