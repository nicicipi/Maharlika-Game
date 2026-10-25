using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;


    public void SwitchToCamera(CinemachineVirtualCamera camera)
    {
        foreach (var item in cameras)
            item.Priority = 0;
        camera.Priority = 10;
    }
    public void ConfineToMap(Collider2D bounds)
    {
        foreach (var item in cameras)
            SetCameraBounds(item, bounds);
    }
    public void SetCameraBounds(CinemachineVirtualCamera cam, Collider2D bounds)
    {
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        if (confiner == null) confiner = cam.gameObject.AddComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = bounds;
        confiner.InvalidateCache();
    }
}
