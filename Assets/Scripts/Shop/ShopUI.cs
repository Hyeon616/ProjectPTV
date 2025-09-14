using System.Collections.Generic;
using UnityEngine;


public class ShopUI : MonoBehaviour
{
    [SerializeField] private Transform _shop;
    [SerializeField] private ShopSlot _slotPrefab;

    public List<ShopSlot> _slots = new List<ShopSlot>();
    private ShopManager _shopManager;

    private int _unitslot = 3;

    public void Init(ShopManager shopManager)
    {
        _shopManager = shopManager;

        InitSlots();
        RefreshShop();
    }
    private void InitSlots()
    {
        for (int i = 0; i < _unitslot; i++)
        {
            ShopSlot slot = Instantiate(_slotPrefab, _shop);
            slot.Init(BuyUnit);
            _slots.Add(slot);
        }
    }

    private void RefreshShop()
    {
        foreach (ShopSlot slot in _slots)
        {
            (UnitData unitData, int grade) = _shopManager.GetShopUnit();
            slot.SetUnit(unitData, grade);
        }
    }

    private void BuyUnit(UnitData unitData, int grade)
    {
        int price = 70 * grade;
        _shopManager.BuyUnit(unitData, grade, price);

    }


}
