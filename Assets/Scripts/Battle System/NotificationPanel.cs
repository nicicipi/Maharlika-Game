
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class NotificationPanel : MonoBehaviour
{
    public static NotificationPanel Singleton;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textNotice;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float slideInOffset = 300f;

    private Vector2 targetPosition;
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

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        targetPosition = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0f;
    }

    public void ShowNotification(string text)
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
        // 1. Setup starting state
        Vector2 startPosition = targetPosition - new Vector2(slideInOffset, 0);

        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 0f;

        // 2. Slide into position
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);

            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // 3. Display notification
        yield return new WaitForSeconds(displayDuration);

        // 4. Fade out
        elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);

            yield return null;
        }

        // 5. Cleanup
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
