using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class SetColor : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private TextMeshProUGUI _text;
    [Space]
    public bool CellIsOpen;
    public Color Color;
    public Transform Point;
    public float MergeCount;
    [Space]
    public UnityEvent Check;
    public UnityEvent Reset;
    private Image image;

    private void Start()
    {
        if (PlayerPrefs.HasKey($"CellIsOpen({GetInstanceID()})")) CellIsOpen = true;
        if (CellIsOpen)
        {
            Reset.AddListener(this.ResetParameters);
            Check.AddListener(this.CurrentColor);
        }
        else
        {
            EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
            eventTrigger.triggers.Add(entry);
        }

        image = GetComponent<Image>();
        CheckCellState();
    }
    private void CheckCellState()
    {
        if (!CellIsOpen)
        {
            image.color = Color.gray;
            gameObject.layer = GAMEControler._LAYER_close;
        }
        else
        {
            Destroy(GetComponent<EventTrigger>());
        }
    }
    public void OpenCell(bool requst)
    {
        if (requst)
        {
            image.color = Color.white;
            gameObject.layer = GAMEControler._LAYER_open;
            GAMEControler.OpenPrice *= 5;

            CellIsOpen = true;
            Reset.AddListener(this.ResetParameters);
            Check.AddListener(this.CurrentColor);

            PlayerPrefs.SetString($"CellIsOpen({GetInstanceID()})", "open");
        }
        GAMEControler.MenuClose(_menu);
    }
    public void CurrentColor()
    {
        if (transform == GAMEControler.CellOutline) image.color = Color.gray;
        else image.color = Color.white;
    } 
    public void ResetParameters()
    {
        transform.GetComponent<Image>().color = Color.white;
        gameObject.layer = GAMEControler._LAYER_open;
        Color = new Color(0, 0, 0, 0);
        Point = null;
        MergeCount = 0;
    }

    public void PointerDown()
    {
        if (!CellIsOpen)
        {
            GAMEControler.CurrentCell = this;
            _text.text = $"Want Buy?\n" +
                $"({GAMEControler.OpenPrice * 5} paint)";
            GAMEControler.MenuOpen(_menu);
        }
    }
    public void OnPointerDownDelegate(PointerEventData data)
    {
        if (!CellIsOpen)
        {
            GAMEControler.CurrentCell = this;
            _text.text = $"Want Buy?\n" +
                $"({GAMEControler.OpenPrice * 5} paint)";
            GAMEControler.MenuOpen(_menu);
        }
    }
}
