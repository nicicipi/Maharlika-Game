using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class DisplaySettings
{
    private const int DisplayWidth = 640;
    private const int DisplayHeight = 480;
    private const int UiReferenceWidth = 1920;
    private const int UiReferenceHeight = 1080;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ConfigureDisplay();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConfigureDisplay();
    }

    private static void ConfigureDisplay()
    {
#if UNITY_ANDROID || UNITY_IOS
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
#else
        Screen.SetResolution(DisplayWidth, DisplayHeight, FullScreenMode.Windowed);
#endif

        Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            if (!canvas.gameObject.scene.IsValid())
                continue;

            if (canvas.renderMode == RenderMode.WorldSpace)
                continue;

            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

            if (canvasScaler == null)
                canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(UiReferenceWidth, UiReferenceHeight);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }
}
