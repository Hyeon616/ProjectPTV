using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BenchSlot : MonoBehaviour, IUnitContainer
{
    [SerializeField] private Image _portraitImage;

    public Unit Unit { get; private set; }


    private void Reset()
    {
        if (_portraitImage == null)
        {
            _portraitImage = transform.GetComponentInChildren<Image>();
        }
    }


    public void SetUnit(Unit unit)
    {
        Unit = unit;

        if (unit != null)
        {
            unit.gameObject.SetActive(false);

            if (_portraitImage != null)
            {
                _portraitImage.sprite = unit.UnitState.UnitData._benchPortrait;

                Color color = _portraitImage.color;
                color.a = 1f;
                _portraitImage.color = color;
            }
        }
        else
        {
            HidePortrait();
        }

    }
    public void ClearUnit()
    {
        Unit = null;

        HidePortrait();
    }

    private void HidePortrait()
    {
        if (_portraitImage != null)
        {
            _portraitImage.sprite = null;

            Color color = _portraitImage.color;
            color.a = 0f;
            _portraitImage.color = color;
        }
    }


    public Transform GetTransform() => transform;

    public bool IsField => false;
}
