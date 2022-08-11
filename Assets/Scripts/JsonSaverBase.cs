using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class JsonSaverBase : MonoBehaviour
{
    [Header("Additional Preferance")]
    [SerializeField] private PointControl _pointPrefab;
    public const string PointFolder = "Point";
    public const string ShopFolder = "Shop";
    public const string MergeFolder = "Merge";
    public const string MainSettingsFolder = "Settings";
    private void Awake()
    {
        string path = CurrentPlatform();
        if (!Directory.Exists(Path.Combine(path,PointFolder))) Directory.CreateDirectory(Path.Combine(path, PointFolder));
        if (!Directory.Exists(Path.Combine(path, ShopFolder))) Directory.CreateDirectory(Path.Combine(path, ShopFolder));
        if (!Directory.Exists(Path.Combine(path, MergeFolder))) Directory.CreateDirectory(Path.Combine(path, MergeFolder));
        if (!Directory.Exists(Path.Combine(path, MainSettingsFolder))) Directory.CreateDirectory(Path.Combine(path, MainSettingsFolder));

        CreatePoint();
    }

    public static void SaveInfo(MergeMenuSave save, string Folder, int ID)
    {
        MergeMenuSave mergeMenu = new MergeMenuSave();
        mergeMenu.MenuMerge.OpenCell = save.MenuMerge.OpenCell;
        mergeMenu.MenuMerge.NewColor = save.MenuMerge.NewColor;
        mergeMenu.MenuMerge.MaxTime = save.MenuMerge.MaxTime;
        mergeMenu.MenuMerge.IsMerge = save.MenuMerge.IsMerge;
        mergeMenu.MenuMerge.Timer = save.MenuMerge.Timer;
        mergeMenu.MenuMerge.EventState = save.MenuMerge.EventState;

        mergeMenu.PointPreferance.Color = save.PointPreferance.Color;
        mergeMenu.PointPreferance.Sprite = save.PointPreferance.Sprite;
        mergeMenu.PointPreferance.Material = save.PointPreferance.Material;
        mergeMenu.PointPreferance.Target = save.PointPreferance.Target;
        mergeMenu.PointPreferance.Parent = save.PointPreferance.Parent;
        mergeMenu.PointPreferance.pointParameters = save.PointPreferance.pointParameters;
        mergeMenu.PointPreferance.mergeParameters = save.PointPreferance.mergeParameters;
        mergeMenu.PointPreferance.constParameters = save.PointPreferance.constParameters;

        mergeMenu.MenuMerge.CellIsOpen = save.MenuMerge.CellIsOpen;
        mergeMenu.MenuMerge.Active = save.MenuMerge.Active;
        mergeMenu.MenuMerge.sprite = save.MenuMerge.sprite;
        mergeMenu.MenuMerge.fill = save.MenuMerge.fill;

        mergeMenu.Settings.AbsentTime = save.Settings.AbsentTime;
        mergeMenu.Settings.Coins = save.Settings.Coins;
        mergeMenu.Settings.CoinsPerSec = save.Settings.CoinsPerSec;
        mergeMenu.Settings.Paint = save.Settings.Paint;
        mergeMenu.Settings.PaintPerSec = save.Settings.PaintPerSec;
        mergeMenu.Settings.Colors = save.Settings.Colors;

        mergeMenu.ClassUpdate.Active = save.ClassUpdate.Active;
        mergeMenu.ClassUpdate.IsReady = save.ClassUpdate.IsReady;
        mergeMenu.ClassUpdate.Devider = save.ClassUpdate.Devider;
        mergeMenu.ClassUpdate.CurrentValue = save.ClassUpdate.CurrentValue;
        mergeMenu.ClassUpdate.NewValue = save.ClassUpdate.NewValue;
        mergeMenu.ClassUpdate.BorderColorBefore = save.ClassUpdate.BorderColorBefore;

        File.WriteAllText(Path.Combine(CurrentPlatform(), Folder, $"{ID}.txt"), JsonUtility.ToJson(mergeMenu));
    }
    public static void Loadinfo(MergeMenuSave save, string Folder, int ID)
    {
        string json = "";
        string data = Path.Combine(CurrentPlatform(), Folder, $"{ID}.txt");
        if (File.Exists(data))
        {
            json = File.ReadAllText(data);
        }
        MergeMenuSave mergeMenu = JsonUtility.FromJson<MergeMenuSave>(json);

        save.MenuMerge.OpenCell = mergeMenu.MenuMerge.OpenCell;
        save.MenuMerge.NewColor = mergeMenu.MenuMerge.NewColor;
        save.MenuMerge.MaxTime = mergeMenu.MenuMerge.MaxTime;
        save.MenuMerge.IsMerge = mergeMenu.MenuMerge.IsMerge;
        save.MenuMerge.Timer = mergeMenu.MenuMerge.Timer;
        save.MenuMerge.EventState = mergeMenu.MenuMerge.EventState;

        save.PointPreferance.Color = mergeMenu.PointPreferance.Color;
        save.PointPreferance.Sprite = mergeMenu.PointPreferance.Sprite;
        save.PointPreferance.Material = mergeMenu.PointPreferance.Material;
        save.PointPreferance.Target = mergeMenu.PointPreferance.Target;
        save.PointPreferance.Parent = mergeMenu.PointPreferance.Parent;
        save.PointPreferance.pointParameters = mergeMenu.PointPreferance.pointParameters;
        save.PointPreferance.mergeParameters = mergeMenu.PointPreferance.mergeParameters;
        save.PointPreferance.constParameters = mergeMenu.PointPreferance.constParameters;

        save.MenuMerge.CellIsOpen = mergeMenu.MenuMerge.CellIsOpen;
        save.MenuMerge.Active = mergeMenu.MenuMerge.Active;
        save.MenuMerge.sprite = mergeMenu.MenuMerge.sprite;
        save.PointPreferance.Color = mergeMenu.PointPreferance.Color;
        save.MenuMerge.fill = mergeMenu.MenuMerge.fill;

        save.Settings.AbsentTime = mergeMenu.Settings.AbsentTime;
        save.Settings.Coins = mergeMenu.Settings.Coins;
        save.Settings.CoinsPerSec = mergeMenu.Settings.CoinsPerSec;
        save.Settings.Paint = mergeMenu.Settings.Paint;
        save.Settings.PaintPerSec = mergeMenu.Settings.PaintPerSec;
        save.Settings.Colors = mergeMenu.Settings.Colors;

        save.ClassUpdate.Active = mergeMenu.ClassUpdate.Active;
        save.ClassUpdate.IsReady = mergeMenu.ClassUpdate.IsReady;
        save.ClassUpdate.Devider = mergeMenu.ClassUpdate.Devider;
        save.ClassUpdate.CurrentValue = mergeMenu.ClassUpdate.CurrentValue;
        save.ClassUpdate.NewValue = mergeMenu.ClassUpdate.NewValue;
        save.ClassUpdate.BorderColorBefore = mergeMenu.ClassUpdate.BorderColorBefore;

        File.Delete(data);
    }

    private void CreatePoint()
    {
        string[] data = Directory.GetFiles(Path.Combine(CurrentPlatform(), PointFolder));
        if (data.Length != 0)
        {
            for (int i = 0; i < data.Length; i++)
            {
                string json;
                MergeMenuSave save = new MergeMenuSave();
                if (FileTypeChecker(data[i], "txt")) json = File.ReadAllText(data[i]);
                else continue;
                save = JsonUtility.FromJson<MergeMenuSave>(json);

                PointControl pointControl = Instantiate(_pointPrefab, save.PointPreferance.Target.position,
                    Quaternion.identity, save.PointPreferance.Parent);

                pointControl._point.PointPreferance.Color = save.PointPreferance.Color;
                pointControl._point.PointPreferance.Sprite = save.PointPreferance.Sprite;
                pointControl._point.PointPreferance.Material = save.PointPreferance.Material;
                pointControl._point.PointPreferance.Target = save.PointPreferance.Target;

                pointControl._point.PointPreferance.pointParameters = save.PointPreferance.pointParameters;
                pointControl._point.PointPreferance.constParameters = save.PointPreferance.constParameters;
                pointControl._point.PointPreferance.mergeParameters = save.PointPreferance.mergeParameters;

                File.Delete(data[i]);
            }
        }
    }


    public static bool FileTypeChecker(string currentType, string needType)
    {
        int Long = currentType.Length;
        string type = "";

        for (int j = 1; j < needType.Length + 1; j++)
        {
            type = $"{type}{currentType[Long - j]}";
        }
        if (type == needType) return true;
        else return false;
    }
    
    public static bool FileExist(string Folder, int ID) => 
        File.Exists(Path.Combine(CurrentPlatform(), Folder, $"{ID}.txt"));
    public static void FileDel(string Folder, int ID) =>
        File.Delete(Path.Combine(CurrentPlatform(), Folder, $"{ID}.txt"));
    public static string CurrentPlatform()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
    return Application.temporaryCachePath;
#elif UNITY_EDITOR
        return Application.streamingAssetsPath;
#endif
    }
}
[System.Serializable]
public class MergeMenuSave
{
    public Point PointPreferance = new Point();
    public MergeMenu MenuMerge = new MergeMenu();
    public MainSettings Settings = new MainSettings();
    public ClassUpdateMenu ClassUpdate = new ClassUpdateMenu();
    
    [System.Serializable]
    public class Point
    {
        public Color Color;
        public Transform Target;
        public Transform Parent;

        public Sprite Sprite;
        public Material Material;

        public MergePointParameters mergeParameters = new MergePointParameters();
        public SimplePointParameters pointParameters = new SimplePointParameters();
        public ConstParameters constParameters = new ConstParameters();
    }

    [System.Serializable]
    public class MergeMenu
    {
        public bool CellIsOpen;
        public bool Active;
        public Sprite sprite;
        public float fill;

        public int OpenCell;
        public Image NewColor;
        public float MaxTime;
        public bool IsMerge;
        public float Timer;
        public bool EventState;
    }

    [System.Serializable]
    public class MainSettings
    {
        public string AbsentTime;
        public float Coins;
        public float Paint;
        public float CoinsPerSec;
        public float PaintPerSec;
        public List<Fabric> Colors = new List<Fabric>();
    }
    [System.Serializable]
    public class ClassUpdateMenu
    {
        public bool Active;
        public bool IsReady;
        public double Devider;
        public double CurrentValue;
        public double NewValue;
        public Color BorderColorBefore;
    }
}
