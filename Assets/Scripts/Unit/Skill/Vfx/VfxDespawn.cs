using System;
using UnityEngine;

public class VfxDespawn : MonoBehaviour
{
    public float _fallbacktime = 5f;
    private float _timeLeft;

    public bool _restartOnEnable = true;

    public string _entryState = "";

    public SpriteRenderer _sorting;

    public int _sortingOffset = 1;

    private void OnEnable()
    {
        _timeLeft = _fallbacktime;

        var particle = ComputeParticleMaxLifetime();
        _timeLeft = Mathf.Max( _timeLeft, particle);

        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            if (_restartOnEnable && !string.IsNullOrEmpty(_entryState))
            {
                anim.Update(0);
                anim.Play(_entryState, 0, 0f);
                anim.Update(0);

            }

            var st = anim.GetCurrentAnimatorStateInfo(0);
            float speed = (anim.speed != 0f) ? anim.speed : 1f;
            float clipLen = st.length / speed;

            if (!st.loop)
                _timeLeft = Mathf.Max(_timeLeft, clipLen);
            else
                _timeLeft = Mathf.Max(_timeLeft, _fallbacktime);
            
            if(_sorting != null)
            {
                var my = GetComponentInChildren<SpriteRenderer>();
                if(my != null)
                {
                    my.sortingLayerID = _sorting.sortingLayerID;
                    my.sortingOrder = _sorting.sortingOrder + _sortingOffset;
                }

            }

        }

    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
            gameObject.SetActive(false);
    }

    private float ComputeParticleMaxLifetime()
    {
        float maxT = 0f;
        var particle = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var p in particle)
        {
            var m = p.main;
            float duration = m.duration;
            float startLife = MaxLifetime(m.startLifetime);
            maxT = Mathf.Max(maxT, duration + startLife);
        }
        return maxT;
    }

    private float MaxLifetime(ParticleSystem.MinMaxCurve c)
    {
        float max = Mathf.Max(c.constant, c.constantMax);
        if (max <= 0f)
            max = _fallbacktime*0.2f;
        return max;
    }
}
