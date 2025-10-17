public class Skill_Archer : ISkill
{
    public void Activate(Unit caster)
    {


        var t = caster.TargetRef ?? caster.Services.Perception.FindTarget(caster);
        if (t == null)
            return;

        for (int i = 0; i < 3; i++)
        {
            caster.Services.Combat.InstantRangeHit(caster, t, caster.UnitState.UnitStats._attack);
        }

    }

}
