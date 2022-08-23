using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopStand : MonoBehaviour
{
    [SerializeField] private Image _product;
    [SerializeField] private Button _button;
    [SerializeField] private Animator _anim;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _paintPerSecText;
    [SerializeField] private TextMeshProUGUI _criticalDropText;
    [SerializeField] private GameObject _garbage;
    [Space]
    public Fabric Fabric;

    private GAMEControler _gameControler;
    private void Start()
    {
        if (Fabric.BaseColor) _garbage.SetActive(false);
        _gameControler = GameObject.FindWithTag("MainCamera").GetComponent<GAMEControler>();
        _button.onClick.AddListener(ColorDell);
        SetParameters();
    }
    public void Buy()
    {
        _gameControler.Buy(Fabric, Fabric.PointParameters.Price);
    }
    private void SetParameters()
    {
        _product.color = Fabric.Color;
        _priceText.text = $"Coin: {System.Math.Round(Fabric.PointParameters.Price, 2)}";
        _paintPerSecText.text = $"Paint/sec: {System.Math.Round(Fabric.PointParameters.PaintPerSec, 2)}";
        _criticalDropText.text = $"Critical Drop: {System.Math.Round(Fabric.PointParameters.CriticalDrop, 3)}%";
    }
    public void ColorDell()
    {
        _gameControler.ColorDell(this);
        _anim.SetTrigger("Open");
        _button.enabled = false;
    }
    public void ColorReturn()
    {
        _gameControler.ColorReturn(this);
        _anim.SetTrigger("Close");
        _button.enabled = false;
    }
    public void SetButtonDell()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(ColorDell);
        _button.enabled = true;
    }
    public void SetButtonReturn()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(ColorReturn);
        _button.enabled = true;
    }

    public void ButtonSound() => GAMEControler.GetAudio().PlayOneShotSound(SoundList.BuyCoin);
    public (Fabric, int) Save { get; set; }
}
