public class Skill_Wizard : ISkill
{
    public void Activate(Unit caster)
    {

        var enemies = caster.Services.Perception.GetAllEnemies(caster);
        foreach (var enemy in enemies)
        {
            caster.Services.Combat.DealDamage(caster, enemy, 80);
        }

    }
}
