using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MargeManager : MonoBehaviour
{
    [SerializeField] private LabMenuControl _labMenu;
    [SerializeField] private Button _event;

    public LabMenuControl GetLabMenu() => _labMenu;

    [ContextMenu("SetStart")]
    public void SetEventStart()
    {
        RemoveEvent();
        _event.onClick.AddListener(MergeButtonStart);
    }
    public void SetEventGet()
    {
        RemoveEvent();
        _event.onClick.AddListener(MergeButtonGet);
    }
    private void RemoveEvent()
    {
        _event.onClick.RemoveAllListeners();
    }


    public void MergeButtonStart()
    {
        if (!_labMenu.NewColorControl.MenuMerge.IsMerge)
        {
            if (_labMenu.CellCheckFill() < 2)
            {
                GAMEControler.ConsoleEnter("For merge you need two (or more) Colors");
                return;
            }
            else
            {
                _labMenu.ColliderControl(false);
                _labMenu.GetParameters();
                _labMenu.NewColorImage.color = _labMenu.NewColorControl.PointPreferance.Color;
            }
        }
        else GAMEControler.ConsoleEnter("Split not complete");
    }

    public void MergeButtonGet()
    {
        _labMenu.GetLabContent().MergeButtonGet(_labMenu.NewColorControl, this);
    }

    public void ClearMergeMenu()
    {
        _labMenu.NewColorImage.gameObject.SetActive(false);
        _labMenu.ColliderControl(true);
        _labMenu.NewColorControl.MenuMerge.IsMerge = false;
        SetEventStart();
        _labMenu.SetButtonText($"Start");
        _labMenu.NewColorControl.MenuMerge.EventState = false;
    }
}
