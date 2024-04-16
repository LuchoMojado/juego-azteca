using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] Slider _hpBar, _staminaBar;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateHpBar(float newValue)
    {
        _hpBar.value = newValue;
    }

    public void UpdateHpBar(float newValue, float maxValue)
    {
        _hpBar.value = newValue;
        _hpBar.maxValue = maxValue;
    }

    public void UpdateStaminaBar(float newValue)
    {
        _staminaBar.value = newValue;
    }

    public void UpdateStaminaBar(float newValue, float maxValue)
    {
        _staminaBar.value = newValue;
        _staminaBar.maxValue = maxValue;
    }
}
