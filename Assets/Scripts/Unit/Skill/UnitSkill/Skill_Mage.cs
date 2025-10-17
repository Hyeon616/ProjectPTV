public class Skill_Mage : ISkill
{
    public void Activate(Unit caster)
    {
        var target = caster.TargetRef ?? caster.Services.Perception.FindTarget(caster);
        if (target == null || target.CurrentTileRef == null)
            return;

        var tiles = caster.Services.Perception.GetTilesInManhattan(target.CurrentTileRef, 1);

        foreach (var tile in tiles)
        {
            var enemy = tile.Unit;
            if (enemy != null && enemy.UnitState.Owner != caster.UnitState.Owner && !enemy.UnitState.IsDead)
                caster.Services.Combat.DealDamage(caster, enemy, 130);
        }
    }
}
