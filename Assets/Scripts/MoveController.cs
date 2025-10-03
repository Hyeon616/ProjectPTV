using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour
{
    private Camera _mainCamera;
    private Unit _dragUnit;
    private Tile _highlightTile;

    private Vector3 _currentPos;
    private IUnitContainer _currentSlot;

    [SerializeField] private FieldSceneManager _fieldSceneManager;
    [SerializeField] private StageManager _stageManager;

    private LayerMask _unitLayer;
    private LayerMask _tileLayer;


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


        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = screenPos };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        BenchSlot currentSlot = null;
        foreach (var slot in results)
        {
            currentSlot = slot.gameObject.GetComponent<BenchSlot>();
            if (currentSlot != null)
                break;
        }

        if (currentSlot != null && currentSlot.Unit != null)
        {
            _dragUnit = currentSlot.Unit;
            _currentSlot = currentSlot;
            _currentPos = _dragUnit.transform.position;

            currentSlot.ClearUnit();
            _dragUnit.gameObject.SetActive(true);

            if (!_stageManager.IsBattle)
                _fieldSceneManager.FieldManager.ShowField();
            return;
        }

        if (_stageManager != null && _stageManager.IsBattle)
            return;

        // Field
        Collider2D hit = Physics2D.OverlapPoint(pos, _unitLayer);
        if (hit != null)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit != null && unit.UnitState.Owner == Owner.Player)
            {
                _dragUnit = unit;

                _currentSlot = _dragUnit.UnitState.CurrentSlot;
                if (_currentSlot != null)
                    _currentSlot.ClearUnit();

                _currentPos = _dragUnit.transform.position;
                _fieldSceneManager.FieldManager.ShowField();
            }
        }
    }


    private void OnTouchDrag(Vector3 screenPos)
    {
        if (_dragUnit == null)
            return;

        Vector2 pos = _mainCamera.ScreenToWorldPoint(screenPos);
        // 드래그 중
        _dragUnit.transform.position = new Vector3(pos.x, pos.y + 0.4f, _dragUnit.transform.position.z);

        // 하이라이트 처리
        Collider2D hit = Physics2D.OverlapPoint(pos, _tileLayer);
        Tile targetTile = hit != null ? hit.GetComponent<Tile>() : null;

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
            return;

        Vector2 pos = _mainCamera.ScreenToWorldPoint(screenPos);
        IUnitContainer target = null;

        if(!(_stageManager != null && _stageManager.IsBattle))
        {
            Collider2D hit = Physics2D.OverlapPoint(pos, _tileLayer);
            if (hit != null) target = hit.GetComponent<IUnitContainer>();
        }

        if (target == null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current) { position = screenPos };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                IUnitContainer slot = result.gameObject.GetComponent<IUnitContainer>();
                if (slot != null)
                {
                    target = slot;
                    break;
                }
            }
        }

        bool success = false;

        if (target != null)
            success = _fieldSceneManager.UnitManager.DragDrop(_dragUnit, target);

        if (!success)
        {
            if (_currentSlot != null)
            {
                _currentSlot.SetUnit(_dragUnit);
                _dragUnit.gameObject.SetActive(!_currentSlot.IsField ? false : true);
            }
            else
            {
                _dragUnit.transform.position = _currentPos;
            }
        }

        if (_highlightTile != null)
        {
            _highlightTile.HighlightTargetTile(false);
            _highlightTile = null;
        }

        _dragUnit = null;
        _fieldSceneManager.FieldManager.HideField();

    }



}
