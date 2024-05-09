using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook _freeLookCamera;
    [SerializeField] CinemachineVirtualCamera _lockOnCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void FreeLook()
    {
        _freeLookCamera.enabled = true;
        _lockOnCamera.enabled = false;
    }

    public void LockOn(Transform target)
    {
        _freeLookCamera.enabled = false;

        _lockOnCamera.LookAt = target;
        _lockOnCamera.enabled = true;
    }
}
