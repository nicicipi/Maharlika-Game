using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;

public class PlayerStats : MonoBehaviour
{
    public string playerName;

    public Sprite characterImage;

    [SerializeField] int maxLevel = 20;
    public int playerLevel = 1;
    public int currentXP;
    public int[] xpForNextLevel;
    [SerializeField] int baseLevelXP = 100;

    public int maxHP = 100;
    public int currentHP;

    public int maxStamina = 100;
    public int currentStamina;

    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int special;
    [SerializeField] int specialDefence;
    [SerializeField] int speed;


    // Start is called before the first frame update
    void Start()
    {
        xpForNextLevel = new int[maxLevel];
        xpForNextLevel[1] = baseLevelXP;
        
        for(int i = 2; i < xpForNextLevel.Length; i++)
        {
            xpForNextLevel[i] = (int)(0.02f * i * i * i + 3.06f * i * i + 105.6f * i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddXP(100);
        }
    }

    public void AddXP(int amountOfXP)
    {
        currentXP += amountOfXP;
        if (currentXP > xpForNextLevel[playerLevel])
        {
            currentHP -= xpForNextLevel[playerLevel];
            playerLevel++;

            if (playerLevel % 2 == 0)
            {
                attack++;
                special++;
                speed++;
            }
            else
            {
                defence++;
                specialDefence++;
            }

            maxHP = Mathf.FloorToInt(maxHP * 1.18f);
            currentHP = maxHP;

            maxStamina = Mathf.FloorToInt(maxStamina * 1.06f);
            currentStamina = maxStamina;
        }
    }
}
