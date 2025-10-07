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

    // °è¼ö
    private Dictionary<Unit, float> _multiHitSkill = new Dictionary<Unit, float>();
    private Dictionary<Unit, float> _attackSkill = new Dictionary<Unit, float>();
    private Dictionary<Unit, float> _attackSpeedSkill = new Dictionary<Unit, float>();

    private HashSet<Unit> _invincibility = new HashSet<Unit>();
    private Dictionary<Unit, (int stacks, float percent)> _lifeSteal = new Dictionary<Unit, (int stacks, float percent)>();
    private Dictionary<Unit, float> _critNext = new Dictionary<Unit, float>();

    private Dictionary<Unit, (float radius, float dps, float timeLeft)> _dotAura = new Dictionary<Unit, (float radius, float dps, float timeLeft)>();

    public void MultiHitSkill(Unit u, float mul, float duration)
    {
        if (!_multiHitSkill.ContainsKey(u))
            _multiHitSkill[u] *= mul;
        AddTimer(u, duration, () =>
        {
            _multiHitSkill[u] /= mul;

            if (Approximately(_multiHitSkill[u], 1f))
                _multiHitSkill.Remove(u);
        });

    }

    public void AttackBuff(Unit u, float mul, float duration)
    {
        if (_attackSkill.ContainsKey(u))
            _attackSkill[u] = 1f;

        _attackSkill[u] *= mul;

        AddTimer(u, duration, () =>
        {
            _attackSkill[u] /= mul;

            if (Approximately(_attackSkill[u], 1f))
                _attackSkill.Remove(u);
        });
    }

    public void AttackSpeedBuff(Unit u, float mul, float duration)
    {
        if (!_attackSpeedSkill.ContainsKey(u))
            _attackSpeedSkill[u] = 1f;

        _attackSpeedSkill[u] *= mul;

        AddTimer(u, duration, () =>
        {
            _attackSpeedSkill[u] /= mul;
            if (Approximately(_attackSpeedSkill[u], 1f))
                _attackSpeedSkill.Remove(u);
        });
    }

    public void SetInvincibility(Unit u, float duration)
    {
        _invincibility.Add(u);
        AddTimer(u, duration, () =>
        {
            _invincibility.Remove(u);
        });
    }

    public void SetNextAttackCritical(Unit u, float critMultiplier)
    {
        _critNext[u] = Mathf.Max(critMultiplier, 1f);
    }

    public void SetLifeSteal(Unit u, int stacks, float percent)
    {
        _lifeSteal[u] = (stacks, Mathf.Clamp01(percent));
    }

    public void StartDotAura(Unit u, float radius, float dps, float duration)
    {
        _dotAura[u] = (radius, dps, duration);
    }

    public float MultiHit(Unit u) => _multiHitSkill.TryGetValue(u, out var v) ? v : 1f;
    public float AttackSkill(Unit u) => _attackSkill.TryGetValue(u, out var v) ? v : 1f;
    public float AttackSpeedSkill(Unit u) => _attackSpeedSkill.TryGetValue(u, out var v) ? v : 1f;
    public bool IsInvincibility(Unit u) => _invincibility.Contains(u);

    public float ConsumeCrit(Unit u)
    {
        if (_critNext.TryGetValue(u, out var v) && v > 1f)
        {
            _critNext.Remove(u);
            return v;
        }
        return 1f;
    }

    public float ApplyLifeSteal(Unit u, int damage)
    {
        if (_lifeSteal.TryGetValue(u, out var v) && v.stacks > 0 && v.percent > 0f)
        {
            int heal = Mathf.RoundToInt(damage * v.percent);
            v.stacks -= 1;
            if (v.stacks <= 0)
                _lifeSteal.Remove(u);
            else
                _lifeSteal[u] = v;

            return heal;
        }
        return 0f;
    }

    public void Tick(UnitServices service, float dt)
    {
        foreach (var kv in _timers)
        {
            var bufftime = kv.Value;
            for (int i = bufftime.Count - 1; i >= 0; i--)
            {
                bufftime[i]._timeLeft -= dt;
                if (bufftime[i]._timeLeft <= 0f)
                {
                    var end = bufftime[i]._onEnd;
                    bufftime.RemoveAt(i);
                    end?.Invoke();
                }
            }
        }

        var temp = new List<Unit>(_dotAura.Keys);
        foreach (var u in temp)
        {
            var entry = _dotAura[u];
            entry.timeLeft -= dt;

            float tickDamage = entry.dps * dt;
            var enemies = service.Perception.GetUnitsInRange(u, (int)entry.radius, onlyAllies: false, centerIsCaster: true);
            foreach (var e in enemies)
            {
                if (tickDamage > 0f)
                {
                    service.Combat.DealDamage(u, e, Mathf.FloorToInt(tickDamage));
                }

            }

            if (entry.timeLeft <= 0f)
                _dotAura.Remove(u);
            else _dotAura[u] = entry;
        }



    }

    private void AddTimer(Unit u, float duration, Action onEnd)
    {
        if (!_timers.ContainsKey(u)) _timers[u] = new List<BuffTimer>();
        _timers[u].Add(new BuffTimer { _timeLeft = duration, _onEnd = onEnd });
    }

    private static bool Approximately(float a, float b) => Mathf.Abs(a - b) < 0.0001f;
}
