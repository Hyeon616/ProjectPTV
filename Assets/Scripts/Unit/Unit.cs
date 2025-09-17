using UnityEngine;
using UnityEngine.UI;



public class Unit : MonoBehaviour
{

    public UnitState UnitState {  get; private set; }

    private Image[] _stars;

    public void Init(UnitData unitData, Owner owner, int grade)
    {
        UnitState = new UnitState(unitData, owner, grade);

        InitStarImage();

        UpdateGradeUI();
    }

    private void UpdateGradeUI()
    {
        foreach (var star in _stars)
            star.enabled = false;

        for (int i = 0; i < UnitState.CurrentGrade && i < _stars.Length; i++)
            _stars[i].enabled = true;

    }

    private void InitStarImage()
    {
        Transform starRoot = transform.Find("Canvas/Star");
        _stars = new Image[3];

        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i] = starRoot.GetChild(i).GetComponent<Image>();
        }
    }

    public void OnUpgrade()
    {
        UnitState.Upgrade();
        UpdateGradeUI();
    }


}
