﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabDataEvent : MonoBehaviour
{
    [SerializeField] private LabMenuControl labMenu;
    public Image Image;
    public MergeMenuSave Point = new MergeMenuSave();
    public BoxCollider2D _collider2D;
    private void Awake()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()))
        {
            JsonSaverBase.Loadinfo(Point, JsonSaverBase.MergeFolder, GetInstanceID());
            ImageFill();
        }
    }

    private void ImageFill()
    {
        AddColorToMenu();
        Image.gameObject.SetActive(Point.MenuMerge.Active);
        Image.color = Point.PointPreferance.Color;
        Image.sprite = Point.MenuMerge.sprite;
    }
    private MergeMenuSave MenuFill()
    {
        Point.MenuMerge.Active = Image.gameObject.activeSelf;
        Point.PointPreferance.Color = Image.color;
        Point.MenuMerge.sprite = Image.sprite;
        return Point;
    }


    public bool ShowColliderState() => Image.gameObject.activeSelf;
    public void AddColorToMenu() => labMenu.AddData(this);
    public void RemoveColorOutMenu() => labMenu.RemoveData(this);

    private void OnEnable()
    {
        if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && Point.MenuMerge.CellIsOpen)
        {
            JsonSaverBase.FileDel(JsonSaverBase.MergeFolder, GetInstanceID());
        }
    }
    private void OnDisable()
    {
        if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && Point.MenuMerge.CellIsOpen)
        {
            JsonSaverBase.SaveInfo(MenuFill(), JsonSaverBase.MergeFolder, GetInstanceID());
        }
    }

#if PLATFORM_ANDROID && !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && Point.MenuMerge.CellIsOpen)
            {
                JsonSaverBase.FileDel(JsonSaverBase.MergeFolder, GetInstanceID());
            }
        }
        else
        {
            if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && Point.MenuMerge.CellIsOpen)
            {
                JsonSaverBase.SaveInfo(MenuFill(), JsonSaverBase.MergeFolder, GetInstanceID());
            }
        }
    }
#endif

    private void OnApplicationQuit()
    {
        if (!JsonSaverBase.FileExist(JsonSaverBase.MergeFolder, GetInstanceID()) && Point.MenuMerge.CellIsOpen)
        {
            JsonSaverBase.SaveInfo(MenuFill(), JsonSaverBase.MergeFolder, GetInstanceID());
        }
    }
}
