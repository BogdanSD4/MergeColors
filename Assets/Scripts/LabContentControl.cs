using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class LabContentControl : MonoBehaviour
{
    [SerializeField] private GAMEControler _gameControler;
    public PointControl PrefabPoint;
    [Space]
    [Header("AfterMergeMenu")]
    [SerializeField] private GameObject _newColorMenu;
    [SerializeField] private GameObject _returnColorMenu;
    [SerializeField] private TextMeshProUGUI _afterMergePointCharacters;
    [SerializeField] private RectTransform _content;
    [Space]
    [Header("ColorCompareMenu")]
    [SerializeField] private GameObject _colorCompare;

    private MergeMenuSave _mergeMenu = new MergeMenuSave();
    private MergeMenuSave _marketMenu = new MergeMenuSave();

    private MargeManager _currentPoint;
    private LabDataEvent _dataLab;

    public void MergeButtonGet(MergeMenuSave point, MargeManager manager)
    {
        _mergeMenu = point;
        _currentPoint = manager;
        _content.localPosition = Vector2.zero;

        _afterMergePointCharacters.text = GAMEControler.FillInfoText(point);

        GAMEControler.MenuOpen(_newColorMenu);
    }

    public void SellNewColor()
    {
        _gameControler.CoinCall = 
            _currentPoint.GetLabMenu().NewColorControl.PointPreferance.pointParameters.Price;
        GAMEControler.MenuClose(_newColorMenu);
        _currentPoint.ClearMergeMenu();
    }
    public void AddColorToMarket()
    {
        bool value;
        (value, _marketMenu) = 
            _gameControler.CheckColorInMaret(_currentPoint.GetLabMenu().NewColorControl.PointPreferance.Color);
        if (value)
        {
            GAMEControler.MenuOpen(_colorCompare);
        }
        else
        {
            MergeMenuSave save = new MergeMenuSave();
            MergeMenuSave currentPoint = _currentPoint.GetLabMenu().NewColorControl;

            GAMEControler.FillParameters(save, currentPoint);

            _gameControler.AddColorToMarket(save);
            _currentPoint.ClearMergeMenu();
            GAMEControler.MenuClose(_newColorMenu);
        }
        
    }

    public void Replace()
    {
        _gameControler.ReplaceColorInMarket(_currentPoint.GetLabMenu().NewColorControl);
        _currentPoint.ClearMergeMenu();
        CloseMenu(_newColorMenu);
        CloseMenu(_colorCompare);
    }
    public void Compare()
    {
        SimplePointParameters point = _mergeMenu.PointPreferance.pointParameters;
        MergePointParameters pointMerge = _mergeMenu.PointPreferance.mergeParameters;

        SimplePointParameters pointMarket = _marketMenu.PointPreferance.pointParameters;
        MergePointParameters pointMergeMarket = _marketMenu.PointPreferance.mergeParameters;

        _afterMergePointCharacters.text =
            $"Code: {point.Code}\n" +
            $"Price: {System.Math.Round(point.Price,2)}  " +
                $"<color={CompareHelper(point.Price, pointMarket.Price)}>" +
                $"{System.Math.Round(pointMarket.Price,2)}</color>\n" +
            $"SellPrice: {System.Math.Round(point.SellPrice,2)}  " +
                $"<color={CompareHelper(pointMarket.SellPrice, point.SellPrice)}>" +
                $"{System.Math.Round(pointMarket.SellPrice,2)}</color>\n" +
            $"CriticalDrop: {System.Math.Round(point.CriticalDrop,2)}  " +
                $"<color={CompareHelper(pointMarket.CriticalDrop, point.CriticalDrop)}>" +
                $"{System.Math.Round(pointMarket.CriticalDrop,2)}</color>\n" +
            $"PaintPerSec: {System.Math.Round(point.PaintPerSec,2)}  " +
                $"<color={CompareHelper(pointMarket.PaintPerSec, point.PaintPerSec)}>" +
                $"{System.Math.Round(pointMarket.PaintPerSec,2)}</color>\n" +
            $"CoinPerSec: {System.Math.Round(point.CoinPerSec,2)}  " +
                $"<color={CompareHelper(pointMarket.CoinPerSec, point.CoinPerSec)}>" +
                $"{System.Math.Round(pointMarket.CoinPerSec,2)}</color>\n" +
            $"BaseClass: {point.BaseClass}\n" +
            $"CurrentClass: {point.CurrentClass}\n" +

            $"SameMergeChance: {System.Math.Round(pointMerge.SameMergeFaledChance, 2)}%  " +
                $"<color={CompareHelper(pointMerge.SameMergeFaledChance, pointMergeMarket.SameMergeFaledChance)}>" +
                $"{System.Math.Round(pointMergeMarket.SameMergeFaledChance, 2)}%</color>\n" +
            $"RealColorDropChance: {System.Math.Round(pointMerge.RealColorChance, 2)}%  " +
                $"<color={CompareHelper(pointMerge.RealColorChance, pointMergeMarket.RealColorChance)}>" +
                $"{System.Math.Round(pointMergeMarket.RealColorChance, 2)}%</color>\n" +
            $"Ordinary: {System.Math.Round(pointMerge.Ordinary, 2)}%  " +
                $"<color={CompareHelper(pointMergeMarket.Ordinary, pointMerge.Ordinary)}>" +
                $"{System.Math.Round(pointMergeMarket.Ordinary, 2)}%</color>\n" +
            $"Unusual: {System.Math.Round(pointMerge.Unusual, 2)}%  " +
                $"<color={CompareHelper(pointMergeMarket.Unusual, pointMerge.Unusual)}>" +
                $"{System.Math.Round(pointMergeMarket.Unusual, 2)}%</color>\n" +
            $"Rare: {System.Math.Round(pointMerge.Rare, 2)}%  " +
                $"<color={CompareHelper(pointMergeMarket.Rare, pointMerge.Rare)}>" +
                $"{System.Math.Round(pointMergeMarket.Rare, 2)}%</color>\n" +
            $"Epic: {System.Math.Round(pointMerge.Epic, 2)}%  " +
                $"<color={CompareHelper(pointMergeMarket.Epic, pointMerge.Epic)}>" +
                $"{System.Math.Round(pointMergeMarket.Epic, 2)}%</color>\n" +
            $"Legendary: {System.Math.Round(pointMerge.Legendary, 2)}%  " +
                $"<color={CompareHelper(pointMergeMarket.Legendary, pointMerge.Legendary)}>" +
                $"{System.Math.Round(pointMergeMarket.Legendary, 2)}%</color>\n" +
            $"Mythical: {System.Math.Round(pointMerge.Mythical, 2)}%  " +
                $"<color={CompareHelper(pointMergeMarket.Mythical, pointMerge.Mythical)}>" +
                $"{System.Math.Round(pointMergeMarket.Mythical, 2)}%</color>\n";

        CloseMenu(_colorCompare);
    }
    private string CompareHelper(float more, float less)
    {
        if (more > less) return "green";
        else if (more == less) return "black";
        else return "red";
    }

    public void CloseMenu(GameObject menu) => GAMEControler.MenuClose(menu);

    public void ReturnColor()
    {
        MergeMenuSave save = _dataLab.Point;
        Transform result = _gameControler.LabMenuFindOpenCell();
        if (result == null) GAMEControler.ConsoleEnter("No space");
        else
        {
            PointControl point = Instantiate(PrefabPoint, _dataLab.transform.position,
                Quaternion.identity, save.PointPreferance.constParameters.LabPointPack);

            point._point.PointPreferance.Color = save.PointPreferance.Color;
            point._point.PointPreferance.Target = result;
            point._point.PointPreferance.pointParameters = save.PointPreferance.pointParameters;
            point._point.PointPreferance.mergeParameters = save.PointPreferance.mergeParameters;
            point._point.PointPreferance.constParameters = save.PointPreferance.constParameters;
        }

        _dataLab.RemoveColorOutMenu();
        _dataLab.Image.gameObject.SetActive(false);
        _dataLab.GetComponent<Collider2D>().enabled = true;
        CloseMenu(_returnColorMenu);
    }


    public void SetData(LabDataEvent lab)
    {
        _dataLab = lab;
        GAMEControler.MenuOpen(_returnColorMenu);
    }

}

