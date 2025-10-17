public class Skill_Paladin : ISkill
{

    // Èú
    public void Activate(Unit caster)
    {
        var allies = caster.Services.Perception.GetUnitsInRange(caster, 3, onlyAllies: true);
        foreach (var unit in allies)
        {
            caster.Services.Combat.Heal(unit, 100);
        }
    }
}
