using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    [SerializeField] private DamageText _damageText;
    private int _poolSize = 20;

    private Stack<DamageText> _pool = new Stack<DamageText>();

    public void PoolCreate()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            Push(Create());
        }
    }

    private DamageText Create()
    {
        var inst = Instantiate(_damageText, transform);
        inst.Init();
        inst.gameObject.SetActive(false);
        return inst;
    }

    public DamageText Get(Transform parent)
    {
        var damageText = _pool.Count > 0 ? _pool.Pop() : Create();
        damageText.transform.SetParent(parent, false);
        damageText.gameObject.SetActive(true);

        return damageText;

    }

    public void Release(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        damageText.transform.SetParent(transform, false);
        _pool.Push(damageText);
    }

    private void Push(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        damageText.transform.SetParent(transform, false);
        _pool.Push(damageText);
    }

}
