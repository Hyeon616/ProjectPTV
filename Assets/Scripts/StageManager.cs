using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager _instance { get; private set; }

    [SerializeField] private UnitDB _unitDB;
    public UnitDB UnitDB => _unitDB;



    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _unitDB.Init();

    }



}
