using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using SaveSystem;

public class MapRevealCameraPan : MonoBehaviour
{
    [Header("Reveal Persistence")]
    [SerializeField]
    private string revealId = "VillageMapReveal";

    private string RevealKey => "MapRevealShown_" + revealId;

    [Header("Cameras")]
    [SerializeField]
    private CinemachineVirtualCamera panCamera;

    [SerializeField]
    private CinemachineVirtualCamera playerCamera;

    [Header("Pan Positions")]
    [SerializeField]
    private Transform topPosition;

    [SerializeField]
    private Transform bottomPosition;

    [Header("Pan Settings")]
    [SerializeField]
    private float panDuration = 5f;

    [SerializeField]
    private AnimationCurve panCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private bool playOnStart = true;
    private void Start()
    {
        if (HasAlreadyPlayed())
        {
            SkipPan();
            return;
        }

        if (playOnStart)
        {
            PlayerDataHandler.Singleton.AddToTriggeredFlag(RevealKey);
            StartPan();
        }
    }

    private bool HasAlreadyPlayed()
    {
        return PlayerDataHandler.Singleton.IsTriggered(RevealKey);
    }

    public void StartPan()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Cutscene);
        StartCoroutine(PanAcrossMap());
    }

    private IEnumerator PanAcrossMap()
    {
        if (panCamera == null ||
            playerCamera == null ||
            topPosition == null ||
            bottomPosition == null)
        {
            Debug.LogError(
                "MapRevealCameraPan is missing a camera or position reference.",
                this
            );

            yield break;
        }

        panCamera.Follow = null;
        panCamera.LookAt = null;
        panCamera.Priority = 20;
        playerCamera.Priority = 10;

        Vector3 startPosition = topPosition.position;
        Vector3 endPosition = bottomPosition.position;

        startPosition.z = panCamera.transform.position.z;
        endPosition.z = panCamera.transform.position.z;

        panCamera.transform.position = startPosition;

        float elapsed = 0f;

        while (elapsed < panDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / panDuration);

            float curvedProgress =
                panCurve.Evaluate(progress);

            panCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    curvedProgress
                );

            yield return null;
        }

        panCamera.transform.position = endPosition;

        panCamera.Priority = 10;
        playerCamera.Priority = 20;

        panCamera.transform.position = endPosition;


        panCamera.Priority = 10;
        playerCamera.Priority = 20;

        EndPan();
    }

    private void SkipPan()
    {
        // Instantly set the camera to the end position and priorities as if the pan finished
        if (panCamera == null || playerCamera == null || bottomPosition == null)
            return;

        panCamera.Follow = null;
        panCamera.LookAt = null;
        panCamera.Priority = 10;
        playerCamera.Priority = 20;

        Vector3 endPosition = bottomPosition.position;
        endPosition.z = panCamera.transform.position.z;
        panCamera.transform.position = endPosition;
        EndPan();
    }
    public void EndPan()
    {
        GameManager.Singleton.SetGameState(GameManager.GameState.Roaming);
    }
         
}