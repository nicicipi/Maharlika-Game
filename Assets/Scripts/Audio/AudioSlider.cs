
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private Slider slider;
    public string parameter;
    private void OnEnable()
    {
        slider.SetValueWithoutNotify(GetVolume());
    }

    public float GetVolume()
    {
        mixerGroup.audioMixer.GetFloat(parameter, out float volumeDB);
        return VolumeToPercent(volumeDB);
    }
    private void Update()
    {
        SetVolume(slider.value);
    }
    public void SetVolume(float percent)
    {
        mixerGroup.audioMixer.SetFloat(parameter, PercentToVolume(percent * 100));
    }

    private float PercentToVolume(float percent)
    {
        if (percent <= 0f)
            return -80f;

        return Mathf.Log10(percent / 100f) * 20f;
    }

    private float VolumeToPercent(float volumeDB)
    {
        return Mathf.Pow(10f, volumeDB / 20f) * 100f;
    }
}
