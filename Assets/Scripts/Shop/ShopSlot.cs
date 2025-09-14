using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Image _portrait;
    private Button _button;

    private UnitData _unitData;
    private int _grade;
    private Action<UnitData, int> _onBuyCallback;


    public void Init(Action<UnitData, int> callback)
    {
        _button = GetComponent<Button>();
        _onBuyCallback = callback;
        _button.onClick.AddListener(BuyUnit);
    }

    public void SetUnit(UnitData unitData, int grade)
    {
        _unitData = unitData;
        _grade = grade;

        _portrait.sprite = unitData._shopPortrait;


    }

    private void BuyUnit()
    {
        if (_unitData != null)
        {
            _onBuyCallback?.Invoke(_unitData, _grade);
        }

    }
}
