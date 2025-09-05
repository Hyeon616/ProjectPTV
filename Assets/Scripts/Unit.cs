using UnityEngine;



public class Unit : MonoBehaviour
{

    public UnitState UnitState {  get; private set; }

    public void Init(UnitState state)
    {
        UnitState = state;
    }


}
