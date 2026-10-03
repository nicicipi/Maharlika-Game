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

    private AudioSource pausedExplorationMusic;
    private AudioSource activeBattleMusic;

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

    public void StartBattleMusic(int musicToPlay)
    {
        if (musicToPlay < 0 || musicToPlay >= backgroundMusic.Length)
            return;

        // Prevent restarting the battle music if this is called twice.
        if (activeBattleMusic == backgroundMusic[musicToPlay] &&
            activeBattleMusic.isPlaying)
        {
            return;
        }

        // Pause the currently playing exploration music.
        pausedExplorationMusic = null;

        foreach (AudioSource song in backgroundMusic)
        {
            if (song.isPlaying)
            {
                song.Pause();
                pausedExplorationMusic = song;
                break;
            }
        }

        activeBattleMusic = backgroundMusic[musicToPlay];
        activeBattleMusic.loop = true;
        activeBattleMusic.Play();
    }

    public void StopBattleMusic()
    {
        if (activeBattleMusic != null)
        {
            activeBattleMusic.Stop();
            activeBattleMusic = null;
        }

        // Resume exploration music from its previous position.
        if (pausedExplorationMusic != null)
        {
            pausedExplorationMusic.UnPause();
            pausedExplorationMusic = null;
        }
    }
}
