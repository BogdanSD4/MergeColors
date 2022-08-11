using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] private GAMEControler _gameControler;

    [SerializeField] private GameObject _absentMenu;

    [SerializeField] private TextMeshProUGUI _day;
    [SerializeField] private TextMeshProUGUI _hour;
    [SerializeField] private TextMeshProUGUI _minutes;
    [SerializeField] private TextMeshProUGUI _seconds;

    [SerializeField] private TextMeshProUGUI _coin;
    [SerializeField] private TextMeshProUGUI _paint;
    [SerializeField] private TextMeshProUGUI _coinLimit;
    [SerializeField] private TextMeshProUGUI _paintLimit;

    private float _Coin;
    private float _Paint;
    private float _CoinLimit = 10000;
    private float _PaintLimit = 10000;

    public List<MergeMenuSave> mergeTimers = new List<MergeMenuSave>();

    private void Update()
    {
        if(mergeTimers.Count != 0)
        {
            for (int i = 0; i < mergeTimers.Count; i++)
            {
                mergeTimers[i].MenuMerge.Timer -= Time.deltaTime;
                if (mergeTimers[i].MenuMerge.Timer <= 0) mergeTimers.Remove(mergeTimers[i]);
            }
        }
    }
    public static float TimeMinusPlayerAbsent { get; set; } 

    private void SetCorrectTime()
    {
        for(int i = 0; i < mergeTimers.Count; i++)
        {
            mergeTimers[i].MenuMerge.Timer -= TimeMinusPlayerAbsent;
        }
        TimeMinusPlayerAbsent = 0;
    }

    public void CheckOffline(string date)
    {
        TimeSpan ts;
 
        ts = DateTime.Now - DateTime.Parse(date);
        int time = ts.Seconds + (ts.Minutes * 60) + (ts.Hours * 3600) + (ts.Days * 86400);
        _day.text = ts.Days.ToString();
        _hour.text = ts.Hours.ToString();
        _minutes.text = ts.Minutes.ToString();
        _seconds.text = ts.Seconds.ToString();

        _coinLimit.text = _CoinLimit.ToString();
        _paintLimit.text = _PaintLimit.ToString();

        _Coin= (time * _gameControler.CoinPerSec);
        _Paint = (time * _gameControler.PaintPerSec);

        if (_Coin > _CoinLimit) _Coin = _CoinLimit;
        if (_Paint > _PaintLimit) _Paint = _PaintLimit;

        _coin.text = Mathf.Round(_Coin).ToString();
        _paint.text = Mathf.Round(_Paint).ToString();

        if (_Coin != 0 || _Paint != 0)
        {
            GAMEControler.MenuOpen(_absentMenu);
        }
        TimeMinusPlayerAbsent = time;
        SetCorrectTime();
    }
    public void TakeEarn()
    {
        _gameControler.CoinCall = _Coin;
        _gameControler.PaintCall = _Paint;
        _absentMenu.SetActive(false);
    }
}

