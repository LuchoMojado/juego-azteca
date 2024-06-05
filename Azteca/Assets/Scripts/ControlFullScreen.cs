using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering.Universal;
//using UnityEngine.InputSystem;

public class ControlFullScreen : MonoBehaviour
{
    [SerializeField] private ControlFullScreen _fullScreenControl;
    [SerializeField] private Material _material;

    private int _vignetteIntensity = Shader.PropertyToID("_VignetteAmount");
    // Start is called before the first frame update
    void Start()
    {
        _fullScreenControl.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _material.SetFloat(_vignetteIntensity, 0.2f);
        }
        else
        {
            _material.SetFloat(_vignetteIntensity, 1f);
        }
    }
}
