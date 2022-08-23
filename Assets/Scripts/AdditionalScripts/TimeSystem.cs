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

    private int _CoinLimitStep = 100;
    private int _PaintLimitStep = 10000;
    [SerializeField] private int _CoinLimit = 100;
    [SerializeField] private int _PaintLimit = 10000;
    private int _coinLevel = 1;
    private int _paintLevel = 1;

    public List<MergeMenuSave> mergeTimers = new List<MergeMenuSave>();

    private void Awake()
    {
        if((_coinLevel = PlayerPrefs.GetInt("CoinLevel")) == 0) _coinLevel = 1;
        if((_paintLevel = PlayerPrefs.GetInt("PaintLevel")) == 0) _paintLevel = 1;
        StartLimitCount();
    }
    private void Update()
    {
        if (mergeTimers.Count != 0)
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
        for (int i = 0; i < mergeTimers.Count; i++)
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

        _Coin = (time * _gameControler.CoinPerSec);
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


    public void LevelUpCoin()
    {
        _CoinLimit += _CoinLimitStep;
        if((_coinLevel++) %10 == 0)
        {
            _CoinLimitStep = _CoinLimit;
        }
        PlayerPrefs.SetInt("CoinLevel", _coinLevel);
    }
    public void LevelUpPaint()
    {
        _PaintLimit += _PaintLimitStep;
        if ((_paintLevel++) % 10 == 0)
        {
            _PaintLimitStep = _PaintLimit;
        }
        print(_paintLevel);
        PlayerPrefs.SetInt("PaintLevel", _paintLevel);
    }
    public int CoinLevel { get { return _coinLevel; } }
    public int PaintLevel { get { return _paintLevel; } }


    private void StartLimitCount()
    {
        for(int i = 1; i < _coinLevel; i++)
        {
            _CoinLimit += _CoinLimitStep;
            if (i % 10 == 0) _CoinLimitStep = _CoinLimit;
        }

        for (int i = 1; i < _paintLevel; i++)
        {
            _PaintLimit += _PaintLimitStep;
            if (i % 10 == 0) _PaintLimitStep = _PaintLimit;
        }
    }
}

