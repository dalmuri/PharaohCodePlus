using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] TMP_Dropdown timerSettingDropDown;

    List<string> timerSettingList = new List<string>();
    int currentTimerSetting;

    const string TIMER_SETTING_KEY = "TIMER_SETTING_KEY";

    public void SetUpTimerDropDown()
    {
        timerSettingDropDown.ClearOptions();

        timerSettingList.Add("infinite");
        timerSettingList.Add("30 seconds");
        timerSettingList.Add("60 seconds");
        timerSettingList.Add("90 seconds");
        timerSettingList.Add("120 seconds");

        timerSettingDropDown.AddOptions(timerSettingList);

        timerSettingDropDown.value = currentTimerSetting;
        timerSettingDropDown.onValueChanged.AddListener(delegate { SetTimerDropDown(timerSettingDropDown.value); });
    }

    void SetTimerDropDown(int option)
    {
        PlayerPrefs.SetInt(TIMER_SETTING_KEY, option);
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey(TIMER_SETTING_KEY)) currentTimerSetting = PlayerPrefs.GetInt(TIMER_SETTING_KEY);
        else currentTimerSetting = 0;
    }

    private void Start()
    {
        SetUpTimerDropDown();
    }
}
