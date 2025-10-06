using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void Activate(Unit caster);
}

public class UnitSkillController
{
    private Dictionary<UnitType, ISkill> _skills = new Dictionary<UnitType, ISkill>();





}
