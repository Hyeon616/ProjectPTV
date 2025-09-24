using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    public UnitState UnitState { get; private set; }
    private Image[] _stars;
    private Animator _anim;
    private float _attackTimer;

    [Header("anim")]
    private Vector2 _lookDir;
    private static readonly int AnimState = Animator.StringToHash("State");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");


    public void Init(UnitData unitData, Owner owner, int grade)
    {
        UnitState = new UnitState(unitData, owner, grade);
        _anim = GetComponent<Animator>();
        _attackTimer = 0f;

        DefaultDirection();
        InitStarImage();
        UpdateGradeUI();
    }

    private void Update()
    {
        switch (UnitState.CurrentState)
        {
            case UnitActionState.Idle:
                break;
            case UnitActionState.Chase:
                ChaseEnemy();
                break;
            case UnitActionState.Attack:
                AttackEnemy();
                break;
            case UnitActionState.Skill:
                UseSkill();
                break;
            case UnitActionState.Die:
                break;

        }
    }

    private void PlayStateAnim(UnitActionState state)
    {
        _anim.SetInteger(AnimState, (int)state);
    }

    private void DefaultDirection()
    {
        _lookDir = UnitState.Owner == Owner.Player ? new Vector2(0, 1) : new Vector2(0, -1);
        _anim.SetFloat(DirX, _lookDir.x);
        _anim.SetFloat(DirY, _lookDir.y);

    }

    private void ChaseEnemy()
    {
        // TODO 목표 까지 이동 후 Attack하기


    }

    private void AttackEnemy()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= UnitState.UnitStats._attackInterval)
        {
            _attackTimer = 0f;

            // TODO 적 HP 감소


            UnitState.GainMp(UnitState.UnitStats._increaseMp);

            if (UnitState._currentMp >= 100)
            {
                UnitState.ChangeState(UnitActionState.Skill);
                PlayStateAnim(UnitActionState.Skill);
            }

        }


    }

    private void UseSkill()
    {
        UnitState._currentMp = 0;
        UnitState.ChangeState(UnitActionState.Attack);
        PlayStateAnim(UnitActionState.Attack);

    }

    public void Die()
    {
        UnitState.ChangeState(UnitActionState.Die);
        PlayStateAnim(UnitActionState.Die);
    }

    private void UpdateGradeUI()
    {
        foreach (var star in _stars)
            star.enabled = false;

        for (int i = 0; i < UnitState.CurrentGrade && i < _stars.Length; i++)
            _stars[i].enabled = true;

    }

    private void InitStarImage()
    {
        Transform starRoot = transform.Find("Canvas/Star");
        _stars = new Image[3];

        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i] = starRoot.GetChild(i).GetComponent<Image>();
        }
    }


}
