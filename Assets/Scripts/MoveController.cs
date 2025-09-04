using UnityEngine;

public class MoveController : MonoBehaviour
{
    private Camera _mainCamera;
    private Unit _dragUnit;
    private Vector3 _currentPos;

    [SerializeField] private FieldSceneManager _fieldSceneManager;
    private LayerMask _unitLayer;
    private LayerMask _tileLayer;

    private Tile _highlightTile;

    void Start()
    {
        _mainCamera = Camera.main;

        _unitLayer = 1 << (int)LayerNum.Unit;
        _tileLayer = 1 << (int)LayerNum.Tile;

    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            OnTouchDown(Input.mousePosition);
        else if (Input.GetMouseButton(0))
            OnTouchDrag(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0))
            OnTouchUp(Input.mousePosition);
#else
        // 모바일 (터치 입력)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                OnTouchDown(touch.position);
            else if (touch.phase == TouchPhase.Moved)
                OnTouchDrag(touch.position);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
                OnTouchUp(touch.position);
        }
#endif
    }

    private void OnTouchDown(Vector2 screenPos)
    {
        Vector2 pos = _mainCamera.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(pos, _unitLayer);

        if (hit != null)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit != null && unit._unitState.Owner == Owner.Player)
            {
                _dragUnit = unit;
                _currentPos = unit.transform.position;

                _fieldSceneManager.FieldManager.ShowField();
            }
        }
    }


    private void OnTouchDrag(Vector3 screenPos)
    {
        if (_dragUnit == null)
            return;

        Vector2 pos = _mainCamera.ScreenToWorldPoint(screenPos);
        _dragUnit.transform.position = new Vector3(pos.x, pos.y + 0.4f, _currentPos.z);

        Collider2D hit = Physics2D.OverlapPoint(pos, _tileLayer);
        Tile targetTile = hit ? hit.GetComponent<Tile>() : null;

        if (_highlightTile != targetTile)
        {
            if (_highlightTile != null)
                _highlightTile.HighlightTargetTile(false);

            if (targetTile != null)
                targetTile.HighlightTargetTile(true);

            _highlightTile = targetTile;
        }

    }

    private void OnTouchUp(Vector3 screenPos)
    {
        if (_dragUnit == null)
        {

            return;
        }

        Vector2 pos = _mainCamera.ScreenToWorldPoint(screenPos);

        Collider2D hit = Physics2D.OverlapPoint(pos, _tileLayer);
        Tile targetTile = hit ? hit.GetComponent<Tile>() : null;

        _fieldSceneManager.UnitManager.DragDrop(_dragUnit, targetTile);


        if (targetTile != null && targetTile.IsPlayerField)
        {
            _dragUnit.transform.SetParent(targetTile.transform);
            _dragUnit.transform.localPosition = new Vector3(0, 0.45f, 0);
        }
        else
            _dragUnit.transform.position = _currentPos;

        if (_highlightTile != null)
        {
            _highlightTile.HighlightTargetTile(false);
            _highlightTile = null;
        }


        _dragUnit = null;
        _fieldSceneManager.FieldManager.HideField();

    }



}
