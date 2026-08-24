using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] statsButtons;

    public static MenuManager instance;

    private PlayerStats[] playerStats;
    [SerializeField] TextMeshProUGUI[] nameText, hpText, spText, lvlText, xpText;
    [SerializeField] Slider[] hpSlider, spSlider, xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(menu.activeInHierarchy)
            {
                UpdateStats();
                menu.SetActive(false);
                GameManager.instance.gameMenuOpened = false;

            } 
            else 
            {
                menu.SetActive(true);
                GameManager.instance.gameMenuOpened = true;
            }
        }
       
    }

    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();

        for(int i = 0; i < playerStats.Length; i++)
        {
            characterPanel[i].SetActive(true);
            nameText[i].text = playerStats[i].playerName;
            hpText[i].text = playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            spText[i].text = playerStats[i].currentStamina + "/" + playerStats[i].maxStamina;
            lvlText[i].text = playerStats[i].playerLevel + "" ;

            characterImage[i].sprite = playerStats[i].characterImage;

            hpSlider[i].maxValue = playerStats[i].maxHP;
            hpSlider[i].value = playerStats[i].currentHP;

            spSlider[i].maxValue = playerStats[i].maxStamina;
            spSlider[i].value = playerStats[i].currentStamina;

            xpText[i].text = playerStats[i].currentXP.ToString() + "/" + playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpSlider[i].value = playerStats[i].currentXP;
        }
    }

    public void StatsMenu()
    {
        for(int i = 0; i < playerStats.Length; i++)
        {
            statsButtons[i].SetActive(true);

            statsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].playerName;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void FadeImage()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading");
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        GameManager.instance.gameMenuOpened = false;
    }
}
