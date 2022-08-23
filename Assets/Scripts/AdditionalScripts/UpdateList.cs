using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateList : MonoBehaviour
{
    [SerializeField] private UpdateMenu _menuType;
    [SerializeField] private UpdateMenuControl _updateMenu;
    [SerializeField] private GAMEControler _gameControler;
    [SerializeField] private int _menuLayer;
    [SerializeField] private List<RectTransform> _menu;

    private Image _image;
    private bool state;
    private bool open;
    private bool close;

    private bool _touch;
    Vector3 pos;

    private void Start()
    {
        _image = GetComponent<Image>();

        MenuSpecialProperties(_menuType);
    }
    private void Update()
    {
        if (open) OpenAdditionalMenu();
        if (close) CloseAdditionalMenu();
    }
    private void OpenClose()
    {
        if (!state)
        {
            open = true;
            state = true;
        }
        else
        {
            close = true;
            state = false;
        }
    }
    private void OpenAdditionalMenu()
    {
        int counter;
        for(int i = 0; i < _menu.Count; i++)
        {
            counter = 0;
            int minus = 1;
            while (counter <= i)
            {
                _menu[_menu.Count - minus].transform.localPosition -= new Vector3(0, UpdateMenuControl.MenuDistance);
                counter++;
                minus++;
            }
        }
        _updateMenu.ChangeContentSize(_menu.Count * UpdateMenuControl.MenuDistance, _menuLayer);
        open = false;
    }
    private void CloseAdditionalMenu()
    {
        int counter;
        for (int i = 0; i < _menu.Count; i++)
        {
            counter = 0;
            int minus = 1;
            while (counter <= i)
            {
                _menu[_menu.Count - minus].transform.localPosition += new Vector3(0, UpdateMenuControl.MenuDistance);
                counter++;
                minus++;
            }
        }
        _updateMenu.ChangeContentSize(-(_menu.Count * UpdateMenuControl.MenuDistance), _menuLayer);
        close = false;
    }



    public void OpenLabMenu(int menuNumber)
    {
        if (!_updateMenu.allUpdate.labMenu.GetLabMenuActive(menuNumber))
        {
            double price = _updateMenu.allUpdate.labMenu.labPrice[menuNumber - 1].Price;
            int num = 0; ;
            if (price <= _gameControler.CoinCall)
            {
                _gameControler.CoinCall = -(float)price;
                (num, price) = _updateMenu.allUpdate.labMenu.LabUpdate(menuNumber, 2);
                SetPrice(menuNumber, price);
            }
            else GAMEControler.ConsoleEnter("Not enought money");
        }
        else GAMEControler.ConsoleEnter("Lab is working");
    }
    public void OpenNewLabMenuCell(int menuNumber)
    {
        if (!_updateMenu.allUpdate.labMenu.GetLabMenuActive(menuNumber))
        {
            double price = _updateMenu.allUpdate.labMenu.labPrice[menuNumber - 1].Price;
            int num = 0;
            if (price <= _gameControler.CoinCall)
            {
                _gameControler.CoinCall = -(float)price;
                (num, price)  = _updateMenu.allUpdate.labMenu.LabUpdate(menuNumber, 1);
                SetPrice(menuNumber, price);
            }
            else GAMEControler.ConsoleEnter("Not enought money");

            if (num == 4)
            {
                _menu[menuNumber - 1].GetComponent<EventDelay>().EventStart(1);
            }
        }
        else GAMEControler.ConsoleEnter("Lab is working");
    }


    public void StorageSetCoinPrice(TextMeshProUGUI text)
    {
        text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.storage.UpdatePriceCoin, 1)}";
    }
    public void StorageSetPaintPrice(TextMeshProUGUI text)
    {
        text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.storage.UpdatePricePaint, 1)}";
    }
    public void StorageLevelUpCoin(TextMeshProUGUI text)
    {
        double price = _updateMenu.allUpdate.storage.UpdatePriceCoin;
        if (price <= _gameControler.CoinCall)
        {
            _gameControler.CoinCall = -(float)price;
            text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.storage.UpCoin(), 1)}";
        }
        else GAMEControler.ConsoleEnter("Not enought money");
    }
    public void StorageLevelUpPaint(TextMeshProUGUI text)
    {
        double price = _updateMenu.allUpdate.storage.UpdatePricePaint;
        if (price <= _gameControler.PaintCall)
        {
            _gameControler.PaintCall = -(float)price;
            text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.storage.UpPaint(), 1)}";
        }
        else GAMEControler.ConsoleEnter("Not enought money");
    }



    public void SellCoinSetPrice(TextMeshProUGUI text) => 
        text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.paintSeller.UpdatePrice, 1)}";
    public void SellCoinLevelUp(TextMeshProUGUI text)
    {
        double price = _updateMenu.allUpdate.paintSeller.UpdatePrice;
        if (price <= _gameControler.CoinCall)
        {
            _gameControler.CoinCall = -(float)price;
            text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.paintSeller.UpdateSeller(), 1)}";
        }
        else GAMEControler.ConsoleEnter("Not enought money");
    }


    public void ClickerSetPriceClick(TextMeshProUGUI text)
    {
        text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.clicker.PriceGetBuyClick, 1)}";
    }
    public void ClickerSetPriceFactor(TextMeshProUGUI text)
    {
        text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.clicker.PriceMaxFactor, 1)}";
    }
    public void ClickerSetPriceAmount(TextMeshProUGUI text)
    {
        text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.clicker.PriceTapAmount, 1)}";
    }
    public void ClickerUpdateClick(TextMeshProUGUI text)
    {
        double price = _updateMenu.allUpdate.clicker.PriceGetBuyClick;
        if (price <= _gameControler.CoinCall)
        {
            _gameControler.CoinCall = -(float)price;
            text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.clicker.UpClick(), 1)}";
        }
        else GAMEControler.ConsoleEnter("Not enought money");
    }
    public void ClickerUpdateFactor(TextMeshProUGUI text)
    {
        double price = _updateMenu.allUpdate.clicker.PriceMaxFactor;
        if (price <= _gameControler.CoinCall)
        {
            _gameControler.CoinCall = -(float)price;
            text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.clicker.UpFactor(), 1)}";
        }
        else GAMEControler.ConsoleEnter("Not enought money");
    }
    public void ClickerUpdateAmount(TextMeshProUGUI text)
    {
        double price = _updateMenu.allUpdate.clicker.PriceTapAmount;
        if (price <= _gameControler.CoinCall)
        {
            _gameControler.CoinCall = -(float)price;
            text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.clicker.UpAmount(), 1)}";
        }
        else GAMEControler.ConsoleEnter("Not enought money");
    }


    private void MenuSpecialProperties(UpdateMenu menu)
    {
        switch (menu)
        {
            case UpdateMenu.LabMenu:
                int[] value = _updateMenu.GetOpenCellsValue();
                for(int i = 0; i < _menu.Count; i++)
                {
                    EventDelay delay = _menu[i].GetComponent<EventDelay>();
                    
                    if (value[i] != 0)
                    {
                        delay.EventStart(0);
                        if (value[i] == 4) delay.EventStart(1);
                    }

                    TextMeshProUGUI text = (TextMeshProUGUI)delay.GetObject(typeof(TextMeshProUGUI), 0);
                    text.text = $"Price: {System.Math.Round(_updateMenu.allUpdate.labMenu.labPrice[i].StartPriceCount(), 1)}";
                }
                break;
            case UpdateMenu.ClassUpdate:
                break;
            case UpdateMenu.Storage:
                for (int i = 0; i < _menu.Count; i++)
                {
                    _menu[i].GetComponent<EventDelay>().EventStart(0);
                }
                break;
            case UpdateMenu.MergeUpdate:
                break;
            case UpdateMenu.SellCoin:
                for (int i = 0; i < _menu.Count; i++)
                {
                    _menu[i].GetComponent<EventDelay>().EventStart(0);
                }
                break;
            case UpdateMenu.Clicker:
                for (int i = 0; i < _menu.Count; i++)
                {
                    _menu[i].GetComponent<EventDelay>().EventStart(0);
                }
                break;
            default:
                break;
        }
    }
    private void SetPrice(int menuOrder, double num)
    {
        EventDelay delay = _menu[menuOrder - 1].GetComponent<EventDelay>();
        TextMeshProUGUI text = (TextMeshProUGUI)delay.GetObject(typeof(TextMeshProUGUI), 0);
        text.text = $"Price: {System.Math.Round(num, 1)}";
    }


    private void OnMouseUp()
    {
        if (_touch) OpenClose();
    }
    private void OnMouseDown()
    {
        _touch = true;
        pos = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        if(Input.mousePosition != pos) _touch = false;
    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    if (_touch) _touch = false;
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    _touch = true;
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (_touch) OpenClose();
    //}
}
public enum UpdateMenu
{
    LabMenu,
    ClassUpdate,
    Storage,
    MergeUpdate,
    SellCoin,
    Clicker,
}

