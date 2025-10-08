using UnityEngine;

public class AttackState : IUnitState
{
    
    public void Enter(Unit u)
    {
        
        var t = u.Target ?? u.Services.Perception.FindTarget(u);
        u.Target = t;

        u.CurrentTileRef.CenterUnit(u);

        if (t != null && u.Services.Perception.IsInRange(u, t))
        {
            u.IsAttacking = true;
            u.AttackEventArmed = true;
            u.AttackTimer = 0f;
            u.TriggerAttack();
        }
        else
        {
            u.RequestState(UnitActionState.Chase);
        }
    }

    public void Execute(Unit u)
    {
        if (u.MovingFromTile != null)
        {
            u.RequestState(UnitActionState.Chase);
            u.SetLocomotionChase();
            u.IsAttacking = false;
            u.AttackEventArmed = false;
            return;
        }

        var t = u.Target ?? u.Services.Perception.FindTarget(u);
        u.Target = t;

        if (t == null || t.UnitState.IsDead)
        {
            u.RequestState(UnitActionState.Chase);
            u.SetLocomotionChase();
            u.IsAttacking = false;
            u.AttackEventArmed = false;
            return;
        }

        if (!u.Services.Perception.IsInRange(u, t))
        {
            u.RequestState(UnitActionState.Chase);
            u.SetLocomotionChase();
            u.IsAttacking = false;
            u.AttackEventArmed = false;
            u.NextTile = null;
            return;
        }

        u.CurrentTileRef.CenterUnit(u);
        if (t.CurrentTileRef != null && u.CurrentTileRef != null)
            u.UpdateDirection(u.CurrentTileRef, t.CurrentTileRef);

        if (!u.IsAttacking)
        {
            u.AttackTimer += Time.deltaTime;
            if (u.AttackTimer >= u.UnitState.UnitStats._attackInterval)
            {
                u.AttackTimer = 0f;
                u.IsAttacking = true;
                u.AttackEventArmed = true;
                u.TriggerAttack();
            }
        }
    }

    public void Exit(Unit u)
    {
        u.IsAttacking = false;
        u.AttackEventArmed = false;
    }
}
