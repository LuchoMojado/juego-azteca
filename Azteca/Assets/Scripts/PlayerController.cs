using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PlayerController : Entity
{
    Rigidbody _rb;
    Movement _movement;
    Inputs _inputs;

    [SerializeField] float _maxStamina, _staminaRegenRate, _staminaRegenDelay, _speed, _speedOnCast, _jumpStr, _stepStr, _stepStopVelocity;

    [Header("Stamina costs")]
    [SerializeField] float _jumpCost, _stepCost, _sunBaseCost, _sunHoldCost;

    [Header("Placeholder")]
    [SerializeField] SunMagicPlaceholder _sunMagic;
    [SerializeField] Transform _magicSpawnPos;
    [SerializeField] float _sunBaseDamage, _sunDamageGrowRate;

    float _stamina, _currentStaminaDelay = 0;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _movement = new Movement(transform, _rb, _speed, _speedOnCast, _jumpStr, _stepStr);
        _inputs = new Inputs(_movement, this);
    }

    protected override void Start()
    {
        base.Start();

        UIManager.instance.UpdateBar(UIManager.Bar.PlayerHp, _hp, _maxHp);

        _stamina = _maxStamina;
        UIManager.instance.UpdateBar(UIManager.Bar.PlayerStamina, _stamina, _maxStamina);

        _inputs.inputUpdate = _inputs.Unpaused;
    }

    void Update()
    {
        if (_inputs.inputUpdate != null) _inputs.inputUpdate();

        StaminaRegeneration();
    }

    private void FixedUpdate()
    {
        _inputs.InputsFixedUpdate();
    }

    public void Jump()
    {
        if (_movement.IsGrounded() && CheckAndReduceStamina(_jumpCost))
        {
            _movement.Jump();
        }
    }

    public void Step(float horizontalInput, float verticalInput)
    {
        if (_movement.IsGrounded() && CheckAndReduceStamina(_stepCost))
        {
            StartCoroutine(Stepping(horizontalInput, verticalInput));
        }
    }

    public IEnumerator Stepping(float horizontalInput, float verticalInput)
    {
        _inputs.inputUpdate = _inputs.Stepping;

        _movement.Step(horizontalInput, verticalInput);

        yield return new WaitForSeconds(0.02f);

        while (_rb.velocity.magnitude > _stepStopVelocity)
        {
            yield return null;
        }

        _inputs.inputUpdate = _inputs.Unpaused;
    }

    public void ActivateSunMagic()
    {
        if (_movement.IsGrounded() && CheckAndReduceStamina(_sunBaseCost))
        {
            StartCoroutine(SunMagic());
        }
    }

    public IEnumerator SunMagic()
    {
        _sunMagic.gameObject.SetActive(true);
        _sunMagic.damage = _sunBaseDamage;
        _movement.Casting();

        while (_inputs.attack && CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
        {
            var lookAt = Camera.main.transform.forward;
            lookAt.MakeHorizontal();
            transform.forward = lookAt;

            _sunMagic.damage += _sunDamageGrowRate * Time.deltaTime;

            yield return null;
        }

        _inputs.inputUpdate = _inputs.Unpaused;
        _inputs.attack = false;
        _movement.StopCasting();
        _sunMagic.gameObject.SetActive(false);
    }

    void StaminaRegeneration()
    {
        if (_stamina < _maxStamina)
        {
            if (_currentStaminaDelay <= 0)
            {
                _stamina += Time.deltaTime * _staminaRegenRate;
                UIManager.instance.UpdateBar(UIManager.Bar.PlayerStamina, _stamina);
            }
            else
            {
                _currentStaminaDelay -= Time.deltaTime;
            }
        }
    }

    bool CheckAndReduceStamina(float cost)
    {
        if (_stamina >= cost)
        {
            _stamina -= cost;
            UIManager.instance.UpdateBar(UIManager.Bar.PlayerStamina, _stamina);
            _currentStaminaDelay = _staminaRegenDelay;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        UIManager.instance.UpdateBar(UIManager.Bar.PlayerHp, _hp);
    }

    public override void Die()
    {
        SceneManager.LoadScene(0);
    }
}
