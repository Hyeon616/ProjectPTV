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

    private void Start()
    {

       // HideUI();
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

            Color color = _portraitImage.color;
            color.a = 0f;
            _portraitImage.color = color;
        }

        if (_backGround != null)
        {
            Color color = _backGround.color;
            color.a = 0f;
            _backGround.color = color;

        }

        if (_stars != null)
        {
            foreach (var star in _stars)
            {
                Color color = star.color;
                color.a = 0f;
                star.color = color;
            }
        }
    }

    private void ShowStarUI()
    {
        

        if (_backGround != null)
        {
            Color color = _backGround.color;
            color.a = 1f;
            _backGround.color = color;

        }

        if (_stars != null)
        {
            for (int i = 0; i < _stars.Length; i++)
            {

                Color color = _stars[i].color;
                color.a = (i < Unit.UnitState.CurrentGrade) ? 1f : 0f;
                _stars[i].color = color;
            }
        }

    }

    public Transform GetTransform() => transform;

    public bool IsField => false;
}
