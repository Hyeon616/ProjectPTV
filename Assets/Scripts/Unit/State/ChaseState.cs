using UnityEngine;

public class ChaseState : IUnitState
{

    public void Enter(Unit u)
    {
        u.SetLocomotionChase();
    }

    public void Execute(Unit u)
    {
        // 0) 타겟 유효성
        if (u.Target == null || u.Target.UnitState.IsDead)
        {
            var t = u.Services.Perception.FindTarget(u);
            u.Target = t;
            if (t == null)
            {
                u.RequestState(UnitActionState.Idle);
                return;
            }
        }
        if (u.CurrentTileRef == null || u.Target.CurrentTileRef == null)
            return;

        // 1) 사거리 안 + 이동중 아님 → 즉시 공격
        if (u.Services.Perception.IsInRange(u, u.Target) && u.MovingFromTile == null)
        {
            if (u.NextTile != null) { u.NextTile.ClearReserve(u); u.NextTile = null; }

            u.CurrentTileRef.CenterUnit(u);
            u.UpdateDirection(u.CurrentTileRef, u.Target.CurrentTileRef);

            u.RequestState(UnitActionState.Attack);
            u.TriggerAttack();
            u.IsAttacking = true;
            u.AttackEventArmed = true;
            u.AttackTimer = 0f;
            return;
        }

        // 2) 다음 스텝 탐색(있으면 즉시 이동 시작)
        Tile candidateStep = null;
        if (u.MovingFromTile == null && u.NextTile == null)
            candidateStep = u.Services.Navigator.FindNextTileTowardTarget(u);

        if (u.MovingFromTile == null && u.NextTile == null && candidateStep != null)
        {
            candidateStep.ReserveTile(u);

            u.MovingFromTile = u.CurrentTileRef;
            u.MovingFromTile?.ClearUnit();

            candidateStep.SetUnit(u);
            u.UnitState.PlaceUnit(candidateStep);
            u.CurrentTile(candidateStep);
            u.NextTile = candidateStep;

            if (u.MovingFromTile != null)
            {
                u.transform.SetParent(u.MovingFromTile.transform);
                u.transform.localPosition = u.MovingFromTile.CenterLocal;
            }

            u.TargetWorldPos = u.CurrentTileRef.CenterWorld;
            u.UpdateDirection(u.MovingFromTile ?? u.CurrentTileRef, u.CurrentTileRef);

            u.SetLocomotionChase();
        }
        else if (u.MovingFromTile == null && u.NextTile == null)
        {
            // 3) 이동 불가 시 '정면-동일 사거리-사거리+1' 홀드
            int dx = Mathf.Abs(u.CurrentTileRef.X - u.Target.CurrentTileRef.X);
            int dy = Mathf.Abs(u.CurrentTileRef.Y - u.Target.CurrentTileRef.Y);
            int md = dx + dy;

            int myRange = u.UnitState.UnitStats._attackRange;
            int enemyRange = u.Target.UnitState.UnitStats._attackRange;

            bool sameRange = (myRange == enemyRange);
            bool frontAlign = (dx == 0 || dy == 0);
            bool atRangePlus = (md == myRange + 1);

            if (sameRange && frontAlign && atRangePlus)
            {
                u.SetLocomotionIdle();
                return;
            }

            u.SetLocomotionChase();
        }

        // 4) 시각 이동 보간
        if (u.MovingFromTile != null)
        {
            float step = u.MoveSpeed * Time.deltaTime;
            float dist = Vector3.Distance(u.transform.position, u.TargetWorldPos);

            if (dist <= step)
            {
                u.transform.position = u.TargetWorldPos;
                u.transform.SetParent(u.CurrentTileRef.transform);
                u.CurrentTileRef.CenterUnit(u);

                u.CurrentTileRef.ClearReserve(u);

                u.SetLocomotionChase();
                u.MovingFromTile = null;
                u.NextTile = null;
                return;
            }
            else
            {
                u.transform.position = Vector3.MoveTowards(u.transform.position, u.TargetWorldPos, step);
                u.SetLocomotionChase();
                return;
            }
        }

        u.SetLocomotionChase();
    }
    public void Exit(Unit u)
    {

    }
}
