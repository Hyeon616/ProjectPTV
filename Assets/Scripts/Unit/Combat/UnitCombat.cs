using UnityEngine;

public class UnitCombat
{
    private UnitServices _services;

    public UnitCombat(UnitServices svc)
    {
        _services = svc;
    }

    public int ComputeAttackDamage(Unit attacker, Unit target)
    {
        int baseAtk = attacker.UnitState.UnitStats._attack;
        float atk = baseAtk * _services.Status.AttackSkill(attacker);
        float critDmg = _services.Status.ConsumeCrit(attacker);
        int damage = Mathf.Max(0, Mathf.RoundToInt(atk * critDmg));

        return damage;
    }

    public void DealDamage(Unit attacker, Unit target, int rawDamage)
    {
        if (target.UnitState.IsDead)
            return;

        if (target.Services.Status.IsInvincibility(target))
            return;

        float hitUnit = target.Services.Status.GuardBuff(target);
        int dmg = Mathf.Max(0, Mathf.RoundToInt(rawDamage * hitUnit));

        target.TakeDamage(dmg);

        bool isCrit = _services.Status.IsCriticalHit(attacker);

        DamageTextManager.Instance.ShowDamage(attacker, target, dmg, isCrit);

        int heal = Mathf.RoundToInt(_services.Status.ApplyLifeSteal(attacker, dmg));
        if (heal > 0)
            Heal(attacker, heal);

    }

    public void Heal(Unit unit, int healAmount)
    {
        if (unit.UnitState.IsDead)
            return;

        unit.UnitState._currentHp = Mathf.Min(unit.UnitState.UnitStats._hp, unit.UnitState._currentHp + healAmount);
    }

    public void InstantRangeHit(Unit attacker, Unit target, int baseDamage)
    {
        int raw = Mathf.RoundToInt(baseDamage * _services.Status.AttackSkill(attacker));
        raw = Mathf.RoundToInt(raw * _services.Status.ConsumeCrit(attacker));
        DealDamage(attacker, target, raw);
    }

}
