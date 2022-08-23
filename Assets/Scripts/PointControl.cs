using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class PointControl : MonoBehaviour, IPointerUpHandler, IBeginDragHandler
{
    public MergeMenuSave _point;

    [SerializeField] private SpriteRenderer _image;
    [SerializeField] private TextMeshPro _margeCountText;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private SortingGroup _sorting;

    private float _speed = 10;
    private float _critFactor;

    private SetColor _targetPreferance;
    private Camera _camera;
    private bool _isActiv;
    private bool _isTranslate;
    private bool _isDrag;
    private delegate void PointBehavior();
    private PointBehavior _pointBehavior;

    [HideInInspector] public bool DontSave;
    private void Awake()
    {
        Invoke(nameof(CheckTargetLayer), 0.1f);
    }
    private void Start()
    {
        if (_point.PointPreferance.pointParameters.layer == "") _point.PointPreferance.pointParameters.layer = Menu.MainField.ToString();
        LayerChange(_point.PointPreferance.pointParameters.layer);
        _point.PointPreferance.pointParameters.Code = 
            $"{Mathf.Round(_point.PointPreferance.Color.r * 255)} | " +
            $"{Mathf.Round(_point.PointPreferance.Color.g * 255)} | " +
            $"{Mathf.Round(_point.PointPreferance.Color.b * 255)}";
        _critFactor = _point.PointPreferance.pointParameters.CriticalDrop / (_point.PointPreferance.pointParameters.MergeLevel + 1);
        _camera = Camera.main;
        _image.color = _point.PointPreferance.Color;
        if (_point.PointPreferance.pointParameters.MergeLevel != 0) _margeCountText.text = 
                _point.PointPreferance.pointParameters.MergeLevel.ToString();
        _isActiv = true;
    }
    private void Update()
    {
        if (_isActiv) Movement();
        if (_isTranslate) _pointBehavior();
    }

    private void CheckTargetLayer()
    {
        if (_point.PointPreferance.Target == null) Destroy(this.gameObject);
        else _point.PointPreferance.Target.gameObject.layer = GAMEControler._LAYER_close;
    }
    private void Movement()
    {
        if (transform.position.x != 
            Mathf.Clamp(transform.position.x, _point.PointPreferance.Target.position.x - 0.001f, _point.PointPreferance.Target.position.x + 0.001f))
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(_point.PointPreferance.Target.position.x, _point.PointPreferance.Target.position.y, 0), _speed * Time.deltaTime);
        }
        else
        {
            transform.position = _point.PointPreferance.Target.position;
            switch (_point.PointPreferance.Target.gameObject.layer)
            {
                case GAMEControler._LAYER_open:
                    _point.PointPreferance.Target.gameObject.layer = GAMEControler._LAYER_close;
                    FillTarget();
                    break;
                case GAMEControler._LAYER_close:
                    FillTarget();
                    break;
                case GAMEControler._LAYER_marge:
                    break;
                default:
                    break;
            }

            _point.PointPreferance.Parent = transform.parent;
            _collider2D.enabled = true;
            _isActiv = false;
        }
    }
    private void FillTarget()
    {
        _targetPreferance = _point.PointPreferance.Target.GetComponent<SetColor>();
        _targetPreferance.Color = _point.PointPreferance.Color;
        _targetPreferance.Point = transform;
        _targetPreferance.MergeCount = _point.PointPreferance.pointParameters.MergeLevel;
    }
    public void Info()
    {
        if (_point.PointPreferance.Target.gameObject.layer == GAMEControler._LAYER_close)
        {
            _point.PointPreferance.constParameters.GameControler.CellCheckColor(_point.PointPreferance.Target);
            _point.PointPreferance.constParameters.GameControler.PointInfo(GetComponent<PointControl>());
            _point.PointPreferance.Target.GetComponent<Image>().color = Color.gray;
            if (Input.GetKey(KeyCode.B))
            {
                PointControl point = Instantiate(this, this.transform.position, Quaternion.identity, _point.PointPreferance.constParameters.Spawner);
                point._point.PointPreferance.Target = _point.PointPreferance.constParameters.GameControler.FindOpenPoint();
                _point.PointPreferance.constParameters.GameControler.SetParametersPerSecond(0, _point.PointPreferance.pointParameters.PaintPerSec);
            }
        }
        _isTranslate = false;
        _isActiv = false;
    }
    public void PointTranslate()
    {
        Vector3 vector = _camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(vector.x, vector.y, 0);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, 0.5f);
    }
    public void PointExit()
    {
        float distance = Mathf.Infinity;
        Transform result = null;
        Collider2D[] cells = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        if (cells.Length == 0 || cells[0].transform == _point.PointPreferance.constParameters.Spawner)
        {
            result = null;
            _speed = 10;
        }
        else
        {
            for (int i = 0; i < cells.Length; i++)
            {
                SetColor setColor = cells[i].GetComponent<SetColor>();
                if (cells[i].gameObject.layer == 0) continue;
                if (cells[i].gameObject.layer == GAMEControler._LAYER_open ||
                    cells[i].gameObject.layer == GAMEControler._LAYER_marge ||
                    cells[i].gameObject.layer == GAMEControler._LAYER_classUpdate ||
                    cells[i].gameObject.layer == GAMEControler._LAYER_pointSecrifice ||
                    setColor.Color == _point.PointPreferance.Color && setColor.MergeCount == _point.PointPreferance.pointParameters.MergeLevel)
                {
                    float dir = Vector2.Distance(transform.position, cells[i].transform.position);
                    if (cells[i].transform == _point.PointPreferance.constParameters.LabButton)
                    {
                        result = _point.PointPreferance.constParameters.GameControler.LabMenuFindOpenCell();
                        if (result == null) GAMEControler.ConsoleEnter("There in no space");
                        else
                        {
                            _point.PointPreferance.Target = result;
                            transform.SetParent(_point.PointPreferance.constParameters.LabPointPack, false);
                            LayerChange(Menu.Lab.ToString());
                            _point.PointPreferance.constParameters.GameControler.SetParametersPerSecond(0, -_point.PointPreferance.pointParameters.PaintPerSec);
                        }
                        _speed = 10;
                        break;
                    }
                    if (dir < distance)
                    {
                        distance = dir;
                        result = cells[i].transform;
                    }
                }
            }
            if(_point.PointPreferance.Target.gameObject.layer != GAMEControler._LAYER_marge) 
                _point.PointPreferance.Target.GetComponent<Image>().color = Color.white;
        }

        if(result == _point.PointPreferance.constParameters.OnFieldButton)
        {
            result = _point.PointPreferance.constParameters.GameControler.FindOpenPoint();
            _point.PointPreferance.Target = result;
            if (result == null) GAMEControler.ConsoleEnter("No space on field");
            else transform.SetParent(_point.PointPreferance.constParameters.Spawner);
            LayerChange(Menu.MainField.ToString());
            _speed = 10;

            if (_point.PointPreferance.pointParameters.MergeLevel == 0) _point.PointPreferance.pointParameters.SellPrice = 1;
            else _point.PointPreferance.pointParameters.SellPrice = Mathf.Pow(2, _point.PointPreferance.pointParameters.MergeLevel);

            _point.PointPreferance.constParameters.GameControler.SetParametersPerSecond(0, _point.PointPreferance.pointParameters.PaintPerSec);
        }

        if (result == null) result = _point.PointPreferance.Target;

        switch (result.gameObject.layer)
        {
            case GAMEControler._LAYER_close:
                SetColor setColor1 = result.GetComponent<SetColor>();
                if (setColor1.Color == _point.PointPreferance.Color && 
                    setColor1.MergeCount == _point.PointPreferance.pointParameters.MergeLevel)
                {
                    PointControl pointControl = setColor1.Point.GetComponent<PointControl>();
                    float chance = Random.Range(0, 100);
                    bool crit = false;
                    if (chance <= _point.PointPreferance.pointParameters.CriticalDrop) crit = true;

                    _point = GAMEControler.MergePoint(_point, pointControl._point);

                    pointControl._margeCountText.text = $"{pointControl.ActiveMerge(crit)}";

                    DontSave = true;
                    Destroy(this.gameObject);
                }
                break;
            case GAMEControler._LAYER_marge:
                result.GetComponent<BoxCollider2D>().enabled = false;
                LabDataEvent labData = result.GetComponent<LabDataEvent>();

                labData.Point.PointPreferance.Color = _point.PointPreferance.Color;
                labData.Point.PointPreferance.pointParameters = _point.PointPreferance.pointParameters;
                labData.Point.PointPreferance.constParameters = _point.PointPreferance.constParameters;
                labData.Point.PointPreferance.mergeParameters = _point.PointPreferance.mergeParameters;

                labData.Point.MenuMerge.CellIsOpen = true;
                labData.Image.gameObject.SetActive(true);
                labData.Image.sprite = _image.sprite;
                labData.Image.color = _image.color;
                labData.Image.fillAmount = 1;

                labData.AddColorToMenu();
                DontSave = true;
                Destroy(this.gameObject);
                break;
            case GAMEControler._LAYER_classUpdate:
                if (_point.PointPreferance.constParameters.ClassUpdate.CellState() && 
                    _point.PointPreferance.pointParameters.MergeLevel == 10)
                {
                    _point.PointPreferance.constParameters.ClassUpdate.SetPoint(this);
                    DontSave = true;
                    Destroy(this.gameObject);
                    break;
                }
                else if (_point.PointPreferance.pointParameters.MergeLevel != 10)
                {
                    GAMEControler.ConsoleEnter("Reached 10LV");
                }
                else
                {
                    GAMEControler.ConsoleEnter("Engaged");
                }
                result = null;
                break;
            case GAMEControler._LAYER_pointSecrifice:
                if (_point.PointPreferance.constParameters.ClassUpdate.PayForUpdate(this))
                {
                    DontSave = true;
                    Destroy(this.gameObject);
                }
                else result = null;
                break;
            default:
                break;
        }

        if (_speed != 10) _speed = distance * 10;
        if (result == null) result = _point.PointPreferance.Target;
        _point.PointPreferance.constParameters.GameControler.PointInfoClose();
        _point.PointPreferance.Target = result;
        _isActiv = true;
        _isDrag = false;
        _isTranslate = false;
    }

    public void LayerChange(string layer)
    {
        _point.PointPreferance.pointParameters.layer = layer;
        GetComponent<SpriteRenderer>().sortingLayerName = layer;
        _sorting.sortingLayerName = layer;
    }
    public int ActiveMerge(bool crit)
    {
        int result = CountParametersAfterMarge(crit);
        FillTarget();
        return result;
    }
    private int CountParametersAfterMarge(bool crit)
    {
        float num = _point.PointPreferance.pointParameters.PaintPerSec;
        if (!crit)
        {
            _point.PointPreferance.pointParameters.SellPrice *= 2;
            _point.PointPreferance.pointParameters.CriticalDrop += _critFactor;
            _point.PointPreferance.pointParameters.MergeLevel++;
            _point.PointPreferance.pointParameters.PaintPerSec *= 
                (1.1f + (float)_point.PointPreferance.pointParameters.MergeLevel / 
                (100 * GAMEControler.ClassBonus(_point.PointPreferance.pointParameters.CurrentClass)));
        }
        else
        {
            _point.PointPreferance.pointParameters.SellPrice *= 4;
            _point.PointPreferance.pointParameters.CriticalDrop += _critFactor * 2;
            _point.PointPreferance.pointParameters.MergeLevel++;
            _point.PointPreferance.pointParameters.PaintPerSec *= 
                (1.1f + (float)_point.PointPreferance.pointParameters.MergeLevel / 
                (100*GAMEControler.ClassBonus(_point.PointPreferance.pointParameters.CurrentClass)));
            _point.PointPreferance.pointParameters.MergeLevel++;
            _point.PointPreferance.pointParameters.PaintPerSec *=
                (1.1f + (float)_point.PointPreferance.pointParameters.MergeLevel / 
                (100 * GAMEControler.ClassBonus(_point.PointPreferance.pointParameters.CurrentClass)));
        }
        if (_point.PointPreferance.pointParameters.CriticalDrop > 50) _point.PointPreferance.pointParameters.CriticalDrop = 50;
        num = _point.PointPreferance.pointParameters.PaintPerSec - num * 2;
        _point.PointPreferance.constParameters.GameControler.SetParametersPerSecond(0, num);
        return _point.PointPreferance.pointParameters.MergeLevel;
    }

    private void OnDestroy()
    {
        if (_point.PointPreferance.Target != null)
        {
            switch (_point.PointPreferance.Target.gameObject.layer)
            {
                case GAMEControler._LAYER_close:
                    break;
                case GAMEControler._LAYER_marge:
                    break;
                default:
                    break;
            }
        }
    }
    private void OnEnable()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.PointFolder, GetInstanceID()))
        {
            JsonSaverBase.FileDel(JsonSaverBase.PointFolder, GetInstanceID());
        }
    }
    private void OnDisable()
    {
        if (_point.PointPreferance.Target != null)
        {
            if (!JsonSaverBase.FileExist(JsonSaverBase.PointFolder, GetInstanceID()) && !DontSave)
            {
                JsonSaverBase.SaveInfo(_point, JsonSaverBase.PointFolder, GetInstanceID());
            }
        }
    }
#if PLATFORM_ANDROID && !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (JsonSaverBase.FileExist(JsonSaverBase.PointFolder, GetInstanceID()))
            {
                JsonSaverBase.FileDel(JsonSaverBase.PointFolder, GetInstanceID());
            }
        }
        else
        {
            if (_point.PointPreferance.Target != null)
            {
                if (!JsonSaverBase.FileExist(JsonSaverBase.PointFolder, GetInstanceID()) && !DontSave)
                {
                    JsonSaverBase.SaveInfo(_point, JsonSaverBase.PointFolder, GetInstanceID());
                }
            }
        }
    }
#endif
    private void OnApplicationQuit()// For PC
    {
        if (_point.PointPreferance.Target != null)
            JsonSaverBase.SaveInfo(_point, JsonSaverBase.PointFolder, GetInstanceID());
    }


    public void MouseUp()
    {
        _isTranslate = true;
        if (_isDrag)
        {
            _pointBehavior = PointExit;
        }
        else _pointBehavior = Info;
    }
    public void MouseDrag()
    {
        if (!_isActiv && !_isTranslate)
        {
            _pointBehavior = PointTranslate;
            _point.PointPreferance.constParameters.GameControler.PointInfoClose();

            switch (_point.PointPreferance.Target.gameObject.layer)
            {
                case GAMEControler._LAYER_close:
                    _point.PointPreferance.Target.GetComponent<SetColor>().Reset.Invoke();
                    break;
                case GAMEControler._LAYER_marge:
                    break;
                default:
                    break;
            }

            _collider2D.enabled = false;
            _isTranslate = true;
            _isDrag = true;
        }
    }

    //private void OnMouseUp()
    //{
    //    MouseUp();
    //}
    //private void OnMouseDrag()
    //{
    //    MouseDrag();
    //}

    public void OnBeginDrag(PointerEventData eventData)
    {
        MouseDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MouseUp();
    }
}
