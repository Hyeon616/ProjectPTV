using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void Activate(Unit caster);
}

public class UnitSkillController
{
    private Dictionary<UnitType, ISkill> _skills = new Dictionary<UnitType, ISkill>();

    public UnitSkillController()
    {
        _skills[UnitType.Knight] = new Skill_Knight();
        _skills[UnitType.Paladin] = new Skill_Paladin();
        _skills[UnitType.DeathKnight] = new Skill_DeathKnight();
        _skills[UnitType.DarkLord] = new Skill_DarkLord();
        _skills[UnitType.Archer] = new Skill_Archer();
        _skills[UnitType.CamoArcher] = new Skill_CamoArcher();
        _skills[UnitType.LongBow] = new Skill_LongBow();
        _skills[UnitType.Mage] = new Skill_Mage();
        _skills[UnitType.Wizard] = new Skill_Wizard();
    }

    public void Activate(Unit caster)
    {
        var type = caster.UnitState.UnitData._unitType;
        if (_skills.TryGetValue(type, out var skill))
            skill.Activate(caster);
    }

}
