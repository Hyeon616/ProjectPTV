using UnityEngine;

public class UnitManager
{
    private readonly UnitFactory _unitFactory = new UnitFactory();

    public Unit SpawnUnit(UnitData unitData, Owner owner, IUnitContainer slot, int unitLayer)
    {

        Unit unit = _unitFactory.CreateUnit(unitData, owner, unitLayer);

        MoveUnit(unit, slot);

        return unit;
    }


    public bool DragDrop(Unit unit, IUnitContainer target)
    {
        if (unit.UnitState.Owner != Owner.Player)
            return false;

        if (unit == null || target == null)
            return false;

        if (target.IsField)
        {
            if (target is Tile tile)
            {
                if (!tile.IsPlayerField)
                    return false;
            }
            else
            {
                return false;
            }
        }

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

        unit.transform.SetParent(target.GetTransform());
        unit.transform.localPosition = target.IsField ? new Vector3(0, 0.45f, 0) : Vector3.zero;

    }


    private void SwapUnit(Unit currentUnit, Unit swapUnit)
    {
        IUnitContainer currentSlot = currentUnit.UnitState.CurrentSlot;
        IUnitContainer swapSlot = swapUnit.UnitState.CurrentSlot;

        currentSlot.SetUnit(swapUnit);
        swapSlot.SetUnit(currentUnit);

        currentUnit.UnitState.PlaceUnit(swapSlot);
        swapUnit.UnitState.PlaceUnit(currentSlot);

        currentUnit.transform.SetParent(swapSlot.GetTransform());
        swapUnit.transform.SetParent(currentSlot.GetTransform());

        currentUnit.transform.localPosition = swapSlot.IsField ? new Vector3(0, 0.45f, 0) : Vector3.zero;
        swapUnit.transform.localPosition = currentSlot.IsField ? new Vector3(0, 0.45f, 0) : Vector3.zero;

    }



}
