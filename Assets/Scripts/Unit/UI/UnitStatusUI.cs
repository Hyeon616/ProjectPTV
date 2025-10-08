using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;


public class UnitStatusUI : MonoBehaviour
{
    [SerializeField] private Image _hp;
    [SerializeField] private Image _mp;

    private float _lerpTime = 0.1f;

    private float _hpTarget = 1f;
    private float _mpTarget = 0f;
    private float _hpVel;
    private float _mpVel;

    private void OnEnable()
    {
        if(_hp)
            _hp.fillAmount = _hpTarget;
        if(_mp)
            _mp.fillAmount = _mpTarget;
    }

    void Update()
    {
        if (_hp)
        {
            float cur = _hp.fillAmount;
            float next = Mathf.SmoothDamp(cur, _hpTarget, ref _hpVel, _lerpTime);
            _hp.fillAmount = Mathf.Clamp01(next);
        }
        if (_mp)
        {
            float cur = _mp.fillAmount;
            float next = Mathf.SmoothDamp(cur, _mpTarget, ref _mpVel, _lerpTime);
            _mp.fillAmount = Mathf.Clamp01(next);
        }
    }

    public void SetHpRatio(float ratio, bool instant = false)
    {
        _hpTarget = Mathf.Clamp01(ratio);
        if (instant && _hp)
            _hp.fillAmount = _hpTarget;
    }

    public void SetMpRatio(float ratio, bool instant = false)
    {
        _mpTarget = Mathf.Clamp01(ratio);
        if (instant && _mp) 
            _mp.fillAmount = _mpTarget;
    }

    public void ResetBar(float hpRatio, float mpRatio)
    {
        _hpTarget = Mathf.Clamp01(hpRatio);
        _mpTarget = Mathf.Clamp01(mpRatio);

        if (_hp) 
            _hp.fillAmount = _hpTarget;
        if (_mp) 
            _mp.fillAmount = _mpTarget;

        _hpVel = _mpVel = 0f;
    }

}
