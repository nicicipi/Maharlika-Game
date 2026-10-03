using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamController : MonoBehaviour
{
    private Player playerTarget;
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField] int musicToPlay;
    private bool musicAlreadyPlayed;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        SnapToPlayer();

        if (AudioManager.instance != null && !musicAlreadyPlayed)
        {
            musicAlreadyPlayed = true;
            AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        }
    }

    public void SnapToPlayer()
    {
        if (playerTarget == null)
        {
            playerTarget = FindObjectOfType<Player>();
        }

        if (playerTarget == null || virtualCamera == null)
            return;

        virtualCamera.Follow = playerTarget.transform;

        // Forces Cinemachine to ignore its previous scene position
        // and snap directly to the player.
        virtualCamera.PreviousStateIsValid = false;
    }

    private void Update()
    {
        if (playerTarget == null)
        {
            SnapToPlayer();
        }
    }
}
