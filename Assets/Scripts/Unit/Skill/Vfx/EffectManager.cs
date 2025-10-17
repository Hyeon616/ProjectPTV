using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private RectTransform _effectCanvas;
    [SerializeField] private GameObject _upgradeEffect;

    [Header("Å¸°Ý")]
    public GameObject _hitFx;

    [Header("Archer")]
    public GameObject _ArcherSkill;

    [Header("CamoArcher")]
    public GameObject _camoArcherBuff;

    [Header("DarkLord")]
    public GameObject _darkLordAura;

    [Header("DeathKnight")]
    public GameObject _deathKnightBuff;

    [Header("Knight")]
    public GameObject _knightBuff;

    [Header("LongBow")]
    public GameObject _longBowLineShot;

    [Header("Mage")]
    public GameObject _mageFireball;

    [Header("Paladin")]
    public GameObject _paladinHeal;

    [Header("Wizard")]
    public GameObject _wizardMeteor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    #region Upgrade
    public void PlayUpgradeEffect(List<Transform> startUnits, Transform targetUnit, Action onComplete)
    {
        StartCoroutine(CheckUnitEffects(startUnits, targetUnit, onComplete));
    }

    private IEnumerator CheckUnitEffects(List<Transform> startUnits, Transform targetUnit, Action onComplete)
    {
        int completed = 0;
        int total = startUnits.Count;

        foreach (var start in startUnits)
        {
            StartCoroutine(UpgradeEffect(start, targetUnit, () =>
            {
                completed++;
            }));
        }

        while (completed < total)
        {
            yield return null;
        }

        onComplete?.Invoke();
    }

    private IEnumerator UpgradeEffect(Transform startTransform, Transform targetTransform, Action onComplete)
    {
        GameObject effect = Instantiate(_upgradeEffect, _effectCanvas);
        RectTransform rect = effect.GetComponent<RectTransform>();

        Vector2 startAnchored = GetAnchoredPosFromTransform(startTransform);
        Vector2 targetAnchored = GetAnchoredPosFromTransform(targetTransform);

        float elapsed = 0f;
        float duration = 0.6f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            rect.anchoredPosition = Vector2.Lerp(startAnchored, targetAnchored, t);

            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            rect.localScale = Vector3.one * scale;

            yield return null;
        }

        Destroy(effect);
        onComplete?.Invoke();
    }

    private Vector2 GetAnchoredPosFromTransform(Transform unitTransform)
    {
        Vector2 screenPos = Vector2.zero;

        if (unitTransform.TryGetComponent<RectTransform>(out var rect))
        {
            screenPos = RectTransformUtility.WorldToScreenPoint(null, rect.position);
        }
        else
        {
            screenPos = Camera.main.WorldToScreenPoint(unitTransform.position);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_effectCanvas, screenPos, null, out Vector2 localPoint);

        return localPoint;

    }
    #endregion
}
