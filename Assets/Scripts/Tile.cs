using UnityEngine;

public class Tile : MonoBehaviour
{
    public Unit Unit { get; private set; }
    public bool IsPlayerField { get; private set; }

    public int X { get; private set; }
    public int Y { get; private set; }

    private SpriteRenderer _renderer;
    private Color _defaultColor;
    private readonly Color _highlightColor = Color.yellow;

    public void Init(int x, int y, bool isPlayerField)
    {
        X = x;
        Y = y;
        IsPlayerField = isPlayerField;

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
            _defaultColor = _renderer.color;
    }

    public void HighlightTargetTile(bool active)
    {
        if (_renderer != null && IsPlayerField)
            _renderer.color = active ? _highlightColor : _defaultColor;
    }

    public void SetUnit(Unit unit)
    {
        Unit = unit;
    }

    public void ClearUnit()
    {
        Unit = null;
    }
}
