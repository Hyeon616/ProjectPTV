using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform _shop;
    [SerializeField] private ShopSlot _slotPrefab;

    public List<ShopSlot> _slots = new List<ShopSlot>();
    private ShopManager _shopManager;

    private int _unitslot = 3;

    private Vector2 _hidePos;
    private Vector2 _showPos;
    private float _animDuration = 0.5f;

    private void Awake()
    {
        _showPos = new Vector2(0, -180);
        _hidePos = new Vector2(0, 180);
    }

    public void Init(ShopManager shopManager)
    {
        _shopManager = shopManager;

        InitSlots();
        RefreshShop();

        _shop.anchoredPosition = _hidePos;
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
        int price = 10 * grade;
        _shopManager.BuyUnit(unitData, grade, price);

        // ±¸¸Å ÈÄ ´Ý±â
        //StartCoroutine(HideShop());
    }

    public void ShowShop()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateShop(_hidePos,_showPos));
    }

    public IEnumerator HideShop()
    {
        yield return AnimateShop(_showPos, _hidePos);

    }

    private IEnumerator AnimateShop(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < _animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _animDuration);
            _shop.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        _shop.anchoredPosition = to;

    }
}
