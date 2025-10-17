using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct DamageTextStyle
{
    public Color32 normalColor;
    public Color32 critColor;

    public Vector2 endOffset;

    public float jumpPower;

    public float baseScale;
    public float critScale;
}

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private Canvas _effectCanvas;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private DamageTextPool _pool;
    [SerializeField]
    private DamageTextStyle _playerStyle = new DamageTextStyle
    {
        normalColor = new Color32(255, 143, 0, 255),
        critColor = new Color32(255, 82, 0, 255),
        endOffset = new Vector2(+45f, -20f),
        jumpPower = 30f,
        baseScale = 1f,
        critScale = 2f
    };
    [SerializeField]
    private DamageTextStyle _enemyStyle = new DamageTextStyle
    {
        normalColor = new Color32(51, 140, 255, 255),
        critColor = new Color32(26, 90, 255, 255),
        endOffset = new Vector2(-45f, -20f),
        jumpPower = 30f,
        baseScale = 1f,
        critScale = 2f
    };

    private RectTransform _rt;

    private Ease _moveEase = Ease.OutQuad;
    private Ease _fadeEase = Ease.OutQuad;
    private Ease _scaleEase = Ease.OutSine;

    private Vector3 _worldOffset = new Vector3(0, 0.1f, 0);
    private bool _useUnscaledTime = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _rt = _effectCanvas.transform as RectTransform;

        _pool.PoolCreate();
    }

    public void ShowDamage(Unit attacker, Unit target, int amount, bool isCrit)
    {
        if (!attacker || !target || target.UnitState.IsDead) return;

        Vector3 wp = target.transform.position + _worldOffset;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(_mainCamera, wp);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, screen, null, out var startLocal);

        bool playerHitsEnemy =
            attacker.UnitState.Owner == Owner.Player &&
            target.UnitState.Owner == Owner.Enemy;

        var style = playerHitsEnemy ? _playerStyle : _enemyStyle;

        var dt = _pool.Get(_rt);

        dt.Text.enableWordWrapping = false;
        dt.Text.alignment = TextAlignmentOptions.Center;
        dt.Text.color = isCrit ? style.critColor : style.normalColor;
        dt.Text.SetText(amount.ToString());

        float startScale = isCrit ? style.critScale : style.baseScale;
        float endScale = startScale * 0.9f;
        dt.RectTransform.anchoredPosition = startLocal;
        dt.ResetVisual(alpha: 1f, scale: startScale);

        float holdTime = 0.30f;
        float travelTime = 0.90f;

        float fadeStartPct = 0.60f;
        float fadeDur = 0.35f;

        Vector2 endOffset = style.endOffset;
        if (endOffset.y >= 0f) endOffset.y = -Mathf.Abs(endOffset.y);
        float jumpPower = style.jumpPower;

        Vector2 endLocal = startLocal + endOffset;

        var seq = DOTween.Sequence()
            .SetUpdate(_useUnscaledTime)
            .SetRecyclable(true)
            .SetLink(dt.gameObject);

        seq.AppendInterval(holdTime);

        seq.Append(
            dt.RectTransform.DOJumpAnchorPos(
                endValue: endLocal,
                jumpPower: jumpPower,
                numJumps: 1,
                duration: travelTime
            ).SetEase(_moveEase)
        );

        seq.Insert(
            holdTime,
            dt.RectTransform.DOScale(endScale, travelTime).SetEase(_scaleEase)
        );

        float fadeDelayFromStart = holdTime + travelTime * fadeStartPct;
        seq.Insert(
            fadeDelayFromStart,
            dt.Text.DOFade(0f, fadeDur).SetEase(_fadeEase)
        );

        seq.OnComplete(() => _pool.Release(dt));

        dt.SetSequence(seq);
        seq.Play();
    }

}
