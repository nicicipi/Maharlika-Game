using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup MasterMixer;
    public AudioMixerGroup SFXMixer;
    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup CharacterMixer;
    [SerializeField] AudioSource[] SFX, backgroundMusic, voiceLines;

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
    private void Start()
    {

        MasterMixer.audioMixer.SetFloat("Master", PercentToVolume(0.1f));
        SFXMixer.audioMixer.SetFloat("SFX", PercentToVolume(0.1f));
        MusicMixer.audioMixer.SetFloat("Music", PercentToVolume(0.1f));
        CharacterMixer.audioMixer.SetFloat("Characters", PercentToVolume(1f));
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
    private float PercentToVolume(float percent)
    {
        if (percent <= 0f)
            return -80f;

        return Mathf.Log10(percent) * 20f;
    }

    private float VolumeToPercent(float volumeDB)
    {
        if (volumeDB <= -80f)
            return 0f;

        return Mathf.Pow(10f, volumeDB / 20f);
    }
}
