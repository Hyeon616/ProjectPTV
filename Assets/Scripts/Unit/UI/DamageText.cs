using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _rt;

    private Sequence _seq;

    public TMP_Text Text => _text;
    public RectTransform RectTransform => _rt;
    public void Init()
    {
        if (!_text)
            _text = GetComponentInChildren<TMP_Text>();

        if (!_rt)
            _rt = GetComponent<RectTransform>();
    }

    public void ResetVisual(float alpha = 1f, float scale = 1f)
    {
        KillTween();
        if (_text)
        {
            var c = _text.color;
            c.a = alpha;
            _text.color = c;
        }
        if (_rt)
            _rt.localScale = new Vector3(scale, scale, 1f);
    }

    public void SetSequence(Sequence seq)
    {
        KillTween();
        _seq = seq;
    }

    public void KillTween()
    {
        if (_seq != null)
        {
            _seq.Kill(false);
            _seq = null;
        }
    }

    private void OnDisable()
    {
        KillTween();
    }

}
