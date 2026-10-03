using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource[] SFX, backgroundMusic;

    public static AudioManager instance;

    private int currentMusicIndex = -1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(int soundToPlay)
    {
        if (soundToPlay >= 0 && soundToPlay < SFX.Length)
        {
            SFX[soundToPlay].Play();
        }
    }

    public void PlayBackgroundMusic(int musicToPlay)
    {
        if (musicToPlay < 0 || musicToPlay >= backgroundMusic.Length)
            return;

        AudioSource requestedMusic = backgroundMusic[musicToPlay];

        // Do not restart the same song when changing scenes
        if (currentMusicIndex == musicToPlay && requestedMusic.isPlaying)
            return;

        StopMusic();

        requestedMusic.loop = true;
        requestedMusic.Play();
        currentMusicIndex = musicToPlay;
    }

    public void StopMusic()
    {
        foreach (AudioSource song in backgroundMusic)
        {
            song.Stop();
        }

        currentMusicIndex = -1;
    }
}
