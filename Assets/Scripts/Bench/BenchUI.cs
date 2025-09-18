using System.Collections.Generic;
using UnityEngine;

public class BenchUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private int _slotCount = 8;

    public List<BenchSlot> _slots = new List<BenchSlot>();

    void Awake()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            var go = Instantiate(_slotPrefab, gameObject.transform);
            var slot = go.GetComponent<BenchSlot>();
            _slots.Add(slot);
        }

    }

    public IEnumerable<BenchSlot> GetAllUnits()
    {
        foreach (var slot in _slots)
        {
            yield return slot;
        }
    }

    public BenchSlot GetEmptySlot()
    {
        foreach (BenchSlot slot in _slots)
        {
            if (slot.Unit == null)
                return slot;
        }
        return null;
    }
}
