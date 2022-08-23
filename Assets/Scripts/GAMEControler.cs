using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class GAMEControler : MonoBehaviour
{
    [Header("CoinBar")]
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _paintText;
    [SerializeField] private float _coins;
    [SerializeField] private float _paint;
    [SerializeField] private string _absentTime;
    [Space]
    [SerializeField] private TextMeshProUGUI _coinPerText;
    [SerializeField] private TextMeshProUGUI _paintPerText;
    [Space]
    [SerializeField] private GameObject _loadMenu;

    [Space]
    [Header("GameMenu")]
    [SerializeField] private List<GameMenu> _gameMenu;

    [Space]
    [Header("FabricList")]
    [SerializeField] private Transform Prefab;
    [SerializeField] private List<Fabric> _fabric;

    [Space]
    [Header("ShopSettings")]
    [SerializeField] private RectTransform _shopContent;
    [SerializeField] private ShopStand _prefabShopPoint;
    [SerializeField] private RectTransform _prefabShopPointEmpty;
    private UnityEvent _shopCreate = new UnityEvent();
    private List<ShopStand> _willDeleted = new List<ShopStand>();

    [Space]
    [Header("LabSettings")]
    [SerializeField] private RectTransform _labContent;
    [SerializeField] private RectTransform _prefabLabPoint;
    [Space]
    [SerializeField] private GameObject _labChest;
    [SerializeField] private Transform _labButtonTransform;
    [SerializeField] private Transform _labCellPack;
    [SerializeField] private Transform _labPointPack;
    [SerializeField] private Transform _labOnField;
    [Header("ClassUpdate")]
    [SerializeField] private ClassUpdateControl _labClassUpate;

    [Space]
    [Header("Spawner")]
    [SerializeField] private SpriteRenderer _spawner;
    [SerializeField] private Transform _pointPack;

    [Space]
    [Header("PointInfo")]
    [SerializeField] private GameObject _infoBar;
    [SerializeField] private TextMeshProUGUI _infoSellPrice;
    [SerializeField] private TextMeshProUGUI _infoPaintPerSec;
    [SerializeField] private TextMeshProUGUI _infoCriticalDrop;
    [SerializeField] private TextMeshProUGUI _infoCurrentClass;
    [SerializeField] private Image _infoMenuColor;
    [Header("MoreInfoMenu")]
    [SerializeField] private GameObject _infoMoreInfo;
    [SerializeField] private TextMeshProUGUI _infoMoreText;
    [SerializeField] private Image _infoMoreColor;
    [SerializeField] private RectTransform _infoContent;
    private PointControl _point;
    private float _sellPrice;
    private float _paintPerSec;
    private SetColor _currentInfoCell;

    [Space]
    [Header("SellerMenu")]
    [SerializeField] private Seller _seller;
    [SerializeField] private GameObject _sellPaintButton;
    [SerializeField] private float _coinPrice;
    [SerializeField] private float _paintCount;
    [SerializeField] private float _sellerResult;

    [Space]
    [Header("PaintLibraryMenu")]
    [SerializeField] private Animation _loadAnim;
    [SerializeField] private RectTransform _libContent;
    [SerializeField] private RectTransform _libColor;
    [SerializeField] private RectTransform _libClassName;
    [SerializeField] private ColorManager _colorManager;

    [Space]
    [Header("SettingsMenu")]
    [SerializeField] private AudioSource _settingsMusic;
    [SerializeField] private AudioSource _settingsSound;
    [SerializeField] private Slider _settingsMusicControl;
    [SerializeField] private Slider _settingsSoundControl;

    [Space]
    [Header("AnotherSettings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Canvas _mainField;
    [SerializeField] private GameObject _return;
    [SerializeField] private GameObject _modMenu;
    [SerializeField] private GameObject _clickBar;
    [SerializeField] private TimeSystem _timeSystem;
    [SerializeField] private TextMeshProUGUI _consoleTMP;
    [SerializeField] private TextMeshProUGUI _clickAmountTMP;
    [SerializeField] private Animation _clickAmountAnim;
    [SerializeField] private SoundManager _soundManager;
    private MergeMenuSave _save = new MergeMenuSave();
    private ConstParameters _constParameters = new ConstParameters();

    private List<Fabric> _fabricSpawn = new List<Fabric>();
    private List<Color> _opensShaders = new List<Color>();

    private const float _shopPointDistance = 1.1f;
    public const int _LAYER_open = 8;
    public const int _LAYER_close = 9;
    public const int _LAYER_marge = 10;
    public const int _LAYER_classUpdate = 11;
    public const int _LAYER_pointSecrifice = 12;
    public static Color OpenPointColor = new Color(0.99f, 1, 1, 1);
    public static long OpenPrice = 1;
    public static SetColor CurrentCell;
    private float _timer;

    private static TextMeshProUGUI _console;
    private static Animation _consoleAnim;

    private static TextMeshProUGUI _clickAmount;
    private static Animation _clickAnim;

    private static SoundManager _SoundManager;

    private void Awake()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.MainSettingsFolder, GetInstanceID())) Load();
        else CoinCall = 5000;
    }
    private void Start()
    {
        CoinCall = 0;
        PaintCall = 0;
        _console = _consoleTMP;
        _consoleAnim = _consoleTMP.GetComponent<Animation>();
        _clickAmount = _clickAmountTMP;
        _clickAnim = _clickAmountAnim;
        _SoundManager = _soundManager;
        CheckFirstSpawnerImage();
        float paint = 0;
        foreach(Transform tr in _spawner.transform)
        {
            paint += tr.GetComponent<PointControl>()._point.PointPreferance.pointParameters.PaintPerSec;
        }
        PaintPerSec = paint;
        SetParametersPerSecond(0, 0);

        _settingsMusicControl.value = _settingsMusic.volume;
        _settingsSoundControl.value = _settingsSound.volume;

        if(_absentTime != "") 
            _timeSystem.CheckOffline(_absentTime);

        string[] data;
        if ((data = Directory.GetFiles(Path.Combine
            (JsonSaverBase.CurrentPlatform(), JsonSaverBase.ShopFolder))).Length != 0)
        {
             ShopMenuLoad(data);
        }
        ShopMenuFill(ColorClass.All);
        PaintLibraryFill();
        SetConstParameters();
        MenuChange(_mainField);
    }
    private void SetConstParameters()
    {
        _constParameters.CellPack = _pointPack;
        _constParameters.LabCellPack = _labCellPack;
        _constParameters.LabPointPack = _labPointPack;
        _constParameters.Spawner = _spawner.transform;
        _constParameters.LabButton = _labButtonTransform;
        _constParameters.OnFieldButton = _labOnField;
        _constParameters.GameControler = GetComponent<GAMEControler>();
        _constParameters.ClassUpdate = _labClassUpate;
    }
    private void ShopMenuFill(ColorClass Class)
    {
        for (int i = 0; i < _shopContent.childCount; i++)
        {
            Destroy(_shopContent.GetChild(i).gameObject);
        }

        float distance = 0;

        _shopContent.sizeDelta = Vector2.zero;
        for (int i = 0; i < _fabric.Count; i++)
        {
            RectTransform shopRT;
            ShopStand shop;

            if (Class != ColorClass.All && _fabric[i].PointParameters.BaseClass != Class) continue;
            shop = Instantiate(_prefabShopPoint, _shopContent);
            shopRT = shop.transform.GetComponent<RectTransform>();
            shop.Fabric = _fabric[i];
            shopRT.pivot = new Vector2(0.5f, 0.5f + distance);

            _shopContent.sizeDelta += new Vector2(0, shopRT.sizeDelta.y + 25);
            distance += _shopPointDistance;
        }
    }
    private void PaintLibraryFill()
    {
        var posYColorClass = 0f;
        var posY = 1.95f;

        for (int i = 0; i < _colorManager._gameColors.Count; i++)
        {
            var posX = 2.15f;
            if(i > 0) posY += 1.95f;
            RectTransform tr = Instantiate(_libClassName, _libContent);
            if (tr.childCount > 1)
            {
                Debug.LogError("LibraryClassNameHaveExtraElement");
            }
            ((TextMeshProUGUI)tr.GetComponent<EventDelay>().GetObject(typeof(TextMeshProUGUI), 0)).text
                = _colorManager._gameColors[i].ColorClass.ToString();
            tr.pivot += new Vector2(0, posYColorClass);

            int cellInLayer = 0;
            int layer = 0;
            for (int j = 0; j < _colorManager._gameColors[i].Color.Count; j++)
            {
                RectTransform col = Instantiate(_libColor, _libContent);
                EventDelay delay = col.GetComponent<EventDelay>();
                Image image = ((Image)delay.GetObject(typeof(Image), 0));
                MergeMenuSave parameters = _colorManager._gameColors[i].Color[j].point;

                if (_colorManager._gameColors[i].Color[j].point.PointPreferance.Color != Color.clear)
                {
                    image.color = parameters.PointPreferance.Color;
                }
                else
                {
                    if (parameters.PointPreferance.Sprite != null)
                    {
                        image.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
                        image.sprite = parameters.PointPreferance.Sprite;
                    }
                    else image.color = Color.clear;
                }

                ((EventDelay)delay.GetObject(typeof(EventDelay), 1))
                    .SetPoint(_colorManager._gameColors[i].Color[j].point, this);

                col.pivot = new Vector2(posX, posY);
                cellInLayer++;
                if (cellInLayer == 4)
                {
                    posY += 1.25f;
                    posX = 2.15f;
                    cellInLayer = 0;
                    layer++;
                    _libContent.sizeDelta += new Vector2(0, 280);
                }
                else posX -= 1.1f;
                
            }
            _libContent.sizeDelta += new Vector2(0, 440);
            posYColorClass += layer * 2.35f;
            posYColorClass += 3.7f;
        }

    }
    private void Update()
    {
        if(PaintPerSec != 0)
        {
            _timer += Time.deltaTime;
            if(_timer > 1)
            {
                _timer = 0;
                PaintCall = PaintPerSec * Clicker.factor;
            }
        }
    }
    public static Transform CellOutline { get; set; }
    public void CellCheckColor(Transform transform)
    {
        CellOutline = transform;
        Transform menu = null;
        for(int i = 0; i < _gameMenu.Count; i++)
        {
            if (_gameMenu[i].Canvas.gameObject.activeSelf)
            {
                switch (_gameMenu[i].Menu)
                {
                    case Menu.MainField: menu = _pointPack;
                        break;
                    case Menu.Shop:
                        break;
                    case Menu.Lab: menu = _labCellPack;
                        break;
                    case Menu.Update:
                        break;
                    default:
                        break;
                }
            }
        }
        if (menu != null)
        {
            foreach (Transform tr in menu)
            {
                if (tr.GetComponent<Image>() && tr.GetComponent<Image>().color == Color.gray)
                {
                    tr.transform.GetComponent<SetColor>().Check.Invoke();
                }
            }
        }
    }
    public float CoinCall
    {
        get { return _coins; }
        set 
        {
            _coins += value;
            _coinText.text = $"Coin: {Mathf.Round(_coins)}";
        }
    }
    public float PaintCall
    {
        get { return _paint; }
        set
        {
            _paint += value;
            _paintText.text = $"Paint: {Mathf.Round(_paint)}";
        }
    }
    public float CoinPerSec { get; set; }
    public float PaintPerSec { get; set; }
    public void SetParametersPerSecond(float coin, float paint)
    {
        CoinPerSec += coin;
        PaintPerSec += paint;
        _coinPerText.text = $"{(float)System.Math.Round(CoinPerSec * Clicker.factor, 1)}/sec";
        _paintPerText.text = $"{(float)System.Math.Round(PaintPerSec * Clicker.factor, 1)}/sec";
    }

    public void PointSell()
    {
        CoinCall = _sellPrice;
        if(_point.transform.parent == _constParameters.Spawner) 
            SetParametersPerSecond(0, -_paintPerSec);
        PointInfoClose();
        _currentInfoCell.Reset.Invoke();
        _point.DontSave = true;
        Destroy(_point.gameObject);
    }
    public void PointInfo(PointControl point)
    {
        _currentInfoCell = point._point.PointPreferance.Target.GetComponent<SetColor>();
        _infoBar.SetActive(true);
        _sellPrice = point._point.PointPreferance.pointParameters.SellPrice;
        _paintPerSec = point._point.PointPreferance.pointParameters.PaintPerSec;
        _infoSellPrice.text = $"Sell Price: {_sellPrice}";
        _infoPaintPerSec.text = $"Paint per second: {(float)System.Math.Round(_paintPerSec, 1)}";
        _infoCriticalDrop.text = $"Critical Drop: {(float)System.Math.Round(point._point.PointPreferance.pointParameters.CriticalDrop, 2)}%";
        _infoCurrentClass.text = $"Current Class: {point._point.PointPreferance.pointParameters.CurrentClass}";
        _infoMenuColor.color = ClassColor(point._point.PointPreferance.pointParameters.CurrentClass);
        _point = point;
    }
    public void PointMoreInfoMenuButton() => OpenAndFillPointMoreInfoMenu(null);
    public void OpenAndFillPointMoreInfoMenu(MergeMenuSave save)
    {
        if (save != null && (_infoMoreColor.color = save.PointPreferance.Color) == Color.clear)
        {
            ConsoleEnter("Color not found");
            return;
        }
        MenuOpen(_infoMoreInfo);
        _infoContent.localPosition = Vector2.zero;
        if (save == null)
        {
            _infoMoreColor.color = _point._point.PointPreferance.Color;
            _infoMoreText.text =
                $"Code: {_point._point.PointPreferance.pointParameters.Code}\n" +
                $"Price: {_point._point.PointPreferance.pointParameters.Price}\n" +
                $"SellPrice: {_point._point.PointPreferance.pointParameters.SellPrice}\n" +
                $"CriticalDrop: {_point._point.PointPreferance.pointParameters.CriticalDrop}\n" +
                $"PaintPerSec: {_point._point.PointPreferance.pointParameters.PaintPerSec}\n" +
                $"CoinPerSec: {_point._point.PointPreferance.pointParameters.CoinPerSec}\n" +
                $"BaseClass: {_point._point.PointPreferance.pointParameters.BaseClass}\n" +
                $"CurrentClass: {_point._point.PointPreferance.pointParameters.CurrentClass}\n" +

                $"SameMergeChance: {System.Math.Round(_point._point.PointPreferance.mergeParameters.SameMergeFaledChance, 1)}%\n" +
                $"RealColorDropChance: {System.Math.Round(_point._point.PointPreferance.mergeParameters.RealColorChance, 1)}%\n" +
                $"Ordinary: {System.Math.Round(_point._point.PointPreferance.mergeParameters.Ordinary, 1)}%\n" +
                $"Unusual: {System.Math.Round(_point._point.PointPreferance.mergeParameters.Unusual, 1)}%\n" +
                $"Rare: {System.Math.Round(_point._point.PointPreferance.mergeParameters.Rare, 1)}%\n" +
                $"Epic: {System.Math.Round(_point._point.PointPreferance.mergeParameters.Epic, 1)}%\n" +
                $"Legendary: {System.Math.Round(_point._point.PointPreferance.mergeParameters.Legendary, 1)}%\n" +
                $"Mythical: {System.Math.Round(_point._point.PointPreferance.mergeParameters.Mythical, 1)}%\n";
        }
        else
        {
            _infoMoreColor.color = save.PointPreferance.Color;
            _infoMoreText.text = FillInfoText(save);
        }
    }

    public static string FillInfoText(MergeMenuSave save)
    {
        return
                $"Code: {save.PointPreferance.pointParameters.Code}\n" +
                $"Price: {System.Math.Round(save.PointPreferance.pointParameters.Price,2)}\n" +
                $"SellPrice: {System.Math.Round(save.PointPreferance.pointParameters.SellPrice, 2)}\n" +
                $"CriticalDrop: {System.Math.Round(save.PointPreferance.pointParameters.CriticalDrop, 2)}\n" +
                $"PaintPerSec: {System.Math.Round(save.PointPreferance.pointParameters.PaintPerSec, 2)}\n" +
                $"CoinPerSec: {System.Math.Round(save.PointPreferance.pointParameters.CoinPerSec, 2)}\n" +
                $"BaseClass: {save.PointPreferance.pointParameters.BaseClass}\n" +
                $"CurrentClass: {save.PointPreferance.pointParameters.CurrentClass}\n" +

                $"SameMergeChance: {System.Math.Round(save.PointPreferance.mergeParameters.SameMergeFaledChance, 2)}%\n" +
                $"RealColorDropChance: {System.Math.Round(save.PointPreferance.mergeParameters.RealColorChance, 2)}%\n" +
                $"Ordinary: {System.Math.Round(save.PointPreferance.mergeParameters.Ordinary, 2)}%\n" +
                $"Unusual: {System.Math.Round(save.PointPreferance.mergeParameters.Unusual, 2)}%\n" +
                $"Rare: {System.Math.Round(save.PointPreferance.mergeParameters.Rare, 2)}%\n" +
                $"Epic: {System.Math.Round(save.PointPreferance.mergeParameters.Epic, 2)}%\n" +
                $"Legendary: {System.Math.Round(save.PointPreferance.mergeParameters.Legendary, 2)}%\n" +
                $"Mythical: {System.Math.Round(save.PointPreferance.mergeParameters.Mythical, 2)}%\n";
    }
    public static void FillParameters(MergeMenuSave Object, MergeMenuSave Data)
    {
        Object.PointPreferance.constParameters = Data.PointPreferance.constParameters;

        Object.PointPreferance.mergeParameters.SameMergeFaledChance = Data.PointPreferance.mergeParameters.SameMergeFaledChance;
        Object.PointPreferance.mergeParameters.RealColorChance = Data.PointPreferance.mergeParameters.RealColorChance;
        Object.PointPreferance.mergeParameters.Ordinary = Data.PointPreferance.mergeParameters.Ordinary;
        Object.PointPreferance.mergeParameters.Unusual = Data.PointPreferance.mergeParameters.Unusual;
        Object.PointPreferance.mergeParameters.Rare = Data.PointPreferance.mergeParameters.Rare;
        Object.PointPreferance.mergeParameters.Epic = Data.PointPreferance.mergeParameters.Epic;
        Object.PointPreferance.mergeParameters.Legendary = Data.PointPreferance.mergeParameters.Legendary;
        Object.PointPreferance.mergeParameters.Mythical = Data.PointPreferance.mergeParameters.Mythical;

        Object.PointPreferance.pointParameters.Code = Data.PointPreferance.pointParameters.Code;
        Object.PointPreferance.pointParameters.MergeLevel = Data.PointPreferance.pointParameters.MergeLevel;
        Object.PointPreferance.pointParameters.Price = Data.PointPreferance.pointParameters.Price;
        Object.PointPreferance.pointParameters.SellPrice = Data.PointPreferance.pointParameters.SellPrice;
        Object.PointPreferance.pointParameters.CriticalDrop = Data.PointPreferance.pointParameters.CriticalDrop;
        Object.PointPreferance.pointParameters.PaintPerSec = Data.PointPreferance.pointParameters.PaintPerSec;
        Object.PointPreferance.pointParameters.CoinPerSec = Data.PointPreferance.pointParameters.CoinPerSec;
        Object.PointPreferance.pointParameters.BaseClass = Data.PointPreferance.pointParameters.BaseClass;
        Object.PointPreferance.pointParameters.CurrentClass = Data.PointPreferance.pointParameters.CurrentClass;

        Object.PointPreferance.Color = Data.PointPreferance.Color;
        Object.PointPreferance.Sprite = Data.PointPreferance.Sprite;
        Object.PointPreferance.Material = Data.PointPreferance.Material;
    }
    public static MergeMenuSave MergePoint(MergeMenuSave Object, MergeMenuSave Data)
    {
        Object.PointPreferance.pointParameters.Price = 
            (Object.PointPreferance.pointParameters.Price + Data.PointPreferance.pointParameters.Price) / 2;
        Object.PointPreferance.pointParameters.CriticalDrop = 
            (Object.PointPreferance.pointParameters.CriticalDrop + Data.PointPreferance.pointParameters.CriticalDrop) / 2;
        Object.PointPreferance.pointParameters.PaintPerSec = 
            (Object.PointPreferance.pointParameters.PaintPerSec + Data.PointPreferance.pointParameters.PaintPerSec) / 2;
        Object.PointPreferance.pointParameters.CoinPerSec = 
            (Object.PointPreferance.pointParameters.CoinPerSec + Data.PointPreferance.pointParameters.CoinPerSec) / 2;

        Object.PointPreferance.mergeParameters.SameMergeFaledChance = 
            (Object.PointPreferance.mergeParameters.SameMergeFaledChance + Data.PointPreferance.mergeParameters.SameMergeFaledChance) / 2;
        Object.PointPreferance.mergeParameters.RealColorChance = 
            (Object.PointPreferance.mergeParameters.RealColorChance + Data.PointPreferance.mergeParameters.RealColorChance) / 2;
        Object.PointPreferance.mergeParameters.Ordinary = 
            (Object.PointPreferance.mergeParameters.Ordinary + Data.PointPreferance.mergeParameters.Ordinary) / 2;
        Object.PointPreferance.mergeParameters.Unusual = 
            (Object.PointPreferance.mergeParameters.Unusual + Data.PointPreferance.mergeParameters.Unusual) / 2;
        Object.PointPreferance.mergeParameters.Rare = 
            (Object.PointPreferance.mergeParameters.Rare + Data.PointPreferance.mergeParameters.Rare) / 2;
        Object.PointPreferance.mergeParameters.Epic = 
            (Object.PointPreferance.mergeParameters.Epic + Data.PointPreferance.mergeParameters.Epic) / 2;
        Object.PointPreferance.mergeParameters.Legendary = 
            (Object.PointPreferance.mergeParameters.Legendary + Data.PointPreferance.mergeParameters.Legendary) / 2;
        Object.PointPreferance.mergeParameters.Mythical = 
            (Object.PointPreferance.mergeParameters.Mythical + Data.PointPreferance.mergeParameters.Mythical) / 2;

        return Object;
    }

    public void PointInfoClose()
    {
        _infoBar.SetActive(false);
        CellCheckColor(null);
    }


    public Transform LabMenuFindOpenCell()
    {
        foreach(Transform tr in _labCellPack)
        {
            if (tr.gameObject.layer == _LAYER_open)
            {
                tr.gameObject.layer = _LAYER_close;
                return tr;
            }
        }
        return null;
    }


    public void OpenPaintLibrary()
    {
        MenuOpen(_loadMenu);
        _loadAnim.Play();
    }
    public void MenuChange(Canvas canvas)
    {
        bool set = true;
        bool shopMenu = false;
        bool labMenu = false;
        Menu menu = Menu.MainField;
        PointInfoClose();
        for (int i = 0; i < _gameMenu.Count; i++)
        {
            _gameMenu[i].Canvas.gameObject.SetActive(false);
            if (_gameMenu[i].Canvas == canvas) menu = _gameMenu[i].Menu;
        }
        canvas.gameObject.SetActive(true);
        switch (menu)
        {
            case Menu.MainField:
                set = false;
                break;
            case Menu.Shop:
                shopMenu = true;
                if (_shopCreate != null)
                {
                    _shopCreate.Invoke();
                    _shopCreate.RemoveAllListeners();
                }
                break;
            case Menu.Lab:
                labMenu = true;
                break;
            case Menu.Update:
                break;
            default:
                break;
        }
        _return.SetActive(set);
        _spawner.gameObject.SetActive(!set);
        _clickBar.gameObject.SetActive(!set);
        _sellPaintButton.SetActive(shopMenu);
        _labChest.SetActive(labMenu);
    }
    public void Buy(Fabric color, float price)
    {
        if (CoinCall >= price)
        {
            CoinCall = -price;
            _fabricSpawn.Add(color);
            CheckFirstSpawnerImage();
            _timer = 0;
        }
        else
        {
            ConsoleEnter("Not enought money");
        }
    }
    private void CheckFirstSpawnerImage()
    {
        if (_fabricSpawn.Count == 0) _spawner.color = Color.clear;
        else
        {
            _spawner.color = _fabricSpawn[0].Color;
            _spawner.sprite = _fabricSpawn[0].Sprite;
        }
    }
    public void SpawnerEventClick(SpriteRenderer image)
    {
        if (OpenPoint())
        {
            if (image.color != new Color(0, 0, 0, 0))
            {
                Transform point = Instantiate(Prefab, new Vector3(_spawner.transform.position.x, _spawner.transform.position.y, 0),
                    Quaternion.identity, _spawner.transform);
                PointControl pointControl = point.GetComponent<PointControl>();
                pointControl._point.PointPreferance.constParameters = _constParameters;
                pointControl._point.PointPreferance.Target = FindOpenPoint();
                pointControl._point.PointPreferance.Color = image.color;
                for (int i = 0; i < _fabric.Count; i++)
                {
                    if (pointControl._point.PointPreferance.Color == _fabric[i].Color)
                    {
                        pointControl._point.PointPreferance.pointParameters.MergeLevel = _fabric[i].PointParameters.MergeLevel;
                        pointControl._point.PointPreferance.pointParameters.Price = _fabric[i].PointParameters.Price;
                        pointControl._point.PointPreferance.pointParameters.SellPrice = _fabric[i].PointParameters.SellPrice;
                        pointControl._point.PointPreferance.pointParameters.CriticalDrop = _fabric[i].PointParameters.CriticalDrop;
                        pointControl._point.PointPreferance.pointParameters.PaintPerSec = _fabric[i].PointParameters.PaintPerSec;
                        pointControl._point.PointPreferance.pointParameters.CoinPerSec = _fabric[i].PointParameters.CoinPerSec;
                        pointControl._point.PointPreferance.pointParameters.BaseClass = _fabric[i].PointParameters.BaseClass;
                        pointControl._point.PointPreferance.pointParameters.CurrentClass = _fabric[i].PointParameters.CurrentClass;

                        pointControl._point.PointPreferance.mergeParameters.SameMergeFaledChance = _fabric[i].ParametersMerge.SameMergeFaledChance;
                        pointControl._point.PointPreferance.mergeParameters.RealColorChance = _fabric[i].ParametersMerge.RealColorChance;
                        pointControl._point.PointPreferance.mergeParameters.Ordinary = _fabric[i].ParametersMerge.Ordinary;
                        pointControl._point.PointPreferance.mergeParameters.Unusual = _fabric[i].ParametersMerge.Unusual;
                        pointControl._point.PointPreferance.mergeParameters.Rare = _fabric[i].ParametersMerge.Rare;
                        pointControl._point.PointPreferance.mergeParameters.Epic = _fabric[i].ParametersMerge.Epic;
                        pointControl._point.PointPreferance.mergeParameters.Legendary = _fabric[i].ParametersMerge.Legendary;
                        pointControl._point.PointPreferance.mergeParameters.Mythical = _fabric[i].ParametersMerge.Mythical;
                    }
                }
                SetParametersPerSecond(0, pointControl._point.PointPreferance.pointParameters.PaintPerSec);
                _fabricSpawn.RemoveAt(0);
                CheckFirstSpawnerImage();
            }
        }
    }
    public Transform FindOpenPoint()
    {
        List<Transform> point = new List<Transform>();
        for(int i = 0; i < _pointPack.childCount; i++)
        {
            if(_pointPack.GetChild(i).gameObject.layer == _LAYER_open)
            {
                point.Add(_pointPack.GetChild(i));
            }
        }
        if (point.Count == 0) return null;
        int num = UnityEngine.Random.Range(0, point.Count);
        point[num].gameObject.layer = _LAYER_close;
        return point[num];
    }
    private bool OpenPoint()
    {
        for (int i = 0; i < _pointPack.childCount; i++)
        {
            if (_pointPack.GetChild(i).gameObject.layer == _LAYER_open)
            {
                return true;
            }
        }
        return false;
    }
    public void OpenCell()
    {
        
        long price = OpenPrice * 5;
        if (PaintCall >= price)
        {
            PaintCall = -price;
            CurrentCell.OpenCell(true);
        }
        else
        {
            ConsoleEnter("Not enought money");
            CurrentCell.OpenCell(false);
        }
    }


    public void SellerOpen(Slider slider)
    {
        slider.value = 1;
        _seller.PanelMenu.SetActive(true);
        _seller.PriceCoin.text = _coinPrice.ToString();
        _seller.CountPaint.text = Mathf.Round(PaintCall).ToString();
        _paintCount = PaintCall;
        _sellerResult = Mathf.FloorToInt((slider.value * _paintCount) / _coinPrice);
        _seller.Result.text = _sellerResult.ToString();
        
    }
    public void SellerClose(BoxCollider2D collider2D)
    {
        if (!collider2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            collider2D.transform.parent.gameObject.SetActive(false);
        }
    }
    public void SellerResult(Slider slider)
    {
        _sellerResult = Mathf.FloorToInt((slider.value * _paintCount) / _coinPrice);
        _seller.Result.text = _sellerResult.ToString();
        _seller.CountPaint.text = Mathf.Round(slider.value * _paintCount).ToString();
    }
    public void SellerSell()
    {
        PaintCall = -(_sellerResult * _coinPrice);
        CoinCall = _sellerResult;
        _seller.PanelMenu.SetActive(false);
    }
    public void SellerUpdate() => print(_coinPrice--);



    public void SettingsMusic() => _settingsMusic.volume = _settingsMusicControl.value;
    public void SettingsSound() => _settingsSound.volume = _settingsSoundControl.value;


    [ContextMenu("PeintPerSecNull")]
    private void ToNull() => PaintPerSec = 0;
    [ContextMenu("DellPlayerPrefs")]
    private void Dell() => PlayerPrefs.DeleteAll();
    public static Color ClassColor(ColorClass colorClass)
    {
        switch (colorClass)
        {
            case ColorClass.Common: 
                return new Color32(0xE1, 0xFF, 0xE3, 0xFF);
            case ColorClass.Unusual:
                return new Color32(0x6B, 0xC8, 0xFF, 0xFF); 
            case ColorClass.Rare:
                return new Color32(0xFF, 0x9D, 0x00, 0xFF); 
            case ColorClass.Epic:
                return new Color32(0xDE, 0x69, 0xFF, 0xFF); 
            case ColorClass.Legendary:
                return new Color32(0xFF, 0xF4, 0x00, 0xFF); 
            case ColorClass.Mythical:
                return new Color32(0xFF, 0x45, 0x45, 0xFF); 
            default:
                return Color.cyan;
        }
    }
    public static float ClassBonus(ColorClass colorClass)
    {
        switch (colorClass)
        {
            case ColorClass.Common:
                return 1;
            case ColorClass.Unusual:
                return 0.9f;
            case ColorClass.Rare:
                return 0.8f;
            case ColorClass.Epic:
                return 0.7f;
            case ColorClass.Legendary:
                return 0.6f;
            case ColorClass.Mythical:
                return 0.5f;
            default:
                return 0;
        }
    }


    public (bool, MergeMenuSave) CheckColorInMaret(Color color)
    {
        MergeMenuSave merge = new MergeMenuSave();
        for(int i = 0; i < _fabric.Count; i++)
        {
            if (_fabric[i].Color == color)
            {
                merge.PointPreferance.pointParameters = _fabric[i].PointParameters;
                merge.PointPreferance.mergeParameters = _fabric[i].ParametersMerge;
                return (true, merge);
            }
        }
        return (false, null);
    }
    public SimplePointParameters GetColorInMarket(Color color)
    {
        for (int i = 0; i < _fabric.Count; i++)
        {
            if (_fabric[i].Color == color) return _fabric[i].PointParameters;
        }
        return null;
    }
    public void AddColorToMarket(MergeMenuSave point)
    {
        Fabric fabric = new Fabric();

        fabric.Color = point.PointPreferance.Color;
        fabric.Sprite = point.PointPreferance.Sprite;
        fabric.Material = point.PointPreferance.Material;
        fabric.ParametersMerge = point.PointPreferance.mergeParameters;
        fabric.PointParameters = point.PointPreferance.pointParameters;
        _fabric.Add(fabric);

        _shopCreate.AddListener(Create);
    }
    public void ReplaceColorInMarket(MergeMenuSave point)
    {
        for (int i = 0; i < _fabric.Count; i++)
        {
            if (_fabric[i].Color == point.PointPreferance.Color)
            {
                _fabric[i].Color = point.PointPreferance.Color;
                _fabric[i].PointParameters = point.PointPreferance.pointParameters;
                _fabric[i].ParametersMerge = point.PointPreferance.mergeParameters;
                break;
            }
        }
        _shopCreate.AddListener(Create);
    }
    private void Create()
    {
        for(int i = 0; i < _willDeleted.Count; i++)
        {
            int count;
            Fabric fabric;
            (fabric, count) = _willDeleted[i].Save;
            _fabric.Remove(fabric);

            File.Delete(Path.Combine(JsonSaverBase.CurrentPlatform(), JsonSaverBase.ShopFolder, $"0{count}.txt"));
            Destroy(_willDeleted[i].gameObject);

            _willDeleted.Remove(_willDeleted[i]);
        }

        ShopMenuFill(ColorClass.All);
    }
    public static void MergeParametersAfterClassUpdate(MergePointParameters mergePoint, ColorClass colorClass)
    {

    }

    public void ShopClassChooseAll() => ShopMenuFill(ColorClass.All);
    public void ShopClassChooseCommon() => ShopMenuFill(ColorClass.Common);
    public void ShopClassChooseUnusual() => ShopMenuFill(ColorClass.Unusual);
    public void ShopClassChooseRare() => ShopMenuFill(ColorClass.Rare);
    public void ShopClassChooseEpic() => ShopMenuFill(ColorClass.Epic);
    public void ShopClassChooseLegendary() => ShopMenuFill(ColorClass.Legendary);
    public void ShopClassChooseMythical() => ShopMenuFill(ColorClass.Mythical);



    private void ShopMenuLoad(string[] data)
    {
        _shopContent.sizeDelta = new Vector2(0, 140);

        for (int i = 0; i < data.Length; i++)
        {
            int Long = data[i].Length;
            string type = "";
            string json;
            for (int j = 1; j < "txt".Length + 1; j++)
            {
                type = $"{type}{data[i][Long - j]}";
            }
            if (type == "txt")
            {
                json = File.ReadAllText(data[i]);
                Fabric fabric = JsonUtility.FromJson<Fabric>(json);

                _fabric.Add(fabric);
            }
            else continue;
        }
    }
    public void ColorDell(ShopStand shop)
    {
        _fabric.Remove(shop.Fabric);
        _willDeleted.Add(shop);
        _shopCreate.AddListener(Create);
    }
    public void ColorReturn(ShopStand shop)
    {
        _fabric.Add(shop.Fabric);
        _willDeleted.Remove(shop);
    }


    public static void MenuOpen(GameObject gameObject) => gameObject.SetActive(true);
    public static void MenuClose(GameObject gameObject) => gameObject.SetActive(false);

    private void Save()
    {
        _save.Settings.AbsentTime = DateTime.Now.ToString();
        _save.Settings.Coins = CoinCall;
        _save.Settings.Paint = PaintCall;
        _save.Settings.CoinsPerSec = CoinPerSec;
        _save.Settings.PaintPerSec = PaintPerSec;
        _save.Settings.Colors = _fabricSpawn;
        _save.Settings.Points = _fabric;
        _save.Settings.SellerCoinPrice = _coinPrice;

        JsonSaverBase.SaveInfo(_save, JsonSaverBase.MainSettingsFolder, GetInstanceID());
    }
    private void Load()
    {
        JsonSaverBase.Loadinfo(_save, JsonSaverBase.MainSettingsFolder, GetInstanceID());

        _absentTime = _save.Settings.AbsentTime;
        _coins = _save.Settings.Coins;
        _paint = _save.Settings.Paint;
        CoinPerSec = _save.Settings.CoinsPerSec;
        PaintPerSec = _save.Settings.PaintPerSec;
        _fabricSpawn = _save.Settings.Colors;
        _fabric = _save.Settings.Points;
        _coinPrice = _save.Settings.SellerCoinPrice;
    }


    public static void ConsoleEnter(string enter)
    {
        _consoleAnim.Stop();
        _console.text = enter;
        _consoleAnim.Play();
    }
    public static void ClickAmount(string enter)
    {
        _clickAnim.Stop();
        _clickAmount.text = enter;
        _clickAnim.Play();
    }
    public static SoundManager GetAudio() => _SoundManager;
#if PLATFORM_ANDROID && !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            _timeSystem.CheckOffline(_absentTime);
            Load();
        }
        else
        {
            Save();
        }
    }
#endif
    private void OnApplicationQuit()
    {
        Save();
    }
}
[System.Serializable]
public class GameMenu
{
    public Menu Menu;
    public Canvas Canvas;
}
[System.Serializable]
public class Fabric
{
    public Color Color;
    public Sprite Sprite;
    public Material Material;
    public bool BaseColor;
    [Header("SimpleParameters")]
    public SimplePointParameters PointParameters = new SimplePointParameters();
    [Header("MergeParameters")]
    public MergePointParameters ParametersMerge = new MergePointParameters();
    
}
[System.Serializable]
public class SimplePointParameters
{
    public string Code;
    public int MergeLevel;
    public float Price;
    public float SellPrice;
    public float CriticalDrop;
    public float PaintPerSec;
    public float CoinPerSec;
    public string layer;
    public ColorClass BaseClass;
    public ColorClass CurrentClass;
}
[System.Serializable]
public class MergePointParameters
{
    public float SameMergeFaledChance;
    public float RealColorChance;
    [Header("ColorClass")]
    public float Ordinary;
    public float Unusual;
    public float Rare;
    public float Epic;
    public float Legendary;
    public float Mythical;
}
[System.Serializable]
public class Seller
{
    public GameObject PanelMenu;
    public BoxCollider2D PanelBox;
    public TextMeshProUGUI CountPaint;
    public TextMeshProUGUI PriceCoin;
    public TextMeshProUGUI Result;
}
[System.Serializable]
public class ConstParameters
{
    public Transform CellPack;
    public Transform LabCellPack;
    public Transform LabPointPack;
    public Transform Spawner;
    public Transform LabMenuSpawner;
    public Transform LabButton;
    public Transform OnFieldButton;
    public GAMEControler GameControler;
    public ClassUpdateControl ClassUpdate;
}
public enum Menu
{
    MainField = 1,
    Shop = 2,
    Lab = 4,
    Update = 8,
    PaintLibrary = 16
}
public enum ColorClass
{
    All,
    Common = 30,
    Unusual = 180,
    Rare = 600,
    Epic = 4800,
    Legendary = 14400,
    Mythical = 43200,
}