using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGradeUI : MonoBehaviour
{
    [Header("UI")]
    private Image[] _stars;

    public void Init(UnitState unitState)
    {
        InitStarImage();
        UpdateGradeUI(unitState);
    }

    private void UpdateGradeUI(UnitState unitState)
    {
        foreach (var star in _stars)
            star.enabled = false;

        for (int i = 0; i < unitState.CurrentGrade && i < _stars.Length; i++)
            _stars[i].enabled = true;

    }

    private void InitStarImage()
    {
        Transform starRoot = transform.Find("GradeCanvas/Star");
        _stars = new Image[3];

        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i] = starRoot.GetChild(i).GetComponent<Image>();
        }
    }
}
