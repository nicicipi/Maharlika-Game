using SaveSystem;
using System.Collections;
using UnityEngine;

public class OpeningSequence : MonoBehaviour
{
    private const string OpeningSequencePlayedKey = "OpeningSequence";
    
    [Header("Screen")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float blackScreenDuration = 1.5f;

    [Header("Explosion")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;

    [Header("Opening Dialogue")]
    [SerializeField] private Dialogue protagonistDialogue;

    private Coroutine openingCoroutine;

    private void Start()
    {
        if (PlayerDataHandler.Singleton.IsTriggered(OpeningSequencePlayedKey))
        {

            blackScreen.gameObject.SetActive(false);
            blackScreen.alpha = 0f;
            blackScreen.blocksRaycasts = false;
            blackScreen.interactable = false;
            return;
        }
        PlayerDataHandler.Singleton.AddToTriggeredFlag(OpeningSequencePlayedKey);
        ResetDialogue();
        openingCoroutine = StartCoroutine(PlayOpeningSequence());
    }

    private IEnumerator PlayOpeningSequence()
    {
        if (blackScreen == null)
        {
            Debug.LogError(
                "OpeningSequence: Black screen CanvasGroup is not assigned.",
                this
            );

            yield break;
        }
        GameManager.Singleton.SetGameState(GameManager.GameState.Cutscene);

        blackScreen.alpha = 1f;
        blackScreen.blocksRaycasts = true;
        blackScreen.interactable = true;

        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        float waitTime = blackScreenDuration;

        yield return new WaitForSecondsRealtime(waitTime);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;


            blackScreen.alpha = Mathf.Lerp(fadeDuration, 0, elapsed);

            yield return null;
        }

        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;
        blackScreen.interactable = false;
        blackScreen.gameObject.SetActive(false);

        if (DialogueManager.Instance == null)
        {
            Debug.LogError(
                "OpeningSequence: DialogueManager is missing.",
                this
            );

            yield break;
        }

        if (protagonistDialogue == null)
        {
            Debug.LogError(
                "OpeningSequence: Protagonist dialogue is not assigned.",
                this
            );

            yield break;
        }

        PlayerPrefs.SetInt(OpeningSequencePlayedKey, 1);
        PlayerPrefs.Save();

        DialogueManager.Instance.StartDialogue(protagonistDialogue);
    }

    public void ResetDialogue()
    {
        if (openingCoroutine != null)
        {
            StopCoroutine(openingCoroutine);
            openingCoroutine = null;
        }

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 1f;
            blackScreen.blocksRaycasts = true;
            blackScreen.interactable = true;
        }
    }
}