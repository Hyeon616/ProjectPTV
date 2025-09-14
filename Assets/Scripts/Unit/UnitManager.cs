public class UnitManager
{
    private readonly UnitFactory _unitFactory = new UnitFactory();
    private readonly FieldManager _fieldManager;
    private readonly BenchUI _benchUI;

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

        return unit;
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
