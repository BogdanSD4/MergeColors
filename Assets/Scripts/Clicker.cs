using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clicker : MonoBehaviour
{
    [SerializeField] private GAMEControler _gameControler;
    [SerializeField] private TextMeshProUGUI _getByClickPrefab;
    [SerializeField] private Vector2 posX;
    [SerializeField] private Vector2 posY;
    [SerializeField] private Transform _spawn;
    [SerializeField] private List<EventDelay> events;
    private float getByClick = 1;
    private int clickCount = 0;

    private bool multiplier;

    public static int factor = 1;
    private float factorLevel = 3;
    [Tooltip("Start at x2")]
    public int[] needTaps;
    private int[] needTapsMin = { 20, 100, 200, 500, 600, 700, 800, 900, 1000 };

    private float timer;
    private Color currentColor;

    private bool delay;
    private void Awake()
    {
        if ((getByClick = PlayerPrefs.GetFloat("GetByClick")) == 0) getByClick = 1;
    }
    private void Update()
    {
        if (multiplier)
        {

            if (!delay)
            {
                delay = true;
                for (int i = 0; i < events.Count; i++) events[i].EventStart(0);
                timer -= 0.5f;
            }

            timer += Time.deltaTime;
            if (timer > 0) for (int i = 0; i < events.Count; i++) events[i].EventStart(0);

            if (timer >= 10)
            {
                clickCount = 0;
                timer = 0;
                multiplier = false;
                currentColor = Color.white;

                for (int i = 0; i < events.Count; i++) events[i].EventStart(2);
                factor = 1;
                _gameControler.SetParametersPerSecond(0, 0);
            }
        }
    }

    public void OnClick()
    {
        _gameControler.CoinCall = getByClick * factor;
        Multiplier(clickCount++);
        GAMEControler.ClickAmount(clickCount.ToString());
        ByClickShow();

        multiplier = true;
        timer = 0;
        delay = false;
    }
    private void Multiplier(int count)
    {
        for (int j = 2; j <= 10; j++)
        {
            if (count == needTaps[j - 2])
            {
                if (factorLevel >= j)
                {
                    factor = j;
                    switch (j)
                    {
                        case 2:
                            currentColor = Color.blue;
                            break;
                        case 3:
                            currentColor = Color.green;
                            break;
                        case 4:
                            currentColor = Color.yellow;
                            break;
                        case 5:
                            currentColor = Color.red;
                            break;
                        case 6:
                            currentColor = Color.red;
                            break;
                        case 7:
                            currentColor = Color.blue;
                            break;
                        case 8:
                            currentColor = Color.blue;
                            break;
                        case 9:
                            currentColor = Color.blue;
                            break;
                        case 10:
                            currentColor = Color.blue;
                            break;
                    }
                    for (int i = 0; i < events.Count; i++) events[i].EventStart(1);
                }
                break;
            }
        }
    }



    public void Fill(Image image) => image.fillAmount = 1 - (timer / 10);
    public void CurrentColor(Image image) => image.color = currentColor;
    public void CurrentText(TextMeshProUGUI text)
    {
        text.text = "x" + factor.ToString();
        _gameControler.SetParametersPerSecond(0, 0);
    }



    public void UpdateGetByClick() => getByClick *= 2;
    public void UpdateMaxFactor() => factorLevel++;
    public void MultiplierUpdate()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (needTaps[i] != needTapsMin[i])
            {
                needTaps[i] -= i + 2;
                if (needTaps[i] < needTapsMin[i])
                {
                    needTaps[i] = needTapsMin[i];
                }
                break;
            }
        }
    }


    public void SetStartPreferance(int levelClick, int levelFactor, int levelAmount)
    {
        for (int i = 1; i < levelClick; i++)
        {
            getByClick *= 2;
        }

        for (int i = 1; i < levelFactor; i++) factorLevel++;

        int j = 0;
        for (int i = 1; i < levelAmount; i++)
        {
            needTaps[j] -= j + 2;
            if (needTaps[j] <= needTapsMin[j])
            {
                needTaps[j] = needTapsMin[j];
                j++;
            }
        }
    }
    [ContextMenu("click")]
    public void ByClickShow()
    {
        float xcord = Random.Range(posX.x, posX.y);
        float ycord = Random.Range(posY.x, posY.y); ;
        TextMeshProUGUI text = Instantiate(_getByClickPrefab, new Vector3(xcord, ycord), Quaternion.identity, _spawn);
        text.text = (getByClick * factor).ToString();
    }
}
