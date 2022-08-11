using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public List<GameColors> _gameColors;
}
[System.Serializable]
public class GameColors
{

    public ColorClass ColorClass;
    [Header("CreatingInLabTimeBase")]
    public float MinTime;
    public float MaxTime;
    public List<Parameters> Color;
    [System.Serializable]
    public class Parameters 
    {
        public float CreatingInLabTime;
        public MergeMenuSave point;
    }
}
