using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource[] SFX, backgroundMusic;

    public static AudioManager instance;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayBackgroundMusic(2);
        }   
    }

    public void PlaySFX(int soundToPlay)
    {
        if(soundToPlay < SFX.Length)
        {
            SFX[soundToPlay].Play();
        }
    }

    public void PlayBackgroundMusic(int musicToPlay)
    {
        StopMusic();

        if (musicToPlay < backgroundMusic.Length)
        {
            backgroundMusic[musicToPlay].Play();
        }
    }

    private void StopMusic()
    {
        foreach(AudioSource song in backgroundMusic)
        {
            song.Stop();
        }
    }
}
