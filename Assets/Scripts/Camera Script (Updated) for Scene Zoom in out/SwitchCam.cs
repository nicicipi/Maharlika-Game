using Cinemachine;
using UnityEngine;

public class SwitchCam : MonoBehaviour
{
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CameraManager.SwitchCamera(cam1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CameraManager.SwitchCamera(cam2);
        }
    }
}
