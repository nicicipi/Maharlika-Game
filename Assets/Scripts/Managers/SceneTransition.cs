using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 0.25f;
    [SerializeField] float cameraSettleDelay = 0.2f;

    private bool isTransitioning;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SetAlpha(0f);
        fadeImage.gameObject.SetActive(false);
    }

    public void TransitionToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        isTransitioning = true;

        if (Player.instance != null)
        {
            Player.instance.deactivateMovement = true;
        }

        // Fade completely to black before loading.
        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f));

        // Load the new scene while the screen is fully black.
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Allow the new scene's Start methods and AreaEnter to run.
        yield return null;
        yield return new WaitForEndOfFrame();

        CamController cameraController = FindObjectOfType<CamController>();

        if (cameraController != null)
        {
            cameraController.SnapToPlayer();
        }

        // Give Cinemachine time to apply the snapped position.
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(cameraSettleDelay);

        // Only reveal the scene after the camera is stable.
        yield return StartCoroutine(Fade(1f, 0f));

        fadeImage.gameObject.SetActive(false);

        if (Player.instance != null)
        {
            Player.instance.deactivateMovement = false;
        }

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        float elapsed = 0f;

        SetAlpha(startAlpha);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, progress));

            yield return null;
        }

        // Guarantee the exact final alpha.
        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    public IEnumerator FadeToBlack()
    {
        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f));
    }

    public IEnumerator FadeFromBlack()
    {
        yield return StartCoroutine(Fade(1f, 0f));
        fadeImage.gameObject.SetActive(false);
    }
}