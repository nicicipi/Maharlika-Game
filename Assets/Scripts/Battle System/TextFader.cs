
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFader : MonoBehaviour
{
    public static TextFader Singleton;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textNotice;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float fadeOutDuration = 3f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeInDuration = 3f;

    private Queue<string> notificationQueue = new();
    private Coroutine currentNoticeRoutine;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(this);
            return;
        }

        Singleton = this;


        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
    }

    public void ShowText(string text)
    {
        notificationQueue.Enqueue(text);

        if (currentNoticeRoutine == null)
        {
            gameObject.SetActive(true);
            currentNoticeRoutine = StartCoroutine(ProcessNotificationQueue());
        }
    }

    private IEnumerator ProcessNotificationQueue()
    {
        while (notificationQueue.Count > 0)
        {
            string notification = notificationQueue.Dequeue();

            textNotice.text = notification;

            yield return StartCoroutine(SlideAndFadeRoutine());
        }

        currentNoticeRoutine = null;
        gameObject.SetActive(false);
    }

    private IEnumerator SlideAndFadeRoutine()
    {
        canvasGroup.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);

            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public IEnumerator WaitUntilFinished()
    {
        while (currentNoticeRoutine != null || notificationQueue.Count > 0)
        {
            yield return null;
        }
    }
}
