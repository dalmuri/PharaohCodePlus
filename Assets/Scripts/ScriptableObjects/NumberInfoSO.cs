using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquareColor { Yellow, Blue, Red, Purple, Empty }

[System.Serializable]
public class NumberInfo
{
    public int number;
    public SquareColor squareColor;
    public int point;
}

[CreateAssetMenu(fileName = "NumberInfoSO", menuName = "Scriptable Object/NumberInfoSO")]
public class NumberInfoSO : ScriptableObject
{
    public NumberInfo[] numberInfos;
}