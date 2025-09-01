using UnityEngine;

public class Tile : MonoBehaviour
{
    public Unit _unit;
    public bool _isPlayerField;

    public bool IsEmptyTile() => _unit == null;

    public void PlaceUnit(Unit unit)
    {
        _unit = unit;
        _unit.transform.SetParent(transform);
        _unit.transform.localPosition = Vector3.zero;
    }

    public void RemoveUnit()
    {
        _unit = null;
    }
}
