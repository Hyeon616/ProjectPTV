using UnityEngine;
using UnityEngine.UI;

public class BenchSlot : MonoBehaviour, IUnitContainer
{
    [SerializeField] private Image _portraitImage;

    [SerializeField] private Image _backGround;
    [SerializeField] private Image[] _stars;

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
                _portraitImage.enabled = true;
            }

            ShowStarUI();

        }
        else
        {
            HideUI();
        }

    }
    public void ClearUnit()
    {
        Unit = null;

        HideUI();
    }

    private void HideUI()
    {
        if (_portraitImage != null)
        {
            _portraitImage.sprite = null;
            _portraitImage.enabled = false;

        }

        if (_backGround != null)
        {
            _backGround.enabled = false;

        }

        if (_stars != null)
        {
            foreach (var star in _stars)
            {
                star.enabled = false;
            }
        }
    }

    private void ShowStarUI()
    {


        if (_backGround != null)
        {
            _backGround.enabled = true;

        }

        if (_stars != null)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].enabled = (i < Unit.UnitState.CurrentGrade);

            }
        }

    }

    public Transform GetTransform() => transform;

    public bool IsField => false;
}
