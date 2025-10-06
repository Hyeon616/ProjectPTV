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


    public Unit FindTarget(Unit self) => FindNearestEnemy(self);
}
