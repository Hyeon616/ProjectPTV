using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IUnitState
{
    public void Enter(Unit u)
    {
        if (u.IsDying) 
            return;

        u.IsDying = true;
        u.TriggerDie();
    }

    public void Execute(Unit u)
    {

    }

    public void Exit(Unit u)
    {
        
    }

}
