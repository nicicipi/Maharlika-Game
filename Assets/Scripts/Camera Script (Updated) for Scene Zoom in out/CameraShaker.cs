using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin _noise;
    private Coroutine _shakeRoutine;

    private void Awake()
    {
        _noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float duration, float power)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, power));
    }

    private IEnumerator ShakeRoutine(float duration, float power)
    {
        _noise.m_AmplitudeGain = power;

        yield return new WaitForSeconds(duration);

        _noise.m_AmplitudeGain = 0f;
        _shakeRoutine = null;
    }
}
