using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCharacter : MonoBehaviour
{
    public Character characterToAdd;
    public void AddNewCharacter()
    {
        NotificationPanel.Singleton.ShowNotification($"{characterToAdd.characterName} joined the team!");
        AudioManager.instance.PlaySFX(36);
        PlayerDataHandler.Singleton.AddCharacter(characterToAdd);
    }
}
