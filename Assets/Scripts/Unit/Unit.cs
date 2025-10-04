using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    public UnitState UnitState { get; private set; }

    private FieldManager _fieldManager;

    [Header("Move/Attack")]
    private float _moveSpeed = 1.5f;
    private float _attackTimer;
    private Unit _target;
    private Tile _currentTile;
    private Tile _nextTile;
    private Tile _movingFromTile;                 
    private Vector3 _targetWorldPos;             
    private static readonly Vector3 LocalCenter = new Vector3(0f, 0.25f, 0f);

    private bool _isDying;
    private bool _isAttacking;     
    private bool _attackEventArmed;  
    private bool _queueSkillAfterHit; 

    [Header("Reset")]
    private Tile _spawnTile;

    [Header("anim")]
    private Animator _anim;
    private Vector2 _lookDir;
    private static readonly int AnimState = Animator.StringToHash("State");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Skill = Animator.StringToHash("Skill");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int AttackStateHash = Animator.StringToHash("Attack");
    private static readonly int SkillStateHash = Animator.StringToHash("Skill");
    private static readonly int DeathStateHash = Animator.StringToHash("Death");

    private UnitGradeUI _unitGradeUI;

    public void Init(UnitData unitData, Owner owner, int grade, FieldManager fieldManager)
    {
        UnitState = new UnitState(unitData, owner, grade);
        _fieldManager = fieldManager;
        _anim = GetComponent<Animator>();
        _unitGradeUI = GetComponent<UnitGradeUI>();
        _attackTimer = 0f;

        DefaultDirection();
        _unitGradeUI.Init(UnitState);
    }

    public void StateUpdate()
    {

        switch (UnitState.CurrentState)
        {
            case UnitActionState.Idle:
                FindTarget();
                break;
            case UnitActionState.Chase:
                ChaseState();
                break;
            case UnitActionState.Attack:
                AttackState();
                break;
            case UnitActionState.Skill:
                SkillState();
                break;
            case UnitActionState.Die:
                DieState();
                break;

        }
    }


    private void SetLocomotionIdle() => _anim.SetInteger(AnimState, (int)UnitActionState.Idle);
    private void SetLocomotionChase() => _anim.SetInteger(AnimState, (int)UnitActionState.Chase);



    public void DefaultDirection()
    {
        _lookDir = UnitState.Owner == Owner.Player ? new Vector2(0, 1) : new Vector2(0, -1);
        _anim.SetFloat(DirX, _lookDir.x);
        _anim.SetFloat(DirY, _lookDir.y);

    }

    private void UpdateDirection(Tile from, Tile to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        Vector2 dir;
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            dir = dx > 0 ? new Vector2(0, 1) : new Vector2(0, -1);  // X+ = N, X- = S
        else
            dir = dy > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);   // Y+ = W, Y- = E

        _anim.SetFloat(DirX, dir.x);
        _anim.SetFloat(DirY, dir.y);
    }

    private void TriggerAttack()
    {
        var st = _anim.GetCurrentAnimatorStateInfo(0);
        if (st.shortNameHash == AttackStateHash)
        {
            _anim.Play(AttackStateHash, 0, 0f); 
            return;
        }
        _anim.ResetTrigger(Attack);
        _anim.SetTrigger(Attack);
    }

    private void TriggerSkill()
    {
        var st = _anim.GetCurrentAnimatorStateInfo(0);
        if (st.shortNameHash == SkillStateHash)
        {
            _anim.Play(SkillStateHash, 0, 0f);
            return;
        }
        _anim.ResetTrigger(Skill);
        _anim.SetTrigger(Skill);
    }

    private void TriggerDie()
    {
        _anim.ResetTrigger(Death);
        _anim.SetTrigger(Death);
    }

    public void CurrentTile(Tile tile)
    {
        _currentTile = tile;
    }

    private void ChaseState()
    {
        // 0) 타겟 유효성
        if (_target == null || _target.UnitState.IsDead)
        {
            FindTarget();
            return;
        }
        if (_currentTile == null || _target._currentTile == null)
            return;

        // 1) 사거리 안이면 즉시 공격(이동 중이 아닐 때만)
        if (IsInRange(_target) && _movingFromTile == null)
        {
            if (_nextTile != null) { _nextTile.ClearReserve(this); _nextTile = null; }

            // 중앙 스냅 후 방향/공격 트리거
            transform.SetParent(_currentTile.transform);
            transform.localPosition = new Vector3(0f, 0.25f, 0f);

            UpdateDirection(_currentTile, _target._currentTile);

            UnitState.ChangeState(UnitActionState.Attack);
            TriggerAttack();
            _isAttacking = true;
            _attackEventArmed = true;
            _attackTimer = 0f;
            return;
        }

        // 2) 먼저 경로 후보를 받아본다 (있으면 그걸로 이동 시작)
        Tile candidateStep = null;
        if (_movingFromTile == null && _nextTile == null)
            candidateStep = FindNextTileTowardTarget();

        if (_movingFromTile == null && _nextTile == null && candidateStep != null)
        {
            // (a) 다음 타일 예약
            candidateStep.ReserveTile(this);

            // (b) 시각 출발 타일 고정
            _movingFromTile = _currentTile;

            // (c) 출발 타일 논리 점유 해제
            _movingFromTile?.ClearUnit();

            // (d) 다음 타일을 **즉시 논리 점유** (사거리/경로/대치 판정은 여기 기준)
            candidateStep.SetUnit(this);
            UnitState.PlaceUnit(candidateStep);
            _currentTile = candidateStep;
            _nextTile = candidateStep;

            // (e) 부모/로컬 위치는 출발 타일 기준으로 고정(부드럽게 이동)
            if (_movingFromTile != null)
            {
                transform.SetParent(_movingFromTile.transform);
                transform.localPosition = new Vector3(0f, 0.25f, 0f);
            }

            // (f) 월드 타겟 좌표 캐시 + 방향 갱신
            _targetWorldPos = _currentTile.transform.position + new Vector3(0f, 0.25f, 0f);
            UpdateDirection(_movingFromTile ?? _currentTile, _currentTile);
        }
        else if (_movingFromTile == null && _nextTile == null)
        {
            // 3) 이동할 후보가 "정말" 없을 때만 정면-동일사거리-사거리+1 홀드 적용
            int dx = Mathf.Abs(_currentTile.X - _target._currentTile.X);
            int dy = Mathf.Abs(_currentTile.Y - _target._currentTile.Y);
            int md = dx + dy;

            int myRange = UnitState.UnitStats._attackRange;
            int enemyRange = _target.UnitState.UnitStats._attackRange;

            bool sameRange = (myRange == enemyRange);
            bool frontAlign = (dx == 0 || dy == 0);   // 상하좌우 직선
            bool atRangePlus = (md == myRange + 1);

            if (sameRange && frontAlign && atRangePlus)
            {
                // 여기서 멈춰 적이 들어오면 내가 선공
                SetLocomotionIdle();
                return;
            }

            // 위 조건도 아니고 후보도 없으면 그냥 체이스 포즈 유지
            SetLocomotionChase();
        }

        // 4) 실제 이동 처리(시각적 보간)
        if (_movingFromTile != null)
        {
            float step = _moveSpeed * Time.deltaTime;
            float dist = Vector3.Distance(transform.position, _targetWorldPos);

            if (dist <= step)
            {
                // 도착: 부모를 논리 타일로 맞추고 중앙 스냅, 예약 해제
                transform.position = _targetWorldPos;
                transform.SetParent(_currentTile.transform);
                transform.localPosition = new Vector3(0f, 0.25f, 0f);

                _currentTile.ClearReserve(this);

                _movingFromTile = null;
                _nextTile = null;
                return;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetWorldPos, step);
                return;
            }
        }

        // 5) 그 외엔 이동 애니메이션 유지
        SetLocomotionChase();
    }

    public void FindTarget()
    {
        _target = FindNearestEnemy();

        if (_target != null)
        {
            if (IsInRange(_target))
            {
                UnitState.ChangeState(UnitActionState.Attack);
                TriggerAttack();                     
                _isAttacking = true;
                _attackEventArmed = true;
                _attackTimer = 0f;
            }
            else
            {
                UnitState.ChangeState(UnitActionState.Chase);
                SetLocomotionChase();              
            }
        }
        else
        {
            UnitState.ChangeState(UnitActionState.Idle);
            SetLocomotionIdle();                 
        }
    }

    // 타겟 기준으로 candidate가 어느 "면"에 있는지 평가: S=0, E=1, W=2, N=3
    // Owner 별 선호 순서: Player = S(0)→E(1)→W(2)→N(3), Enemy = N(3)→E(1)→W(2)→S(0)
    private int DirectionPriority(Tile candidate, Tile target, Owner owner)
    {
        int dx = candidate.X - target.X; // x+:N, x-:S
        int dy = candidate.Y - target.Y; // y+:W, y-:E

        // candidate가 어느 면에 "더 가깝게" 위치하는지 결정
        int side; // 0:S, 1:E, 2:W, 3:N
        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
        {
            // 수직 성분이 더 크면 N/S 쪽
            side = (dx < 0) ? 0 /*S*/ : 3 /*N*/;
        }
        else
        {
            // 수평 성분이 더 크면 E/W 쪽
            side = (dy < 0) ? 1 /*E*/ : 2 /*W*/;
        }

        // 선호 순서 매핑
        // 값이 작을수록 우선
        switch (owner)
        {
            case Owner.Player:
                // S(0)→E(1)→W(2)→N(3)
                if (side == 0) return 0;
                if (side == 1) return 1;
                if (side == 2) return 2;
                return 3; // N
            case Owner.Enemy:
                // N(3)→E(1)→W(2)→S(0)
                if (side == 3) return 0;
                if (side == 1) return 1;
                if (side == 2) return 2;
                return 3; // S
            default:
                return 10; // 안전장치
        }
    }

    private Tile FindNextTileTowardTarget()
    {
        if (_target == null) return null;
        if (!(_target.UnitState.CurrentSlot is Tile targetTile)) return null;
        if (_currentTile == null) return null;

        int rows = _fieldManager.Rows;
        int cols = _fieldManager.Cols;
        int range = UnitState.UnitStats._attackRange;

        // 1) BFS로 모든 타일까지의 최단거리 계산
        bool[,] visited = new bool[rows, cols];
        int[,] dist = new int[rows, cols];
        for (int x = 0; x < rows; x++)
            for (int y = 0; y < cols; y++)
                dist[x, y] = int.MaxValue;

        Tile[,] parent = new Tile[rows, cols];
        Queue<Tile> q = new Queue<Tile>();

        visited[_currentTile.X, _currentTile.Y] = true;
        dist[_currentTile.X, _currentTile.Y] = 0;
        q.Enqueue(_currentTile);

        int[] dx4 = { 1, -1, 0, 0 };
        int[] dy4 = { 0, 0, 1, -1 };

        while (q.Count > 0)
        {
            Tile cur = q.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = cur.X + dx4[i];
                int ny = cur.Y + dy4[i];

                if (nx < 0 || nx >= rows || ny < 0 || ny >= cols) continue;
                if (visited[nx, ny]) continue;

                Tile nxt = _fieldManager.GetTile(nx, ny);
                // BFS는 "통과 가능"한 타일만 확장 (자신 or 비어있거나 자신이 예약 OK)
                if (!nxt.IsFreeFor(this)) continue;

                visited[nx, ny] = true;
                dist[nx, ny] = dist[cur.X, cur.Y] + 1;
                parent[nx, ny] = cur;
                q.Enqueue(nxt);
            }
        }

        // 2) "사거리 이내 & 점유 가능" 타일들을 후보로 수집
        int bestDist = int.MaxValue;
        int bestDirScore = int.MaxValue;
        Tile bestGoal = null;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                // dist가 유효하고, 그 타일에서 타겟까지 사거리 이내
                if (dist[x, y] == int.MaxValue) continue;

                int mdToTarget = Mathf.Abs(x - targetTile.X) + Mathf.Abs(y - targetTile.Y);
                if (mdToTarget > range) continue;

                Tile cand = _fieldManager.GetTile(x, y);
                if (!cand.IsFreeFor(this)) continue; // 최종 도착 시점에도 점유 가능해야 함

                int d = dist[x, y];
                if (d < bestDist)
                {
                    bestDist = d;
                    bestGoal = cand;
                    bestDirScore = DirectionPriority(cand, targetTile, UnitState.Owner);
                }
                else if (d == bestDist)
                {
                    // 최단거리 동점이면 타겟 '면' 우선순위로 tie-break
                    int dirScore = DirectionPriority(cand, targetTile, UnitState.Owner);
                    if (dirScore < bestDirScore)
                    {
                        bestDirScore = dirScore;
                        bestGoal = cand;
                    }
                }
            }
        }

        if (bestGoal == null) return null;
        if (bestGoal == _currentTile) return null;

        // 3) bestGoal까지의 "첫 한 칸" 되짚기
        Tile step = bestGoal;
        Tile prev = parent[step.X, step.Y];
        while (prev != null && prev != _currentTile)
        {
            step = prev;
            prev = parent[step.X, step.Y];
        }
        return step;
    }

    private Unit FindNearestEnemy()
    {
        Unit nearest = null;
        int minDist = int.MaxValue;

        foreach (var tile in _fieldManager.GetAllUnits())
        {
            Unit enemy = tile.Unit;

            if (enemy == null) continue;

            if (enemy.UnitState.Owner == UnitState.Owner) continue;
            if (enemy.UnitState.IsDead) continue;

            if (!(UnitState.CurrentSlot is Tile myTile)) continue;
            if (!(enemy.UnitState.CurrentSlot is Tile enemyTile)) continue;

            int dx = Mathf.Abs(myTile.X - enemyTile.X);
            int dy = Mathf.Abs(myTile.Y - enemyTile.Y);
            int dist = dx + dy;

            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private bool IsInRange(Unit target)
    {
        if (_currentTile == null || target == null || target._currentTile == null)
            return false;

        int md = Mathf.Abs(_currentTile.X - target._currentTile.X) + Mathf.Abs(_currentTile.Y - target._currentTile.Y);
        return md <= UnitState.UnitStats._attackRange;
    }

    private void AttackState()
    {
        if (_movingFromTile != null)
        {
            UnitState.ChangeState(UnitActionState.Chase);
            SetLocomotionChase();
            _isAttacking = false;
            _attackEventArmed = false;
            return;
        }

        if (_target == null || _target.UnitState.IsDead)
        {
            UnitState.ChangeState(UnitActionState.Chase);
            SetLocomotionChase();
            _isAttacking = false;
            _attackEventArmed = false;
            return;
        }

        if (!IsInRange(_target))
        {
            UnitState.ChangeState(UnitActionState.Chase);
            SetLocomotionChase();
            _isAttacking = false;
            _attackEventArmed = false;
            _nextTile = null;
            return;
        }

        if (_currentTile != null)
        {
            var center = _currentTile.transform.position + LocalCenter;
            transform.position = center;
            transform.SetParent(_currentTile.transform);
        }

        if (_target._currentTile != null && _currentTile != null)
            UpdateDirection(_currentTile, _target._currentTile);

        if (!_isAttacking)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= UnitState.UnitStats._attackInterval)
            {
                _attackTimer = 0f;
                _isAttacking = true;
                _attackEventArmed = true;
                TriggerAttack();
            }
        }
    }

    public void AnimEvent_AttackImpact()
    {
        if (!_attackEventArmed) return;
        if (_target == null || _target.UnitState.IsDead) return;
        if (!IsInRange(_target)) return;

        _target.TakeDamage(UnitState.UnitStats._attack);
        UnitState.GainMp(UnitState.UnitStats._increaseMp);

        if (UnitState._currentMp >= 100)
        {
            UnitState._currentMp = 100;
            _queueSkillAfterHit = true; 
        }

        _attackEventArmed = false;
    }

    public void AnimEvent_AttackEnd()
    {
        _isAttacking = false;

        if (_queueSkillAfterHit)
        {
            _queueSkillAfterHit = false;
            UnitState.ChangeState(UnitActionState.Skill);
            TriggerSkill();            
            return;
        }

    }

    public void AnimEvent_SkillEnd()
    {
        UnitState._currentMp = 0;

        if (_target != null && !_target.UnitState.IsDead && IsInRange(_target))
        {
            UnitState.ChangeState(UnitActionState.Attack);
            _isAttacking = false;
            _attackEventArmed = false;
            _attackTimer = 0f;
        }
        else
        {
            UnitState.ChangeState(UnitActionState.Chase);
            SetLocomotionChase();
            _isAttacking = false;
            _attackEventArmed = false;
        }
    }

    public void TakeDamage(int damage)
    {
        UnitState.TakeDamage(damage);
        if (UnitState.IsDead)
        {
            UnitState.ChangeState(UnitActionState.Die);
            TriggerDie();
        }
    }


    private void SkillState()
    {
        //UnitState.UseSkill();


    }

    public void AnimEvent_DeathEnd()
    {
        StartCoroutine(DeathDealy());
    }

    public void DieState()
    {
        if (_isDying) return;
        _isDying = true;
        TriggerDie();
    }

    private IEnumerator DeathDealy()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        _currentTile?.ClearUnit();
    }

    public void SpawnTile(Tile tile)
    {
        _spawnTile = tile;
    }

    public void ResetWave()
    {
        gameObject.SetActive(true);
        _isDying = false;
        UnitState.ResetForWave();

        if (_spawnTile != null)
        {
            _currentTile?.ClearUnit();
            _currentTile = _spawnTile;
            _spawnTile.SetUnit(this);
            UnitState.PlaceUnit(_spawnTile);

        }

        DefaultDirection();
    }


}
