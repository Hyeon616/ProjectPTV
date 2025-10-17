using System.Collections.Generic;
using UnityEngine;

public class VfxPool
{
    private Transform _poolRoot;
    private Dictionary<GameObject, Stack<GameObject>> _pool = new Dictionary<GameObject, Stack<GameObject>>();

    public VfxPool(string name = "VFX_Pool")
    {
        _poolRoot = new GameObject(name).transform;
        Object.DontDestroyOnLoad(_poolRoot);
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (!_pool.TryGetValue(prefab, out var stack))
        {
            stack = new Stack<GameObject>();
            _pool[prefab] = stack;
        }

        GameObject go = null;
        while (stack.Count > 0 && go == null)
            go = stack.Pop();

        if (go == null)
            go = Object.Instantiate(prefab);

        go.transform.SetPositionAndRotation(pos, rot);
        go.transform.SetParent(parent, worldPositionStays: true);
        go.SetActive(true);

        return go;
    }

    public void Despawn(GameObject prefab, GameObject instance)
    {
        instance.SetActive(false);
        instance.transform.SetParent(_poolRoot, worldPositionStays: false);
        if (!_pool.TryGetValue(prefab, out var stack))
        {
            stack = new Stack<GameObject>();
            _pool[prefab] = stack;
        }
        stack.Push(instance);

    }



}
