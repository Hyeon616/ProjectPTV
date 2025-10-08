using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatus
{
    private class BuffTimer
    {
        public float _timeLeft;
        public Action _onEnd;
    }

    private Dictionary<Unit, List<BuffTimer>> _timers = new Dictionary<Unit, List<BuffTimer>>();

    private Dictionary<Unit, float> _multiHitSkill = new Dictionary<Unit, float>();
    private Dictionary<Unit, float> _attackSkill = new Dictionary<Unit, float>();
    private Dictionary<Unit, float> _attackSpeedSkill = new Dictionary<Unit, float>();

    private readonly HashSet<Unit> _invincibility = new HashSet<Unit>();
    private Dictionary<Unit, (int stacks, float percent)> _lifeSteal = new Dictionary<Unit, (int stacks, float percent)>();
    private Dictionary<Unit, float> _critNext = new Dictionary<Unit, float>();

    private Dictionary<Unit, (float radius, float dps, float timeLeft)> _dotAura = new Dictionary<Unit, (float radius, float dps, float timeLeft)>();

    private static float GetMul(Dictionary<Unit, float> dict, Unit u)
    {
        return dict.TryGetValue(u, out var v) ? v : 1f;
    }

    private static void SetMul(Dictionary<Unit, float> dict, Unit u, float value)
    {
        if (Approximately(value, 1f)) dict.Remove(u);
        else dict[u] = value;
    }

    private static bool Approximately(float a, float b) => Mathf.Abs(a - b) < 0.0001f;

    private static bool IsGone(Unit u)
    {
        return u == null || u.UnitState == null || u.UnitState.IsDead || !u.gameObject.activeInHierarchy;
    }

    public void MultiHitSkill(Unit u, float mul, float duration)
    {
        if (u == null) return;
        var cur = GetMul(_multiHitSkill, u);
        cur *= mul;
        SetMul(_multiHitSkill, u, cur);

        AddTimer(u, duration, () =>
        {
            if (_multiHitSkill.TryGetValue(u, out var v))
            {
                v /= mul;
                SetMul(_multiHitSkill, u, v);
            }
        });
    }

    public void AttackBuff(Unit u, float mul, float duration)
    {
        if (u == null) return;
        var cur = GetMul(_attackSkill, u);
        cur *= mul;
        SetMul(_attackSkill, u, cur);

        AddTimer(u, duration, () =>
        {
            if (_attackSkill.TryGetValue(u, out var v))
            {
                v /= mul;
                SetMul(_attackSkill, u, v);
            }
        });
    }

    public void AttackSpeedBuff(Unit u, float mul, float duration)
    {
        if (u == null) return;
        var cur = GetMul(_attackSpeedSkill, u);
        cur *= mul;
        SetMul(_attackSpeedSkill, u, cur);

        AddTimer(u, duration, () =>
        {
            if (_attackSpeedSkill.TryGetValue(u, out var v))
            {
                v /= mul;
                SetMul(_attackSpeedSkill, u, v);
            }
        });
    }

    public void SetInvincibility(Unit u, float duration)
    {
        if (u == null) return;
        _invincibility.Add(u);
        AddTimer(u, duration, () => _invincibility.Remove(u));
    }

    public void SetNextAttackCritical(Unit u, float critMultiplier)
    {
        if (u == null) return;
        _critNext[u] = Mathf.Max(critMultiplier, 1f);
    }

    public void SetLifeSteal(Unit u, int stacks, float percent)
    {
        if (u == null) return;
        _lifeSteal[u] = (Mathf.Max(0, stacks), Mathf.Clamp01(percent));
        if (_lifeSteal[u].stacks <= 0 || _lifeSteal[u].percent <= 0f)
            _lifeSteal.Remove(u);
    }

    public void StartDotAura(Unit u, float radius, float dps, float duration)
    {
        if (u == null) return;
        _dotAura[u] = (Mathf.Max(0f, radius), Mathf.Max(0f, dps), Mathf.Max(0f, duration));
        if (_dotAura[u].timeLeft <= 0f) _dotAura.Remove(u);
    }

    public float MultiHit(Unit u) => GetMul(_multiHitSkill, u);
    public float AttackSkill(Unit u) => GetMul(_attackSkill, u);
    public float AttackSpeedSkill(Unit u) => GetMul(_attackSpeedSkill, u);
    public bool IsInvincibility(Unit u) => _invincibility.Contains(u);

    public float ConsumeCrit(Unit u)
    {
        if (u == null) return 1f;
        if (_critNext.TryGetValue(u, out var v) && v > 1f)
        {
            _critNext.Remove(u);
            return v;
        }
        return 1f;
    }

    public float ApplyLifeSteal(Unit u, int damage)
    {
        if (u == null) return 0f;
        if (_lifeSteal.TryGetValue(u, out var v) && v.stacks > 0 && v.percent > 0f)
        {
            int heal = Mathf.RoundToInt(damage * v.percent);
            v.stacks -= 1;
            if (v.stacks <= 0) _lifeSteal.Remove(u);
            else _lifeSteal[u] = v;
            return heal;
        }
        return 0f;
    }


    public void Tick(UnitServices service, float dt)
    {
        
        var toRemove = new HashSet<Unit>();

        foreach (var kv in _timers)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);

        foreach (var kv in _multiHitSkill)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);
        foreach (var kv in _attackSkill)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);
        foreach (var kv in _attackSpeedSkill)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);
        foreach (var u in _invincibility)
            if (IsGone(u)) toRemove.Add(u);
        foreach (var kv in _lifeSteal)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);
        foreach (var kv in _critNext)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);
        foreach (var kv in _dotAura)
            if (IsGone(kv.Key)) toRemove.Add(kv.Key);

        if (toRemove.Count > 0)
        {
            foreach (var u in toRemove)
                RemoveUnitFromAll(u);
        }

        foreach (var kv in _timers)
        {
            var list = kv.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i]._timeLeft -= dt;
                if (list[i]._timeLeft <= 0f)
                {
                    var end = list[i]._onEnd;
                    list.RemoveAt(i);
                    end?.Invoke();
                }
            }
        }


        var auraKeys = new List<Unit>(_dotAura.Keys);
        foreach (var caster in auraKeys)
        {
            var entry = _dotAura[caster];
            entry.timeLeft -= dt;

            if (entry.timeLeft <= 0f)
            {
                _dotAura.Remove(caster);
                continue;
            }

            _dotAura[caster] = entry;

            if (service == null || service.Perception == null || service.Combat == null) continue;

            float tickDamage = entry.dps * dt;
            if (tickDamage <= 0f) continue;

            var enemies = service.Perception.GetUnitsInRange(
                caster, (int)entry.radius, onlyAllies: false, centerIsCaster: true);

            if (enemies == null) continue;
            foreach (var e in enemies)
            {
                if (IsGone(e)) continue;
                service.Combat.DealDamage(caster, e, Mathf.FloorToInt(tickDamage));
            }
        }
    }

    public void ClearAll(Unit u)
    {
        if (u == null) return;
        RemoveUnitFromAll(u);
    }

    public void ClearAll()
    {
        _timers.Clear();
        _multiHitSkill.Clear();
        _attackSkill.Clear();
        _attackSpeedSkill.Clear();
        _invincibility.Clear();
        _lifeSteal.Clear();
        _critNext.Clear();
        _dotAura.Clear();
    }

    private void RemoveUnitFromAll(Unit u)
    {
        _timers.Remove(u);
        _multiHitSkill.Remove(u);
        _attackSkill.Remove(u);
        _attackSpeedSkill.Remove(u);
        _invincibility.Remove(u);
        _lifeSteal.Remove(u);
        _critNext.Remove(u);
        _dotAura.Remove(u);
    }

    private void AddTimer(Unit u, float duration, Action onEnd)
    {
        if (u == null) return;
        if (duration <= 0f) { onEnd?.Invoke(); return; }

        if (!_timers.TryGetValue(u, out var list))
        {
            list = new List<BuffTimer>();
            _timers[u] = list;
        }
        list.Add(new BuffTimer { _timeLeft = duration, _onEnd = onEnd });
    }
}