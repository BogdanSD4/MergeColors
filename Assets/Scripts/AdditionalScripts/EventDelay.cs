using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventDelay : MonoBehaviour
{
    [SerializeField] private float _invoke = 0.2f;
    [SerializeField] List<UnityEvent> _event;
    private float _timer;
    private bool _firstUse;
    private MergeMenuSave _point;
    private GAMEControler _controler;
    [Space]
    [SerializeField] private List<GameObject> _object;
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
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
        }
        else if(_timer != 0)
        {
            _timer = 0;
            _firstUse = false;
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

    public void EventOne() => _event[0].Invoke();
    public void EventTwo() => _event[1].Invoke();
}
