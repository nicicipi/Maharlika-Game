using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXHandler : MonoBehaviour
{
    public void PlaySFX(int number)
    {
        AudioManager.instance.PlaySFX(number);
    }
    public void PlayMusic(int number)
    {
        AudioManager.instance.PlayBackgroundMusic(number);
    }
}
