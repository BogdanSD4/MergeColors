using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventDelay : MonoBehaviour
{
    [SerializeField] private WorkMode _mode;
    [SerializeField] private float _invoke = 0.2f;
    [SerializeField] private List<UnityEvent> _event;
    private float _timer;
    private bool _firstUse;
    private bool _touch;
    private bool _move;
    private Vector3 pos;
    private MergeMenuSave _point;
    private GAMEControler _controler;
    [Space]
    [SerializeField] private List<GameObject> _object;
    private void Update()
    {
        if (_touch)
        {
            if (!_firstUse)
            {
                _event[0].Invoke();
                _firstUse = true;
            }
            _timer += Time.deltaTime;
            if (_timer > _invoke)
            {
                _timer = 0;
                _event[0].Invoke();
            }

            switch (_mode)
            {
                case WorkMode.LibraryPoint:
                    _touch = false;
                    break;
                case WorkMode.Spawner:
                    break;
            }
        }
        else if(_timer != 0)
        {
            _timer = 0;
            _firstUse = false;
        }
    }

    private void OnMouseUp()
    {
        switch (_mode)
        {
            case WorkMode.LibraryPoint:
                if(!_move) _touch = true;
                else _move = false;
                break;
            case WorkMode.Spawner:
                _touch = false;
                break;
        }
    }
    private void OnMouseDown()
    {
        switch (_mode)
        {
            case WorkMode.LibraryPoint:
                break;
            case WorkMode.Spawner:
                _touch = true;
                break;
        }
        pos = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        switch (_mode)
        {
            case WorkMode.LibraryPoint:
                if (Input.mousePosition != pos) _move = true;
                break;
            case WorkMode.Spawner:
                _touch = true;
                Vector3 mouse = Input.mousePosition;
                if (mouse.x != Mathf.Clamp(mouse.x, pos.x - 0.5f, pos.x + 0.5f) &&
                    mouse.y != Mathf.Clamp(mouse.y, pos.y - 0.5f, pos.y + 0.5f)) _touch = false;
                break;
        }
    }
    public Component GetObject(System.Type type, int ObjectNUM) => _object[ObjectNUM].GetComponent(type);
    public void SetPoint(MergeMenuSave save, GAMEControler controler)
    {
        _point = save;
        _controler = controler;
        _event[0].AddListener(FillEvent);
    }
    public void FillEvent()
    {
        _controler.OpenAndFillPointMoreInfoMenu(_point);
    }

    public void EventStart(int order) => _event[order].Invoke();

    public enum WorkMode
    {
        LibraryPoint,
        Spawner
    }

    public void DestroyObject(GameObject gameObject) => Destroy(gameObject);
}

