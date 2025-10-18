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
            u.TriggerAttack();
        }
        else
        {
            u.RequestState(UnitActionState.Chase);
        }
    }

    public void Execute(Unit u)
    {

        var t = u.Target ?? u.Services.Perception.FindTarget(u);
        u.Target = t;

        if (u.IsAttacking)
        {
            if (t == null || t.UnitState.IsDead)
            {
                return;
            }

            if (u.CurrentTileRef != null) u.CurrentTileRef.CenterUnit(u);
            if (t.CurrentTileRef != null && u.CurrentTileRef != null)
                u.UpdateDirection(u.CurrentTileRef, t.CurrentTileRef);

            return; 
        }

        if (t == null || t.UnitState.IsDead)
        {
            u.RequestState(UnitActionState.Chase);
            u.SetLocomotionChase();
            return;
        }

        if (!u.Services.Perception.IsInRange(u, t))
        {
            u.RequestState(UnitActionState.Chase);
            u.SetLocomotionChase();
            u.NextTile = null;
            return;
        }

        if (u.CurrentTileRef != null) u.CurrentTileRef.CenterUnit(u);
        if (t.CurrentTileRef != null && u.CurrentTileRef != null)
            u.UpdateDirection(u.CurrentTileRef, t.CurrentTileRef);

        u.AttackTimer += Time.deltaTime;
        if (u.AttackTimer >= u.UnitState.UnitStats._attackInterval)
        {
            u.AttackTimer = 0f;
            u.TriggerAttack(); 
        }

    }

    public void Exit(Unit u)
    {
        u.IsAttacking = false;
        u.AttackEventArmed = false;
    }
}
