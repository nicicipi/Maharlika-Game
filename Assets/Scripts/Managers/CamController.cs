using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    public bool snapToPlayer = false;
    [SerializeField] int musicToPlay;
    private bool musicAlreadyPlayed;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (AudioManager.instance != null && !musicAlreadyPlayed)
        {
            musicAlreadyPlayed = true;
            AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        }
    }
    private void Start()
    {
        if(snapToPlayer)
            SnapToPlayer();
    }

    public void SnapToPlayer()
    {
        StartCoroutine(SnapRoutine());
    }

    private IEnumerator SnapRoutine()
    {
        while (GameManager.Singleton.currentPlayer == null)
            yield return null;
        yield return new WaitForEndOfFrame();
        virtualCamera.Follow = GameManager.Singleton.currentPlayer.transform;
        virtualCamera.PreviousStateIsValid = false;
    }

}
