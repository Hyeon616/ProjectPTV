public class Skill_DeathKnight : ISkill
{
    // ÇÇÈí 3¹ø
    public void Activate(Unit caster)
    {
        caster.Services.Status.SetLifeSteal(caster, stacks: 3, percent: 0.5f);
    }

}
