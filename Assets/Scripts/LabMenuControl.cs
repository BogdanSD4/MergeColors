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

    public List<LabDataEvent> LabDatas = new List<LabDataEvent>();

    public Image NewColorImage;
    public MergeMenuSave NewColorControl = new MergeMenuSave();

    private int colorInMenu;
    public static float MaxFactorSameColor = 2;
    public static float MaxFactorRealColor = 2;
    public float priceByBetterPoint;

    private void Awake()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()))
        {
            JsonSaverBase.Loadinfo(NewColorControl, JsonSaverBase.MergeFolder, GetInstanceID());
            ImageFill();
            StartMerge();
            CheckCellsState(NewColorControl.MenuMerge.OpenCell);
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
            if (!NewColorControl.MenuMerge.IsMerge) SetButtonText("Start");
        }
        else
        {
            _mergeManager.SetEventGet();
            SetButtonText("Get Color");
        }
    }

    private void Update()
    {
        if (NewColorControl.MenuMerge.IsMerge)
        {
            var ts = TimeSpan.FromSeconds(NewColorControl.MenuMerge.Timer);
            SetButtonText(string.Format("{0}:{1}:{2}:{3}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds));
            for (int i = 0; i < LabDatas.Count; i++)
            {
                LabDatas[i].Image.fillAmount = NewColorControl.MenuMerge.Timer / NewColorControl.MenuMerge.MaxTime;
            }
            if (!NewColorImage.gameObject.activeSelf) NewColorImage.gameObject.SetActive(true);
            NewColorImage.fillAmount = Mathf.Abs((NewColorControl.MenuMerge.Timer / NewColorControl.MenuMerge.MaxTime) - 1);
            if (NewColorControl.MenuMerge.Timer <= 0)
            {
                for (int i = 0; i < LabDatas.Count; i++)
                    LabDatas[i].Image.gameObject.SetActive(false);

                _mergeManager.SetEventGet();
                SetButtonText("Get Color");
                NewColorControl.MenuMerge.IsMerge = false;
                NewColorControl.MenuMerge.EventState = true;
            }
        }
    }
    public int UpdateMenu(int num)
    {
        CheckCellsState(num);
        return NewColorControl.MenuMerge.OpenCell += num;
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
    public void AddData(LabDataEvent lab) => LabDatas.Add(lab);
    public void RemoveData(LabDataEvent lab) => LabDatas.Remove(lab);
    public void RemoveAllData() { for (int i = 0; i < LabDatas.Count; i++) LabDatas.Remove(LabDatas[i]); }
    public int ColorsInMenu() => LabDatas.Count;


    public void SetButtonText(string text) => _button.text = text; 
    private void CheckCellsState(int openCell)
    {
        int counter = 0;
        int boxEnable = 0;
        foreach (Transform tr in _cellPack)
        {
            if (counter == openCell) break;
            if (tr.gameObject.layer != GAMEControler._LAYER_marge)
            {
                Destroy(tr.gameObject);
                counter++;
            }
            else
            {
                if (boxEnable == openCell) continue;
                else 
                {
                    BoxCollider2D box = tr.GetComponent<BoxCollider2D>();
                    if (!box.enabled)
                    {
                        tr.GetComponent<BoxCollider2D>().enabled = true;
                        print(box.enabled);
                        boxEnable++;
                    }
                }
            }
        }
    }

    public void GetParameters()
    {
        colorInMenu = LabDatas.Count;

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
        for (int i = 0; i < colorInMenu; i++) points.Add(LabDatas[i].Point);
        int num = points.Count;

        Debug.ClearDeveloperConsole();
        float chance = UnityEngine.Random.Range(0.1f, 100f);
        (SameMergeFaledChance, color) = IsColorTheSame();
        if (chance <= SameMergeFaledChance && SameMergeFaledChance != 0)
        {
            SameMergeFaledChance = 0;
            int divider = 1;
            for (int i = 0; i < num; i++)
            {
                if (i == num - 1) divider = num;

                Price = (Price + points[i].PointPreferance.pointParameters.Price) / divider;
                CriticalDrop = (CriticalDrop + points[i].PointPreferance.pointParameters.CriticalDrop) / divider;
                PaintPerSec = (PaintPerSec + points[i].PointPreferance.pointParameters.PaintPerSec) / divider;
                CoinPerSec = (CoinPerSec + points[i].PointPreferance.pointParameters.CoinPerSec) / divider;

                SameMergeFaledChance = (SameMergeFaledChance + points[i].PointPreferance.mergeParameters.SameMergeFaledChance) / divider;
                RealColorChance = (RealColorChance + points[i].PointPreferance.mergeParameters.RealColorChance) / divider;
                Ordinary = (Ordinary + points[i].PointPreferance.mergeParameters.Ordinary) / divider;
                Unusual = (Unusual + points[i].PointPreferance.mergeParameters.Unusual) / divider;
                Rare = (Rare + points[i].PointPreferance.mergeParameters.Rare) / divider;
                Epic = (Epic + points[i].PointPreferance.mergeParameters.Epic) / divider;
                Legendary = (Legendary + points[i].PointPreferance.mergeParameters.Legendary) / divider;
                Mythical = (Mythical + points[i].PointPreferance.mergeParameters.Mythical) / divider;
            }
            if (SameMergeFaledChance == 100) SameMergeFaledChance--;

            colorClass = PointClass(
                Ordinary,
                Unusual,
                Rare,
                Epic,
                Legendary,
                Mythical);

            bool isBetter = false;
            float factor = UnityEngine.Random.Range(0.5f, MaxFactorSameColor);
            float factorDivider = (MaxFactorSameColor - 0.5f) / 10;

            if (factor > 1) isBetter = true;

            if (isBetter)
            {
                priceByBetterPoint = Price * 5 * factor;

                Price *= factor;
                PaintPerSec += (PaintPerSec * (factor / factorDivider / 100));
                CoinPerSec += (CoinPerSec * (factor / factorDivider / 100));

                SameMergeFaledChance -= (SameMergeFaledChance * ((100 - SameMergeFaledChance) / 100));
            }
            else
            {
                priceByBetterPoint = 0;

                Price += Price - (Price * factor);
                PaintPerSec -= (PaintPerSec * (factor / factorDivider / 100));
                CoinPerSec -= (CoinPerSec * (factor / factorDivider / 100));

                SameMergeFaledChance += (SameMergeFaledChance * ((100 - SameMergeFaledChance) / 100));
            }

            CriticalDrop *= factor;
            timerInSecond += colorClass.GetHashCode() * factor;
        }
        else
        {
            chance = UnityEngine.Random.Range(0.1f, 100f);

            SameMergeFaledChance = 0;
            int divider = 1;
            for (int i = 0; i < num; i++)
            {
                if (i == num - 1) divider = num;

                Price = (Price + points[i].PointPreferance.pointParameters.Price) / divider;
                CriticalDrop = (CriticalDrop + points[i].PointPreferance.pointParameters.CriticalDrop) / divider;
                PaintPerSec = (PaintPerSec + points[i].PointPreferance.pointParameters.PaintPerSec) / divider;
                CoinPerSec = (CoinPerSec + points[i].PointPreferance.pointParameters.CoinPerSec) / divider;

                SameMergeFaledChance = (SameMergeFaledChance + points[i].PointPreferance.mergeParameters.SameMergeFaledChance) / divider;
                RealColorChance = (RealColorChance + points[i].PointPreferance.mergeParameters.RealColorChance) / divider;
                Ordinary = (Ordinary + points[i].PointPreferance.mergeParameters.Ordinary) / divider;
                Unusual = (Unusual + points[i].PointPreferance.mergeParameters.Unusual) / divider;
                Rare = (Rare + points[i].PointPreferance.mergeParameters.Rare) / divider;
                Epic = (Epic + points[i].PointPreferance.mergeParameters.Epic) / divider;
                Legendary = (Legendary + points[i].PointPreferance.mergeParameters.Legendary) / divider;
                Mythical = (Mythical + points[i].PointPreferance.mergeParameters.Mythical) / divider;
            }
            if (SameMergeFaledChance == 100) SameMergeFaledChance--;

            colorClass = PointClass(
                    Ordinary,
                    Unusual,
                    Rare,
                    Epic,                    
                    Legendary,
                    Mythical);

            if (chance <= RealColorChance)
            {
                float factor = UnityEngine.Random.Range(0.5f, MaxFactorRealColor);
                int colorCount = ChangeColorBalance(UnityEngine.Random.Range(0.1f, 100));
                float chance_0 = 100 - RealColorChance;
                float factorDivider = (MaxFactorRealColor - 0.5f) / 10;
                float merge = 1;


                if (UnityEngine.Random.Range(0.1f, 100) < chance_0)
                {
                    color = Color.black;
                    int divider_0 = 1;
                    for (int i = 0; i < num; i++)
                    {
                        merge = 1;
                        if (i < colorCount) merge = UnityEngine.Random.Range(0.2f, 1);
                        color = (color + (points[i].PointPreferance.Color * merge)) / divider_0;
                        if (divider_0 != 2) divider_0 = 2;
                        print(merge);
                    }
                    print("Balance");
                    RealColorChance += (RealColorChance * ((100 - RealColorChance) / 100));
                }
                else
                {
                    colorCount = 0;
                    for (int i = 0; i < num; i++) color += points[i].PointPreferance.Color;
                    RealColorChance -= (RealColorChance * ((100 - RealColorChance) / 100));
                    print("Real");
                }

                if (factor > 1)
                {
                    priceByBetterPoint = Price * 5 * factor;

                    Price *= factor;
                    PaintPerSec += (PaintPerSec * (factor / factorDivider / 100));
                    CoinPerSec += (CoinPerSec * (factor / factorDivider / 100));

                    
                }
                else
                {
                    priceByBetterPoint = 0;

                    Price += Price - (Price * factor);
                    PaintPerSec -= (PaintPerSec * (factor / factorDivider / 100));
                    CoinPerSec -= (CoinPerSec * (factor / factorDivider / 100));

                    
                }

                CriticalDrop *= factor;
                timerInSecond += (colorClass.GetHashCode() / (5 - colorCount)) * factor;
            }
            else
            {
                float chance_1 = UnityEngine.Random.Range(0.1f, 100);

                for (int i = 0; i < 1;) 
                {
                    if (chance_1 <= Ordinary) { print("O"); break; }
                    else chance_1 -= Ordinary;
                    if (chance_1 <= Unusual) { print("U"); break; }
                    else chance_1 -= Unusual;
                    if (chance_1 <= Rare) { print("R"); break; }
                    else chance_1 -= Rare;
                    if (chance_1 <= Epic) { print("E"); break; }
                    else chance_1 -= Epic;
                    if (chance_1 <= Legendary) { print("L"); break; }
                    else chance_1 -= Legendary;
                    if (chance_1 <= Mythical) { print("M"); break; }
                }
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
        for (int i = 0; i < colorInMenu; i++)
        {
            LabDatas[i]._collider2D.enabled = state;
        }
    }
    private (float, Color) IsColorTheSame()
    {
        float percent = 0;
        int sameColor = 0;
        List<Color> color = new List<Color>();
        
        for (int i = 0; i < colorInMenu; i++)
        {
            MergeMenuSave.Point point = LabDatas[i].Point.PointPreferance;
            for (int j = 0; j < colorInMenu; j++)
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
        factor += ((sameColor / (float)color.Count) / 10);

        if (percent == 0)
        {
            return (percent, Color.clear);
        }
        else percent = (percent * factor) / colorInMenu;

        return (percent, color[UnityEngine.Random.Range(0, color.Count)]);
    }
    private int ChangeColorBalance(float chance)
    {
        if (chance < 50) return 0;
        else chance -= 50;
        if (chance < 20) return 1;
        else chance -= 20;
        if (chance < 15) return 2;
        else chance -= 15;
        if (chance < 10) return 3;
        else chance -= 10;
        if (chance < 5) return 4;

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
            if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.MenuMerge.OpenCell != 0)
            {
                JsonSaverBase.FileDel(JsonSaverBase.MergeFolder, GetInstanceID());
            }
        }
        else
        {
            if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && NewColorControl.MenuMerge.OpenCell != 0)
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
