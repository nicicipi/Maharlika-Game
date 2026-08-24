using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened, dialogueBoxOpened;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        //If want the players to be automatically put in the gamemanager
        //playerStats = FindObjectsOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameMenuOpened || dialogueBoxOpened)
        {
            Player.instance.deactivateMovement = true; 
        }
        else 
        {
            Player.instance.deactivateMovement = false; 
        }
        
    }

    public PlayerStats[] GetPlayerStats()
    {
        return playerStats;
    }


}

