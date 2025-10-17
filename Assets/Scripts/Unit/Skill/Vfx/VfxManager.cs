using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class VfxManager
{
    private VfxPool _pool = new VfxPool("VFX_Pool");

    
    #region Buff Animation Value
    private float kHeadOffsetY = 0.1f;   // 유닛 머리 기준 오프셋
    private float kExtraRiseY = 0.2f;   // 추가 상승 높이
    private Ease kMoveEase = Ease.OutCubic;
    private Ease kFadeEase = Ease.InOutSine;

    private float kAppearRatio = 0.5f;
    private float kMoveRatio = 0.3f;
    private float kHoldRatio = 0.2f;
    private float kFadeRatio = 0.3f;

    #endregion

    public Vector3 TileCenter(Tile t) => t.transform.position + new Vector3(0, 0.25f, 0);

    public GameObject PlayAtTile(GameObject prefab, Tile tile, float scale = 1f, float yRot = 0)
    {
        return PlayAtWorld(prefab, TileCenter(tile), Quaternion.Euler(0, yRot, 0), null, scale);
    }

    public GameObject PlayOnUnit(GameObject prefab, Unit unit, bool follow = true, float scale = 1f)
    {
        var parent = follow ? unit.transform : null;
        var pos = follow ? unit.transform.position : unit.CurrentTileRef != null ? TileCenter(unit.CurrentTileRef) : unit.transform.position;

        return PlayAtWorld(prefab, pos, Quaternion.identity, parent, scale);
    }

    private GameObject PlayAtWorld(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, float scale = 1f)
    {
        if (prefab == null)
            return null;

        var go = _pool.Spawn(prefab, pos, rot, parent);
        go.transform.localScale = Vector3.one * scale;

        if (go.GetComponent<VfxDespawn>() == null)
            go.AddComponent<VfxDespawn>();

        return go;
    }

    public void PlayLineEffect(GameObject prefab, Tile from, Tile to, float scale = 1f)
    {
        var start = TileCenter(from);
        var end = TileCenter(to);
        var mid = (start + end) * 0.5f;
        var dir = end - start;
        var rot = Quaternion.LookRotation(Vector3.forward, dir);

        var go = PlayAtWorld(prefab, mid, rot, null, 1f);

    }

    public void SpawnProjectile(GameObject projectile, Tile from, Tile to, float speed, Action onHit = null, Action<GameObject> onSpawn = null)
    {
        if (projectile == null || from == null || to == null)
            return;

        var start = TileCenter(from);
        var end = TileCenter(to);

        var go = _pool.Spawn(projectile, start, Quaternion.identity, null);
        if (go.GetComponent<VfxDespawn>() == null)
            go.AddComponent<VfxDespawn>();

        onSpawn?.Invoke(go);
        go.GetComponent<MonoBehaviour>().StartCoroutine(Shoot(go, end, speed, onHit));

    }

    private IEnumerator Shoot(GameObject projectile, Vector3 target, float speed, Action onHit)
    {
        while (projectile != null && projectile.activeSelf)
        {
            var pos = projectile.transform.position;
            var step = speed * Time.deltaTime;
            var delta = target - pos;
            if (delta.magnitude <= step)
            {
                projectile.transform.position = target;
                onHit?.Invoke();
                projectile.SetActive(false);
                yield break;
            }
            projectile.transform.position = Vector3.MoveTowards(pos, target, step);
            yield return null;
        }
    }

    private static void SetAlpha(SpriteRenderer sr, float a)
    {
        if (sr == null) return;
        var c = sr.color;
        c.a = a;
        sr.color = c;
    }

    private GameObject PlayRiseFadeOnUnit(GameObject prefab, Unit unit)
    {
        if (prefab == null || unit == null) return null;

        var parent = unit.transform;
        var start = unit.transform.position;

        var go = PlayAtWorld(prefab, start, Quaternion.identity, parent, 1f);
        var sr = go.GetComponentInChildren<SpriteRenderer>();

        var appear = Mathf.Max(0.01f, kAppearRatio);
        var move = Mathf.Max(0.01f, kMoveRatio);
        var hold = Mathf.Max(0f, kHoldRatio);
        var fade = Mathf.Max(0.01f, kFadeRatio);

        var baseWorld = unit.CurrentTileRef != null ? TileCenter(unit.CurrentTileRef) : unit.transform.position;
        var mid = baseWorld + new Vector3(0, kHeadOffsetY, 0);
        var end = mid + new Vector3(0, kExtraRiseY, 0);

        Vector3 midPos = go.transform.InverseTransformPoint(mid);
        Vector3 endPos = go.transform.InverseTransformPoint(end);

        SetAlpha(sr, 0f);

        var seq = DOTween.Sequence().SetLink(go).SetUpdate(false);
        if (sr != null)
            seq.Append(sr.DOFade(1f, appear).SetEase(kFadeEase));

        seq.Join(go.transform.DOLocalMove(midPos, move).SetEase(kMoveEase));

        if (hold > 0f)
            seq.AppendInterval(hold);

        seq.Join(go.transform.DOLocalMove(endPos, fade).SetEase(Ease.OutSine));

        if (sr != null)
            seq.Join(sr.DOFade(0f, fade).SetEase(kFadeEase));

        seq.OnComplete(() => { if (go != null) go.SetActive(false); });
        return go;
    }


    public void KnightEffect(Unit caster)
    {
        PlayRiseFadeOnUnit(EffectManager.Instance._knightBuff, caster);
    }

}
