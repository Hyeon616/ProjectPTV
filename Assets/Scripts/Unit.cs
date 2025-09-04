using UnityEngine;



public class Unit : MonoBehaviour
{

    public UnitState _unitState;
    public Tile _currentTile;

    public void Init(UnitState unitState, Tile tile)
    {
        _unitState = unitState;
        SetTile(tile);
    }

    public void SetTile(Tile tile)
    {
        _currentTile = tile;
        transform.SetParent(_currentTile.transform);
        transform.localPosition = new Vector3(0, 0.45f, 0);
    }

}
