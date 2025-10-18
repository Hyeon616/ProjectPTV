using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationSpeed : StateMachineBehaviour
{
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");

    private bool _applied;
    private float _baseDuration;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var unit = animator.GetComponent<Unit>();
        if (unit == null) return;

        var clips = animator.GetCurrentAnimatorClipInfo(layerIndex);
        if (clips == null || clips.Length == 0 || clips[0].clip == null) return;

        // 길이만 캐시. 속도는 절대 여기서 만지지 않음
        unit.SetAttackBaseDuration(clips[0].clip.length);
    }

    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (_applied) return;
    //    _applied = ApplySpeed(animator, layerIndex);
    //}

    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    animator.SetFloat(AttackSpeed, 1f);
    //    _applied = false;
    //    _baseDuration = 0f;
    //}

    private bool ApplySpeed(Animator animator, int layer)
    {
        var unit = animator.GetComponent<Unit>();
        if (unit == null) return false;

        var clips = animator.GetCurrentAnimatorClipInfo(layer);
        if (clips == null || clips.Length == 0 || clips[0].clip == null)
            return false; 

        _baseDuration = clips[0].clip.length;
        unit.SetAttackBaseDuration(_baseDuration); 

        
        float attackInterval = Mathf.Max(unit.UnitState.UnitStats._attackInterval, 0.0001f);

        
        float attackSpeed = Mathf.Clamp(_baseDuration / attackInterval, 0.1f, 5f);
        animator.SetFloat(AttackSpeed, attackSpeed);
        return true;
    }

}
