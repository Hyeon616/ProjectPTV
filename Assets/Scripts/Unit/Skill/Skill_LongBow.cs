using UnityEngine;

public class Skill_LongBow : ISkill
{
    // 관통 화살
    public void Activate(Unit caster)
    {
        var target = caster.TargetRef ?? caster.Services.Perception.FindTarget(caster);
        if (target == null || caster.CurrentTileRef == null || target.CurrentTileRef == null)
            return;

        var lineTiles = caster.Services.Perception.GetLineTowards(caster.CurrentTileRef, target.CurrentTileRef);
        int damage = Mathf.RoundToInt(caster.UnitState.UnitStats._attack * 1.5f);

        foreach (var tile in lineTiles)
        {
            var enemy = tile.Unit;
            if (enemy != null && enemy.UnitState.Owner != caster.UnitState.Owner && !enemy.UnitState.IsDead)
                caster.Services.Combat.DealDamage(caster, enemy, damage);
        }


    }
}
