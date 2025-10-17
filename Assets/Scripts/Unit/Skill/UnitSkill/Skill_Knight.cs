
public class Skill_Knight : ISkill
{
    
    // 받는 피해 50%
    public void Activate(Unit caster)
    {
        caster.Services.Status.GuardSkill(caster, 0.5f, 5f);

        caster.Services.Vfx.KnightEffect(caster);
    }
}
