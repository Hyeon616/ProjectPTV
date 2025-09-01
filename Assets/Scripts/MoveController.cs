using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class MoveController : MonoBehaviour
{
    private Camera _mainCamera;
    private Unit _dragUnit;
    
    [SerializeField] private FieldSceneManager _fieldSceneManager;


    void Start()
    {
        _mainCamera = Camera.main;

    }

    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnMouseDown();
        else if (Input.GetMouseButtonUp(0))
            OnMouseUp();
    }

    private void OnMouseDown()
    {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null && unit._unitState._owner == Owner.Player)
            {
                _dragUnit = unit;
            }
        }
    }

    private void OnMouseUp()
    {
        if (_dragUnit == null) return;

        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Tile targetTile = hit.collider.GetComponent<Tile>();
            if (targetTile != null)
            {
                _fieldSceneManager.UnitManager.DragDrop(_dragUnit, targetTile);
            }
        }

        _dragUnit = null;
    }

}
