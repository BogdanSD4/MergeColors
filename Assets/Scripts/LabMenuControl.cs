using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class LabMenuControl : MonoBehaviour
{
    [SerializeField] private Transform _cellPack;
    [SerializeField] private TextMeshProUGUI _button;
    [SerializeField] private MargeManager _mergeManager;
    [SerializeField] private TimeSystem _timeSystem;
    [SerializeField] private LabContentControl _labContent;

    [SerializeField] private List<LabDataEvent> LabDatas = new List<LabDataEvent>();

    public Image NewColorImage;
    public MergeMenuSave NewColorControl = new MergeMenuSave();

    private void Awake()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()))
        {
            JsonSaverBase.Loadinfo(NewColorControl, JsonSaverBase.MergeFolder, GetInstanceID());
            ImageFill();
            StartMerge();
        }
        else
        {
            for (int i = 0; i < NewColorControl.MenuMerge.OpenCell; i++)
            {
                LabDatas[i].Point.MenuMerge.CellIsOpen = true;
            }
        }
    }
    void Start()
    {
        if (!NewColorControl.MenuMerge.EventState)
        {
            _mergeManager.SetEventStart();
            if(!NewColorControl.MenuMerge.IsMerge) SetButtonText("Start");
        }
        else
        {
            _mergeManager.SetEventGet();
            SetButtonText("Get Color");
        }

        CheckCellsState();
    }

    private void Update()
    {
        if (NewColorControl.MenuMerge.IsMerge)
        {
            var ts = TimeSpan.FromSeconds(NewColorControl.MenuMerge.Timer);
            SetButtonText(string.Format("{0}:{1}:{2}:{3}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
            for (int i = 0; i < NewColorControl.MenuMerge.OpenCell; i++)
            {
                LabDatas[i].Image.fillAmount = NewColorControl.MenuMerge.Timer / NewColorControl.MenuMerge.MaxTime;
            }
            if (!NewColorImage.gameObject.activeSelf) NewColorImage.gameObject.SetActive(true);
            NewColorImage.fillAmount = Mathf.Abs((NewColorControl.MenuMerge.Timer / NewColorControl.MenuMerge.MaxTime) - 1);
            if (NewColorControl.MenuMerge.Timer <= 0)
            {
                for (int i = 0; i < NewColorControl.MenuMerge.OpenCell; i++) 
                    LabDatas[i].Image.gameObject.SetActive(false);

                _mergeManager.SetEventGet();
                SetButtonText("Get Color");
                NewColorControl.MenuMerge.IsMerge = false;
                NewColorControl.MenuMerge.EventState = true;
            }
        }
    }

    private void ImageFill()
    {
        NewColorImage.gameObject.SetActive(NewColorControl.MenuMerge.Active);
        NewColorImage.color = NewColorControl.PointPreferance.Color;
        NewColorImage.sprite = NewColorControl.MenuMerge.sprite;
    }
    private MergeMenuSave MenuFill()
    {
        NewColorControl.MenuMerge.Active = NewColorImage.gameObject.activeSelf;
        NewColorControl.MenuMerge.sprite = NewColorImage.sprite;
        NewColorControl.MenuMerge.fill = NewColorImage.fillAmount;
        return NewColorControl;
    }


    public void StartMerge()
    {
        _timeSystem.mergeTimers.Add(NewColorControl);
    }

    public LabContentControl GetLabContent() => _labContent;
    public void SetDataInLabContent(LabDataEvent lab) => _labContent.SetData(lab);


    public void SetButtonText(string text) => _button.text = text; 
    private void CheckCellsState()
    {
        int num = 0;
        foreach (Transform tr in _cellPack)
        {
            if (num == NewColorControl.MenuMerge.OpenCell) break;
            if (tr.gameObject.layer != GAMEControler._LAYER_marge)
            {
                Destroy(tr.gameObject);
                num++;
            }
        }
    }
    public int CellCheckFill()
    {
        int num = 0;
        for(int i = 0; i < NewColorControl.MenuMerge.OpenCell; i++)
        {
            if (LabDatas[i].Image.gameObject.activeSelf) num++;
        }
        return num;
    }

    public void GetParameters()
    {
        Color color;
        float timerInSecond = 30;

        float Price = 0;
        float CriticalDrop = 0;
        float PaintPerSec = 0;
        float CoinPerSec = 0;
        ColorClass colorClass = ColorClass.All;

        float SameMergeFaledChance = 0;
        float RealColorChance = 0;
        float Ordinary = 0;
        float Unusual = 0;
        float Rare = 0;
        float Epic = 0;
        float Legendary = 0;
        float Mythical = 0;

        List<MergeMenuSave> points = new List<MergeMenuSave>();
        for (int i = 0; i < CellCheckFill(); i++) points.Add(LabDatas[i].Point);

        Debug.ClearDeveloperConsole();
        float chance = UnityEngine.Random.Range(0.1f, 100f);
        (SameMergeFaledChance, color) = IsColorTheSame();
        print(SameMergeFaledChance);
        if (chance <= SameMergeFaledChance && SameMergeFaledChance != 0)
        {
            print("Failed");
            foreach(MergeMenuSave pc in points)
            {
                if(pc.PointPreferance.Color == color)
                {
                    Price = pc.PointPreferance.pointParameters.Price;
                    CriticalDrop = pc.PointPreferance.pointParameters.CriticalDrop;
                    PaintPerSec = pc.PointPreferance.pointParameters.PaintPerSec;
                    CoinPerSec = pc.PointPreferance.pointParameters.CoinPerSec;
                    colorClass = pc.PointPreferance.pointParameters.BaseClass;

                    RealColorChance = pc.PointPreferance.mergeParameters.RealColorChance;
                    Ordinary = pc.PointPreferance.mergeParameters.Ordinary;
                    Unusual = pc.PointPreferance.mergeParameters.Unusual;
                    Rare = pc.PointPreferance.mergeParameters.Rare;
                    Epic = pc.PointPreferance.mergeParameters.Epic;
                    Legendary = pc.PointPreferance.mergeParameters.Legendary;
                    Mythical = pc.PointPreferance.mergeParameters.Mythical;
                }
            }
        }
        else
        {
            SameMergeFaledChance = UnityEngine.Random.Range(10.0f, 50.0f);
            chance = UnityEngine.Random.Range(0.1f, 100f);
            float num = points.Count;

            for (int i = 0; i < num; i++)
            {
                Ordinary += points[i].PointPreferance.mergeParameters.Ordinary / num;
                Unusual += points[i].PointPreferance.mergeParameters.Unusual / num;
                Rare += points[i].PointPreferance.mergeParameters.Rare / num;
                Epic += points[i].PointPreferance.mergeParameters.Epic / num;
                Legendary += points[i].PointPreferance.mergeParameters.Legendary / num;
                Mythical += points[i].PointPreferance.mergeParameters.Mythical / num;
            }
            colorClass = PointClass(
                    Ordinary,
                    Unusual,
                    Rare,
                    Epic,
                    Legendary,
                    Mythical);

            if (0 <= (RealColorChance = RealColorDrop()))
            {
                int colorCount = ChangeColorBalance(UnityEngine.Random.Range(0.1f, 100f));
                float merge = 1;

                if (colorCount != 0)
                {
                    merge = UnityEngine.Random.Range(0.0f, 1);
                    print($"RealColor | Balance {colorCount}");

                }

                timerInSecond += colorClass.GetHashCode() / (6 - colorCount);

                color = points[0].PointPreferance.Color * merge;
                for(int i = 1; i < num; i++)
                {
                    merge = 1;
                    if (i < colorCount) merge = UnityEngine.Random.Range(0.0f, 1);
                    color = (color + points[i].PointPreferance.Color * merge) / 2;
                }

                for(int i = 0; i < num; i++)
                {
                    Price += points[i].PointPreferance.pointParameters.Price;
                    CriticalDrop += points[i].PointPreferance.pointParameters.CriticalDrop;
                    PaintPerSec += points[i].PointPreferance.pointParameters.PaintPerSec;
                    CoinPerSec += points[i].PointPreferance.pointParameters.CoinPerSec;
                }
            }
            else
            {
                print("Secrete");
            }
        }

        timerInSecond += colorClass.GetHashCode();

        NewColorControl.PointPreferance.Color = new Color(color.r, color.g, color.b, 1);
        NewColorControl.PointPreferance.Sprite = _labContent.PrefabPoint._point.PointPreferance.Sprite;
        NewColorControl.PointPreferance.constParameters = points[0].PointPreferance.constParameters;

        NewColorControl.PointPreferance.pointParameters.Code = 
            $"{Mathf.Round(color.r * 255)} | {Mathf.Round(color.g * 255)} | {Mathf.Round(color.b * 255)}";
        NewColorControl.PointPreferance.pointParameters.MergeLevel = 0;
        NewColorControl.PointPreferance.pointParameters.Price = Price;
        NewColorControl.PointPreferance.pointParameters.SellPrice = 1;
        NewColorControl.PointPreferance.pointParameters.CriticalDrop = CriticalDrop;
        NewColorControl.PointPreferance.pointParameters.PaintPerSec = PaintPerSec;
        NewColorControl.PointPreferance.pointParameters.CoinPerSec = CoinPerSec;
        NewColorControl.PointPreferance.pointParameters.BaseClass = colorClass;
        NewColorControl.PointPreferance.pointParameters.CurrentClass = NewColorControl.PointPreferance.pointParameters.BaseClass;

        NewColorControl.PointPreferance.mergeParameters.SameMergeFaledChance = SameMergeFaledChance;
        NewColorControl.PointPreferance.mergeParameters.RealColorChance = RealColorChance;
        NewColorControl.PointPreferance.mergeParameters.Ordinary = Ordinary;
        NewColorControl.PointPreferance.mergeParameters.Unusual = Unusual;
        NewColorControl.PointPreferance.mergeParameters.Rare = Rare;
        NewColorControl.PointPreferance.mergeParameters.Epic = Epic;
        NewColorControl.PointPreferance.mergeParameters.Legendary = Legendary;
        NewColorControl.PointPreferance.mergeParameters.Mythical = Mythical;

        NewColorControl.MenuMerge.IsMerge = true;
        NewColorControl.MenuMerge.Timer = timerInSecond;
        NewColorControl.MenuMerge.MaxTime = timerInSecond;
        NewColorImage.fillAmount = 0;
        StartMerge();
    }


    public void ColliderControl(bool state)
    {
        for (int i = 0; i < NewColorControl.MenuMerge.OpenCell; i++)
        {
            LabDatas[i]._collider2D.enabled = state;
        }
    }
    private float RealColorDrop()
    {
        float percent = 0;
        for(int i = 0; i < NewColorControl.MenuMerge.OpenCell; i++)
        {
            percent += LabDatas[i].Point.PointPreferance.mergeParameters.RealColorChance;
        }
        return percent / CellCheckFill();
    }
    private (float, Color) IsColorTheSame()
    {
        float percent = 0;
        int sameColor = 0;
        int colors = CellCheckFill();
        List<Color> color = new List<Color>();
        
        for (int i = 0; i < colors; i++)
        {
            MergeMenuSave.Point point = LabDatas[i].Point.PointPreferance;
            for (int j = 0; j < colors; j++)
            {
                if(i != j && point.Color == LabDatas[j].Point.PointPreferance.Color)
                {
                    color.Add(point.Color);
                    percent += point.mergeParameters.SameMergeFaledChance;
                    sameColor++;
                    break;
                }
            }
        }
        float factor = 1;
        if (color.Count == 1)
        {
            factor += ((sameColor / (float)color.Count) / 10);
        }
        percent = (percent * factor) / colors;

        print($"SameColor: {color.Count}\n" +
            $"SamePoint: {sameColor}\n" +
            $"Factor: {factor}\n" +
            $"Result: {percent}");

        if (percent == 0)
        {
            return (percent, Color.clear);
        }
        return (percent, color[UnityEngine.Random.Range(0, color.Count)]);
    }
    private int ChangeColorBalance(float chance)
    {
        if (chance < 50) return 0;
        else chance -= 50;
        if (chance < 20) return 2;
        else chance -= 20;
        if (chance < 15) return 3;
        else chance -= 15;
        if (chance < 10) return 4;
        else chance -= 10;
        if (chance < 5) return 5;
        else chance -= 5;

        return 0;
    }
    private ColorClass PointClass(float ordinary, float unusual,
        float rare, float epic, float legendary, float mythical)
    {
        float chance = UnityEngine.Random.Range(0.001f, 100);

        if (chance < ordinary) return ColorClass.Common;
        else chance -= ordinary;

        if (chance < unusual) return ColorClass.Unusual;
        else chance -= unusual;

        if (chance < rare) return ColorClass.Rare;
        else chance -= rare;

        if (chance < epic) return ColorClass.Epic;
        else chance -= epic;

        if (chance < legendary) return ColorClass.Legendary;
        else chance -= legendary;

        if (chance < mythical) return ColorClass.Mythical;
        else chance -= mythical;

        return ColorClass.All;
    }


    [ContextMenu("TimesUp")]
    private void Finish() => NewColorControl.MenuMerge.Timer = 1;

    private void OnEnable()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.MenuMerge.OpenCell != 0)
        {
            JsonSaverBase.FileDel(JsonSaverBase.MergeFolder, GetInstanceID());
        }
    }
    private void OnDisable()
    {
        if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.MenuMerge.OpenCell != 0)
        {
            JsonSaverBase.SaveInfo(MenuFill(), JsonSaverBase.MergeFolder, GetInstanceID());
        }
    }

#if PLATFORM_ANDROID && !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.OpenCell != 0)
            {
                JsonSaverBase.FileDel(JsonSaverBase.MergeFolder, GetInstanceID());
            }
        }
        else
        {
            if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.OpenCell != 0)
            {
                JsonSaverBase.SaveInfo(MenuFill(), JsonSaverBase.MergeFolder, GetInstanceID());
            }
        }
    }
#endif
    private void OnApplicationQuit()
    {
        if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.MenuMerge.OpenCell != 0)
        {
            JsonSaverBase.SaveInfo(MenuFill(), JsonSaverBase.MergeFolder, GetInstanceID());
        }
    }
}
