using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitState UnitState { get; private set; }
    public UnitServices Services { get; private set; }

    #region Field Info
    private Tile _spawnTile;
    public Tile CurrentTileRef => _currentTile;
    public Unit TargetRef => _target;

    private float _moveSpeed = 1.5f;
    private float _attackTimer;
    private Unit _target;
    private Tile _currentTile;
    private Tile _nextTile;
    private Tile _movingFromTile;
    private Vector3 _targetWorldPos;
    #endregion

    #region State Boolean
    private bool _isDying;
    private bool _isAttacking;
    private bool _attackEventArmed;
    private bool _queueSkillAfterHit;
    #endregion

    #region Animation
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

    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
    private float _attackBaseDuration = -1f;
    public void SetAttackBaseDuration(float duration) => _attackBaseDuration = duration;


    #endregion

    #region State
    private IUnitState _state;
    private readonly IdleState _idleState = new IdleState();
    private readonly ChaseState _chaseState = new ChaseState();
    private readonly AttackState _attackState = new AttackState();
    private readonly SkillState _skillState = new SkillState();
    private readonly DieState _dieState = new DieState();
    #endregion

    #region UI
    private UnitGradeUI _unitGradeUI;
    private UnitStatusPresenter _unitStatusPresenter;
    #endregion

    #region Property

    public float MoveSpeed => _moveSpeed;
    public float AttackTimer { get => _attackTimer; set => _attackTimer = value; }
    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }
    public bool AttackEventArmed { get => _attackEventArmed; set => _attackEventArmed = value; }
    public bool QueueSkillAfterHit { get => _queueSkillAfterHit; set => _queueSkillAfterHit = value; }
    public bool IsDying { get => _isDying; set => _isDying = value; }

    public Unit Target { get => _target; set => _target = value; }
    public Tile MovingFromTile { get => _movingFromTile; set => _movingFromTile = value; }
    public Tile NextTile { get => _nextTile; set => _nextTile = value; }
    public Vector3 TargetWorldPos { get => _targetWorldPos; set => _targetWorldPos = value; }
    public void CurrentTile(Tile tile) => _currentTile = tile;
    public void SpawnTile(Tile tile) => _spawnTile = tile;
    #endregion

    public void Init(UnitData unitData, Owner owner, int grade, FieldManager fieldManager)
    {

        UnitState = new UnitState(unitData, owner, grade);
        Services = new UnitServices(fieldManager);

        _anim = GetComponent<Animator>();
        _unitGradeUI = GetComponent<UnitGradeUI>();
        _unitStatusPresenter = GetComponent<UnitStatusPresenter>();

        _attackTimer = 0f;
        DefaultDirection();

        _unitGradeUI.Init(UnitState);
        _unitStatusPresenter.Bind(UnitState);
        _unitStatusPresenter.ResetUI(UnitState._currentHp, UnitState.UnitStats._hp, UnitState._currentMp, 100);

        RequestState(UnitActionState.Idle);
    }

    #region State

    public void StateUpdate()
    {

        if (UnitState.IsDead || _isDying)
        {
            if (UnitState.CurrentState != UnitActionState.Die)
                RequestState(UnitActionState.Die);

            _state?.Execute(this);
            return;
        }
        Services.Tick(Time.deltaTime);

        _state?.Execute(this);
    }

    public void RequestState(UnitActionState next)
    {
        if ((UnitState.IsDead || _isDying) && next != UnitActionState.Die)
            return;

        if (UnitState.CurrentState == next && _state != null)
            return;

        _state?.Exit(this);

        UnitState.ChangeState(next);
        switch (next)
        {
            case UnitActionState.Idle: _state = _idleState; break;
            case UnitActionState.Chase: _state = _chaseState; break;
            case UnitActionState.Attack: _state = _attackState; break;
            case UnitActionState.Skill: _state = _skillState; break;
            case UnitActionState.Die: _state = _dieState; break;
        }

        _state?.Enter(this);
    }

    public void SetLocomotionIdle() => _anim.SetInteger(AnimState, (int)UnitActionState.Idle);
    public void SetLocomotionChase() => _anim.SetInteger(AnimState, (int)UnitActionState.Chase);

    #endregion

    #region Direction

    public void DefaultDirection()
    {
        _lookDir = UnitState.Owner == Owner.Player ? new Vector2(0, 1) : new Vector2(0, -1);
        _anim.SetFloat(DirX, _lookDir.x);
        _anim.SetFloat(DirY, _lookDir.y);
    }

    public void UpdateDirection(Tile from, Tile to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;
        Vector2 dir = (Mathf.Abs(dx) > Mathf.Abs(dy))
            ? (dx > 0 ? new Vector2(0, 1) : new Vector2(0, -1))  // X+ = N, X- = S
            : (dy > 0 ? new Vector2(-1, 0) : new Vector2(1, 0));  // Y+ = W, Y- = E
        _anim.SetFloat(DirX, dir.x);
        _anim.SetFloat(DirY, dir.y);
    }

    #endregion

    #region Trigger
    public void TriggerAttack()
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

    public void TriggerSkill()
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

    public void TriggerDie()
    {
        _anim.ResetTrigger(Death);
        _anim.SetTrigger(Death);
    }
    #endregion

    #region AnimationEvent
    public void AnimEvent_AttackImpact()
    {
        if (UnitState.IsDead || _isDying)
            return;

        if (!_attackEventArmed)
            return;

        if (_target == null || _target.UnitState.IsDead)
            return;

        if (!Services.Perception.IsInRange(this, _target))
            return;

        int dmg = Services.Combat.ComputeAttackDamage(this, _target);
        Services.Combat.DealDamage(this, _target, dmg);

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
        if (UnitState.IsDead || _isDying)
            return;

        _isAttacking = false;

        _anim.SetFloat(AttackSpeed, 1f);

        if (_queueSkillAfterHit)
        {
            _queueSkillAfterHit = false;
            RequestState(UnitActionState.Skill);
            TriggerSkill();
        }
    }

    public void AnimEvent_SkillEnd()
    {
        if (UnitState.IsDead || _isDying)
            return;

        UnitState.SpendMp(UnitState._currentMp);

        if (_target != null && !_target.UnitState.IsDead && Services.Perception.IsInRange(this, _target))
        {
            RequestState(UnitActionState.Attack);
            _isAttacking = false;
            _attackEventArmed = false;
            _attackTimer = 0f;
        }
        else
        {
            RequestState(UnitActionState.Chase);
            SetLocomotionChase();
            _isAttacking = false;
            _attackEventArmed = false;
        }
    }

    public void AnimEvent_DeathEnd()
    {
        StartCoroutine(DeathDealy());
    }

    private IEnumerator DeathDealy()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        _currentTile?.ClearUnit();
    }
    #endregion

    public void TakeDamage(int damage)
    {
        if (UnitState.IsDead) return;

        UnitState.TakeDamage(damage);
        if (UnitState.IsDead)
        {
            _isDying = true;

            _isAttacking = false;
            _attackEventArmed = false;
            _queueSkillAfterHit = false;
            _attackTimer = 0f;

            if (_nextTile != null)
            {
                _nextTile.ClearReserve(this);
                _nextTile = null;
            }

            _movingFromTile = null;

            _anim.ResetTrigger(Attack);
            _anim.ResetTrigger(Skill);
            _anim.SetFloat(AttackSpeed, 1f);

            if (_currentTile != null)
                _currentTile.CenterUnit(this);

            Services.Status.ClearAll(this);

            RequestState(UnitActionState.Die);
            TriggerDie();
        }
    }

    public void ResetWave()
    {
        StopAllCoroutines();

        gameObject.SetActive(true);

        _isDying = false;
        _isAttacking = false;
        _attackEventArmed = false;
        _queueSkillAfterHit = false;

        _attackTimer = 0f;
        _movingFromTile = null;
        _nextTile = null;
        _target = null;

        UnitState.ResetForWave();

        if (_spawnTile != null)
        {
            _currentTile?.ClearUnit();
            _currentTile = _spawnTile;
            _spawnTile.SetUnit(this);
            UnitState.PlaceUnit(_spawnTile);
        }

        Services.Status.ClearAll(this);

        _anim.ResetTrigger(Attack);
        _anim.ResetTrigger(Skill);
        _anim.ResetTrigger(Death);
        _anim.SetFloat(AttackSpeed, 1f);
        _attackBaseDuration = -1f;
        _anim.Rebind();
        _anim.Update(0f);
        _anim.SetInteger(AnimState, (int)UnitActionState.Idle);

        DefaultDirection();

        RequestState(UnitActionState.Idle);
    }


}
