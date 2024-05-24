using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{
    [SerializeField] GameObject[] Buttons;
    [SerializeField] GameObject settingWindowGO;

    public void SetButtonsActive(bool isActive)
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].SetActive(isActive);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SetSettingMenu(bool isActive)
    {
        settingWindowGO.SetActive(isActive);
    }
}
