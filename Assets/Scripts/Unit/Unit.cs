using UnityEngine;



public class Unit : MonoBehaviour
{

    public UnitState UnitState {  get; private set; }

    public void Init(UnitData unitData, Owner owner, int grade)
    {
        UnitState = new UnitState(unitData, owner, grade);

        ShowGradeUI();
    }

    private void ShowGradeUI()
    {
        // TODO
        // 유닛 하단의 별

    }

}
