using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IUnitState
{
    public void Enter(Unit u)
    {
        u.SetLocomotionIdle();
    }

    public void Execute(Unit u)
    {
        var t = u.Services.Perception.FindTarget(u);
        u.Target = t;

        if (t != null)
        {
            if (u.Services.Perception.IsInRange(u, t))
                u.RequestState(UnitActionState.Attack);
            else
                u.RequestState(UnitActionState.Chase);
        }
        
    }
    public void Exit(Unit u)
    {
        
    }

}
