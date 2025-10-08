using System.Collections.Generic;
using UnityEngine;

public class UnitPerception
{
    private FieldManager _field;

    public UnitPerception(FieldManager field)
    {
        _field = field;
    }

    public bool IsInRange(Unit self, Unit target)
    {
        if (self == null || target == null) return false;
        if (self.CurrentTileRef == null || target.CurrentTileRef == null) return false;

        int md = Mathf.Abs(self.CurrentTileRef.X - target.CurrentTileRef.X)
               + Mathf.Abs(self.CurrentTileRef.Y - target.CurrentTileRef.Y);
        return md <= self.UnitState.UnitStats._attackRange;
    }

    public Unit FindNearestEnemy(Unit self)
    {
        Unit nearest = null;
        int minDist = int.MaxValue;

        foreach (var tile in _field.GetAllUnits())
        {
            Unit enemy = tile.Unit;
            if (enemy == null) continue;
            if (enemy.UnitState.Owner == self.UnitState.Owner) continue;
            if (enemy.UnitState.IsDead) continue;

            if (self.UnitState.CurrentSlot is not Tile myTile) continue;
            if (enemy.UnitState.CurrentSlot is not Tile enemyTile) continue;

            int dx = Mathf.Abs(myTile.X - enemyTile.X);
            int dy = Mathf.Abs(myTile.Y - enemyTile.Y);
            int dist = dx + dy;

            if (dist < minDist) { minDist = dist; nearest = enemy; }
        }
        return nearest;
    }

    public List<Unit> GetAllEnemies(Unit self)
    {
        var list = new List<Unit>();
        foreach (var tile in _field.GetAllUnits())
        {
            var u = tile.Unit;
            if (u != null && u.UnitState.Owner != self.UnitState.Owner && !u.UnitState.IsDead)
                list.Add(u);
        }
        return list;
    }

    public List<Unit> GetUnitsInRange(Unit self, int radius, bool onlyAllies, bool centerIsCaster = false)
    {
        var result = new List<Unit>();

        if (self == null || self.UnitState == null) 
            return result;
        if (self.UnitState.IsDead) 
            return result;         

        Tile center = centerIsCaster ? self.CurrentTileRef
                                     : (self.TargetRef != null ? self.TargetRef.CurrentTileRef : null);
        if (center == null) return result;

        foreach (var tile in _field.GetAllUnits())
        {

            if (tile.Unit.UnitState == null || tile.Unit.UnitState.IsDead)
                continue;

            if (onlyAllies && tile.Unit.UnitState.Owner != self.UnitState.Owner)
                continue;

            if (!onlyAllies && tile.Unit.UnitState.Owner == self.UnitState.Owner)
                continue;



            int md = Mathf.Abs(tile.X - center.X) + Mathf.Abs(tile.Y - center.Y);
            if (md <= radius)
                result.Add(tile.Unit);
        }

        return result;
    }

    public List<Tile> GetTilesInManhattan(Tile center, int radius)
    {
        var res = new List<Tile>();
        for (int x = 0; x < _field.Rows; x++)
            for (int y = 0; y < _field.Cols; y++)
            {
                int md = Mathf.Abs(x - center.X) + Mathf.Abs(y - center.Y);
                if (md <= radius) res.Add(_field.GetTile(x, y));
            }
        return res;
    }

    public List<Tile> GetLineTowards(Tile from, Tile to)
    {
        var list = new List<Tile>();
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
        {
            int step = (dx >= 0) ? 1 : -1;
            for (int x = from.X + step; x >= 0 && x < _field.Rows; x += step)
                list.Add(_field.GetTile(x, from.Y));
        }
        else
        {
            int step = (dy >= 0) ? 1 : -1;
            for (int y = from.Y + step; y >= 0 && y < _field.Cols; y += step)
                list.Add(_field.GetTile(from.X, y));
        }
        return list;
    }

    public Unit FindTarget(Unit self) => FindNearestEnemy(self);
}
