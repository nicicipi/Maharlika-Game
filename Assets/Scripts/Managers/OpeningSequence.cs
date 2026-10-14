using System.Collections;
using UnityEngine;

public class OpeningSequence : MonoBehaviour
{
    private const string OpeningSequencePlayedKey = "OpeningSequencePlayed";
    
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
        if (PlayerPrefs.GetInt(OpeningSequencePlayedKey, 0) == 1)
        {
            if (blackScreen != null)
            {
                blackScreen.gameObject.SetActive(false);
            }

            return;
        }

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

        blackScreen.alpha = 1f;
        blackScreen.blocksRaycasts = true;
        blackScreen.interactable = true;

        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        float waitTime = blackScreenDuration;

        if (explosionSound != null)
        {
            waitTime = Mathf.Max(waitTime, explosionSound.length);
        }

        yield return new WaitForSecondsRealtime(waitTime);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float fadeProgress = Mathf.Clamp01(
                elapsed / fadeDuration
            );

            blackScreen.alpha = 1f - fadeProgress;

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
        PlayerPrefs.DeleteKey(OpeningSequencePlayedKey);
        PlayerPrefs.Save();

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