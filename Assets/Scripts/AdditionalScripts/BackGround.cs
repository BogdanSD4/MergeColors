using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour
{
    [SerializeField] private List<Color> _colors;
    [SerializeField] private Image _image;
    private Color _target;
    private float _timer;
    private void OnEnable()
    {
        _image.color = ChooseColor();
        _target = ChooseColor();
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        _image.color = Color.LerpUnclamped(_image.color, _target, Time.deltaTime / 2);
        if(_timer >= 5)
        {
            _timer = 0;
            _target = ChooseColor();
        } 
    }
    private Color ChooseColor()
    {
        int color = Random.Range(0, _colors.Count);
        return _colors[color];
    }
}
