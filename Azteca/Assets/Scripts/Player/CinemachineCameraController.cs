using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook _freeLookCamera;
    [SerializeField] CinemachineVirtualCamera _lockOnCamera;

    CinemachineBasicMultiChannelPerlin _freeNoise, _lockNoise, _currentNoise;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _lockNoise = _lockOnCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _freeNoise = _freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Final()
    {
        _freeLookCamera.gameObject.SetActive(false);
        _lockOnCamera.gameObject.SetActive(false);
    }

    public void FreeLook()
    {
        _freeLookCamera.enabled = true;
        _lockOnCamera.enabled = false;
        _currentNoise = _freeNoise;
    }

    public void LockOn(Transform target)
    {
        _freeLookCamera.enabled = false;

        _lockOnCamera.LookAt = target;
        _lockOnCamera.enabled = true;
        _currentNoise = _lockNoise;
    }

    public void CameraShake(float intensity, float duration)
    {
        StartCoroutine(Shaking(intensity, duration));
    }

    private IEnumerator Shaking(float intensity, float duration)
    {
        _currentNoise.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        _currentNoise.m_AmplitudeGain = 0;
    }
}
