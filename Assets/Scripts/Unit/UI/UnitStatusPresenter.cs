using UnityEngine;

public class UnitStatusPresenter : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private UnitStatusUI _unitStatusUI;

    private void OnEnable()
    {


    }


    public void Bind(UnitState unitState)
    {
        if (unitState == null)
            return;

        unitState.OnHpChanged += HandleHpChanged;
        unitState.OnMpChanged += HandleMpChanged;
    }


    private void HandleHpChanged(int cur, int max)
    {
        _unitStatusUI.SetHpRatio(max > 0 ? (cur / (float)max) : 0f);
    }

    private void HandleMpChanged(int cur, int max)
    {
        _unitStatusUI.SetMpRatio(max > 0 ? (cur / (float)max) : 0f);
    }

    public void ResetUI(int curHp, int maxHp, int curMp, int maxMp)
    {
        _unitStatusUI.ResetBar(maxHp > 0 ? curHp / (float)maxHp : 0f, maxMp > 0 ? curMp / (float)maxMp : 0f);

    }
}
