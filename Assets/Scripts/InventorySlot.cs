using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IUnitContainer
{
    public Unit Unit { get; private set; }

    

    private Image _slotImage;

    private void Awake()
    {
        _slotImage = GetComponent<Image>();
    }

    

    public Transform GetTransform() => transform;

    public void SetUnit(Unit unit)
    {
        Unit = unit;
        if (unit != null)
        {
            unit.transform.SetParent(transform);
            unit.transform.localPosition = Vector3.zero;
        }
    }

    public void ClearUnit()
    {
        Unit = null;
    }

    public bool IsField => false;
}
