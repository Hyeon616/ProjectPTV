using UnityEngine;



public class Unit : MonoBehaviour
{

    public UnitState _unitState;
    public Tile _currentTile;
    public Owner _owner;

    public Tile _tile { get; private set; }

    public void Init(UnitState unitState, Owner owner, Tile tile)
    {
        _unitState = unitState;
        _owner = owner;
        SetTile(tile);
    }

    public void SetTile(Tile tile)
    {
        _tile = tile;
        transform.SetParent(_tile.transform);
        transform.localPosition = Vector3.zero;
    }

}
