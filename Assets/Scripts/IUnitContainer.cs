
using UnityEngine;

public interface IUnitContainer
{

    Unit Unit { get; }
    void SetUnit(Unit unit);
    void ClearUnit();
    Transform GetTransform();
    bool IsField { get; }
}
