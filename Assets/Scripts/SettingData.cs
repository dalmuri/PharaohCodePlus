using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingData : MonoBehaviour
{
    public static SettingData instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public int timeLimit = -1;
    public int numberOfGettingHandInTurn = 10;
}
