using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] Slider _hpBar, _staminaBar, _bossHpBar;
    [SerializeField] Button _joystick;
    [SerializeField] Image _paused;

    public GameObject textoFinal;

    public enum Bar
    {
        PlayerHp,
        PlayerStamina,
        BossHp
    }

    private void Awake()
    {
        instance = this;
    }

    public void UpdateBar(Bar bar, float newValue)
    {
        switch (bar)
        {
            case Bar.PlayerHp:
                _hpBar.value = newValue;
                break;
            case Bar.PlayerStamina:
                _staminaBar.value = newValue;
                break;
            case Bar.BossHp:
                _bossHpBar.value = newValue;
                break;
            default:
                break;
        }
    }
    public void UpdateBar(Bar bar, float newValue, float maxValue)
    {
        switch (bar)
        {
            case Bar.PlayerHp:
                _hpBar.maxValue = maxValue;
                _hpBar.value = newValue;
                break;
            case Bar.PlayerStamina:
                _staminaBar.maxValue = maxValue;
                _staminaBar.value = newValue;
                break;
            case Bar.BossHp:
                _bossHpBar.maxValue = maxValue;
                _bossHpBar.value = newValue;
                break;
            default:
                break;
        }
    }

    public void ToggleBossBar(bool turnOn)
    {
        _bossHpBar.gameObject.SetActive(turnOn);
    }

    public void Paused()
    {
        _paused.gameObject.SetActive(true);
        _joystick.gameObject.SetActive(true);
    }
    public void UnPaused()
    {
        _paused.gameObject.SetActive(false);
        _joystick.gameObject.SetActive(false);
    }

    public void Final()
    {
        _hpBar.gameObject.SetActive(false);
        _staminaBar.gameObject.SetActive(false);
        textoFinal.SetActive(true);
    }
}
