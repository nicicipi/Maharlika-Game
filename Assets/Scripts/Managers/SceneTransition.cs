using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 0.25f;
    [SerializeField] float cameraSettleDelay = 0.2f;


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

    public void TransitionToScene(SceneAsset sceneAsset, int position)
    {
        StartCoroutine(TransitionCoroutine(sceneAsset, position));
    }

    private IEnumerator TransitionCoroutine(SceneAsset sceneAsset, int position)
    {
        GameManager.Singleton.SetPlayState(GameManager.PlayState.Loading);
        fadeImage.gameObject.SetActive(true);

        yield return Fade(0f, 1f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneAsset.name);

        while (!loadOperation.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(cameraSettleDelay);
        yield return SetupScene(position);

        yield return Fade(1f, 0f);

        GameManager.Singleton.SetPlayState(GameManager.PlayState.Gameplay);
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator SetupScene(int spawnPosIndex)
    {
        yield return null;
     
        SceneBootupHandler handler = FindAnyObjectByType<SceneBootupHandler>();
        handler.SpawnPlayerOnPoint(GameManager.Singleton.playerPrefab.gameObject, spawnPosIndex);
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

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    public IEnumerator FadeToBlack( UnityAction OnEnd = null)
    {
        fadeImage.gameObject.SetActive(true);
        yield return Fade(0f, 1f);
        OnEnd?.Invoke();
    }

    public IEnumerator FadeFromBlack()
    {
        if (fadeImage == null)
        {
            Debug.LogError("SceneTransition: fadeImage is not assigned.");
            yield break;
        }

        fadeImage.gameObject.SetActive(true);

        yield return Fade(1f, 0f);

        fadeImage.gameObject.SetActive(false);
    }
}