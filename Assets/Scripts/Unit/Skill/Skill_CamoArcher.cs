public class Skill_CamoArcher : ISkill
{

    // 2초간 무적, 다음 공격 크리
    public void Activate(Unit caster)
    {
        caster.Services.Status.SetInvincibility(caster, 2f);
        caster.Services.Status.SetNextAttackCritical(caster, critMultiplier: 2f);
    }
}
