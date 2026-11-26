using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private Slider slider;
    [SerializeField] private string parameter;

    private void OnEnable()
    {
        if (mixerGroup.audioMixer.GetFloat(parameter, out float volumeDB))
        {
            slider.SetValueWithoutNotify(VolumeToPercent(volumeDB));
        }
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(SetVolume);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(SetVolume);
    }

    public void SetVolume(float percent)
    {
        mixerGroup.audioMixer.SetFloat(parameter, PercentToVolume(percent));
    }

    private float PercentToVolume(float percent)
    {
        if (percent <= 0f)
            return -80f;

        return Mathf.Log10(percent) * 20f;
    }

    private float VolumeToPercent(float volumeDB)
    {
        if (volumeDB <= -80f)
            return 0f;

        return Mathf.Pow(10f, volumeDB / 20f);
    }
}