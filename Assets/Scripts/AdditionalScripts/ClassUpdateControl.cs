using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassUpdateControl : MonoBehaviour
{
    [Header("ClassUpdate")]
    [SerializeField] private Image _labClassUpdatePointBefore;
    [SerializeField] private Image _labClassUpdatePointAfter;
    [SerializeField] private TextMeshProUGUI _labClassUpdateXP;
    [SerializeField] private TextMeshProUGUI _labClassUpdatePutColorTips;
    [SerializeField] private Slider _labClassUpdateXPLine;
    [SerializeField] private CircleCollider2D _boxPoint;
    [SerializeField] private BoxCollider2D _boxXP;
    [Space]
    [SerializeField] private Image _labClassBorderBefor;
    [SerializeField] private Image _labClassBorderAfter;
    [Space]
    [SerializeField] private Transform PrefabPoint;
    [SerializeField] private GAMEControler _gameControler;
    private MergeMenuSave _parameters = new MergeMenuSave();
    private bool _active;
    private bool _isReady;
    [SerializeField] private float _speed = 100;
    private float _bonusFactor = 10;
    private double _divider;
    private double _currentValue;
    private double _newValue;

    private void Awake()
    {
        if(JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()))
        {
            Load();
        }
    }
    private void Start()
    {
        if (_labClassBorderBefor.color == Color.clear)
        {
            print("clear");
            _boxPoint.enabled = true;
            _boxXP.enabled = false;
            SetXpText("Color not found");
            _labClassUpdatePutColorTips.gameObject.SetActive(true);
            _labClassBorderAfter.color =
                _labClassUpdatePointBefore.color =
                _labClassUpdatePointAfter.color = Color.clear;
        }
        else
        {
            print("fill");
            _boxPoint.enabled = false;
            if (_currentValue > 0)
            {
                _boxXP.enabled = true;
                SetXpText(System.Math.Round(_currentValue).ToString() + " XP");
            }
            else SetXpText("Ready");
            _labClassUpdatePutColorTips.gameObject.SetActive(false);

            float fill = (float)(1 - _currentValue / _divider);
            _labClassUpdateXPLine.value = fill;
            _labClassUpdatePointBefore.fillAmount = 1 - fill;
            _labClassUpdatePointAfter.fillAmount = fill;
        }
    }
    public bool CellState()
    {
        if (_labClassUpdatePointBefore.color == Color.clear) return true;
        else return false;
    }
    private void Update()
    {
        if (_active)
        {
            float booster = 0.1f;
            _currentValue = Mathf.MoveTowards((float)_currentValue, (float)_newValue - 1, _speed * Time.deltaTime);

            float fill = (float)(1 - _currentValue / _divider);
            _labClassUpdateXPLine.value = fill;
            _labClassUpdatePointBefore.fillAmount = 1 - fill;
            _labClassUpdatePointAfter.fillAmount = fill;
            
            SetXpText(System.Math.Round(_currentValue).ToString() + " XP");

            if (_speed >= 50000 && booster != 500) booster = 500;
            else if (_speed >= 5000 && booster != 50 && _speed < 50000) booster = 50;
            else if (_speed >= 500 && booster != 5 && _speed < 5000) booster = 5;
            else if (_speed >= 200 && booster != 1 && _speed < 500) booster = 1;
            _speed += booster;

            if(_currentValue <= 0)
            {

                SetXpText("Ready");
                _active = false;
                _isReady = true;
                _speed = 100;
                _boxXP.enabled = false;
            }
            
            if(_currentValue <= _newValue)
            {
                _currentValue = _newValue;
                _speed = 100;
                _active = false;
            }
        }
    }
    private void SetXpText(string text) => _labClassUpdateXP.text = text;

    [ContextMenu("Finish")]
    private void Finish() => _currentValue = 0;

    public void SetPoint(PointControl point)
    {
        MergeMenuSave merge = point._point;
        _parameters.PointPreferance.Color = merge.PointPreferance.Color;
        _parameters.PointPreferance.constParameters = merge.PointPreferance.constParameters;
        _parameters.PointPreferance.mergeParameters = merge.PointPreferance.mergeParameters;
        _parameters.PointPreferance.pointParameters = merge.PointPreferance.pointParameters;

        _labClassBorderBefor.color = GAMEControler.ClassColor(merge.PointPreferance.pointParameters.CurrentClass);

        _labClassUpdatePointBefore.color = merge.PointPreferance.Color;
        _labClassUpdatePointBefore.fillAmount = 1;

        _labClassUpdatePointAfter.color = merge.PointPreferance.Color;
        _labClassUpdatePointAfter.fillAmount = 0;

        _labClassUpdatePutColorTips.gameObject.SetActive(false);
        SetXpText(CountXP());

        UpdateParameters();
        _labClassBorderAfter.color = GAMEControler.ClassColor(_parameters.PointPreferance.pointParameters.CurrentClass);
        _labClassUpdateXPLine.value = 0;
        _boxPoint.enabled = false;
        _boxXP.enabled = true;
    }
    public bool PayForUpdate(PointControl point)
    {
        if (_currentValue > 0)
        {
            ColorClass colorClass = point._point.PointPreferance.pointParameters.CurrentClass;
            int level = point._point.PointPreferance.pointParameters.MergeLevel;

            float factor = Random.Range(1f, _bonusFactor) / 100;
            float value =
                (ClassXP(colorClass) * level * factor
                + (level * point._point.PointPreferance.pointParameters.PaintPerSec)
                + (level * point._point.PointPreferance.pointParameters.CoinPerSec * 100));
            print(_newValue);
            if (value == 0) _newValue -= point._point.PointPreferance.pointParameters.Price;
            else _newValue -= value;

            GAMEControler.ConsoleEnter($"Bonus: {Mathf.Round(factor * 100)}% ({value})");
            _active = true;
            return true;
        }
        return false;
    }
    public void ReturnColor()
    {
        if (_isReady)
        {
            Transform result = _gameControler.LabMenuFindOpenCell();
            if (result == null) GAMEControler.ConsoleEnter("No space");
            else
            {
                Transform tr = Instantiate(PrefabPoint, _labClassUpdatePointAfter.transform.position,
                    Quaternion.identity, _parameters.PointPreferance.constParameters.LabPointPack);
                PointControl point = tr.GetComponent<PointControl>();

                point._point.PointPreferance.Color = _parameters.PointPreferance.Color;
                point._point.PointPreferance.Target = result;
                point._point.PointPreferance.pointParameters = _parameters.PointPreferance.pointParameters;
                point._point.PointPreferance.mergeParameters = _parameters.PointPreferance.mergeParameters;
                point._point.PointPreferance.constParameters = _parameters.PointPreferance.constParameters;

                EndOfUpdate();
            }
        }
    }
    public void EndOfUpdate()
    {
        print("end");
        _boxPoint.enabled = true;
        _isReady = false;
        _labClassBorderBefor.color =
            _labClassBorderAfter.color =
            _labClassUpdatePointAfter.color =
            _labClassUpdatePointBefore.color = Color.clear;
        _labClassUpdateXPLine.value = 0;
        _labClassUpdatePutColorTips.gameObject.SetActive(true);
        SetXpText("Color not found");
    }

    private string CountXP()
    {
        string current;
        double XP = ClassXP(_parameters.PointPreferance.pointParameters.CurrentClass);
        XP += (XP * (Random.Range(10f, 60f)/100));
        _currentValue = _divider = _newValue = XP;

        current = System.Math.Round(XP).ToString();
        print(current);
        int space = 0;
        string result = "";
        for (int i = current.Length - 1; i >= 0; i--)
        {
            space++;
            result = result.Insert(0, $"{current[i]}");
            if (space % 3 == 0)
            {
                result = result.Insert(0, " ");
            }
        }
        return result + " XP";
    }
    private float ClassXP(ColorClass colorClass)
    {
        float XP = 0;
        switch (colorClass)
        {
            case ColorClass.Common:
                XP = 2000;
                _parameters.PointPreferance.pointParameters.CurrentClass = ColorClass.Unusual;
                break;
            case ColorClass.Unusual:
                XP = 40000;
                _parameters.PointPreferance.pointParameters.CurrentClass = ColorClass.Rare;
                break;
            case ColorClass.Rare:
                XP = 800000;
                _parameters.PointPreferance.pointParameters.CurrentClass = ColorClass.Epic;
                break;
            case ColorClass.Epic:
                XP = 16000000;
                _parameters.PointPreferance.pointParameters.CurrentClass = ColorClass.Legendary;
                break;
            case ColorClass.Legendary:
                XP = 320000000;
                _parameters.PointPreferance.pointParameters.CurrentClass = ColorClass.Mythical;
                break;
            default:
                break;
        }
        return XP;
    }
    private void UpdateParameters()
    {
        _parameters.PointPreferance.pointParameters.MergeLevel = 0;
        _parameters.PointPreferance.pointParameters.SellPrice *= 2;
        _parameters.PointPreferance.pointParameters.PaintPerSec +=
            _parameters.PointPreferance.pointParameters.PaintPerSec / 
            GAMEControler.ClassBonus(_parameters.PointPreferance.pointParameters.CurrentClass);

        GAMEControler.MergeParametersAfterClassUpdate(
            _parameters.PointPreferance.mergeParameters, _parameters.PointPreferance.pointParameters.CurrentClass);
    }


    private void Save()
    {
        _parameters.ClassUpdate.Active = _active;
        _parameters.ClassUpdate.IsReady = _isReady;
        _parameters.ClassUpdate.Devider = _divider;
        _parameters.ClassUpdate.CurrentValue = _currentValue;
        _parameters.ClassUpdate.NewValue = _newValue;
        _parameters.ClassUpdate.BorderColorBefore = _labClassBorderBefor.color;

        JsonSaverBase.SaveInfo(_parameters, JsonSaverBase.MergeFolder, GetInstanceID());
    }
    private void Load()
    {
        JsonSaverBase.Loadinfo(_parameters, JsonSaverBase.MergeFolder, GetInstanceID());

        _active = _parameters.ClassUpdate.Active;
        _isReady = _parameters.ClassUpdate.IsReady;
        _divider = _parameters.ClassUpdate.Devider;
        _currentValue = _parameters.ClassUpdate.CurrentValue;
        _newValue = _parameters.ClassUpdate.NewValue;
        _labClassBorderBefor.color = _parameters.ClassUpdate.BorderColorBefore;
        _labClassBorderAfter.color = GAMEControler.ClassColor(_parameters.PointPreferance.pointParameters.CurrentClass);
        _labClassUpdatePointBefore.color = _labClassUpdatePointAfter.color = _parameters.PointPreferance.Color;
    }

#if PLATFORM_ANDROID && !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && _active)
            {
                JsonSaverBase.FileDel(JsonSaverBase.MergeFolder, GetInstanceID());
            }
        }
        else
        {
            if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && _active)
            {
                Save();
            }
        }
    }
#endif

    private void OnApplicationQuit()
    {
        if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()))
        {
            _currentValue = _newValue;
            Save();
        }
    }
}
