using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCharacter : MonoBehaviour
{
    public Character characterToAdd;
    public void AddNewCharacter()
    {
        PlayerDataHandler.Singleton.AddCharacter(characterToAdd);
    }
}
