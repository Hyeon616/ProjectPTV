public interface IUnitState
{
    void Enter(Unit u);
    void Execute(Unit u);
    void Exit(Unit u);
}
