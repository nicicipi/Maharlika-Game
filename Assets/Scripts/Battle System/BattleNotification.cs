using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BattleNotification : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textNotice;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.5f;  // Time to slide into place
    [SerializeField] private float displayDuration = 1.3f;  // Time it stays on screen
    [SerializeField] private float fadeDuration = 0.35f;   // Time to fade out
    [SerializeField] private float slideInOffset = 300f;   // Starting distance to the left

    private Vector2 targetPosition;
    private Coroutine currentNoticeRoutine;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // Cache the resting anchored position
        targetPosition = rectTransform.anchoredPosition;
    }

    public void SetText(string text)
    {
        textNotice.text = text;
    }

    public void Activate()
    {
        // Stop any currently running animation so rapid calls reset cleanly
        if (currentNoticeRoutine != null)
            StopCoroutine(currentNoticeRoutine);

        gameObject.SetActive(true);
        currentNoticeRoutine = StartCoroutine(SlideAndFadeRoutine());
    }

    private IEnumerator SlideAndFadeRoutine()
    {
        // 1. Setup starting state (off-screen left and fully visible)
        Vector2 startPosition = targetPosition - new Vector2(slideInOffset, 0);
        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 1f;

        // 2. Slide from left to right into final position
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            // Smooth ease-out curve
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // 3. Pause so the player can read the notice
        yield return new WaitForSeconds(displayDuration);

        // 4. Fade out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        // 5. Cleanup
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        currentNoticeRoutine = null;
    }
}