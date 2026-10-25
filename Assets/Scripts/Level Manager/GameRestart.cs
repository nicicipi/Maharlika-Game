using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRestart : MonoBehaviour
{
    public void RestartGame()
    {
        GameMenuManager.instance.EndDemo();
    }
}
