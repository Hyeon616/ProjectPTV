public class Skill_DarkLord : ISkill
{
    // 5초간 주변 적 도트 데미지
    public void Activate(Unit caster)
    {
        caster.Services.Status.StartDotAura(caster, radius: 2, dps: 40, duration: 5f);
    }

}
