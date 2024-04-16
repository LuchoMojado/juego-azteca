using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Entity
{
    Rigidbody _rb;
    Movement _movement;
    Inputs _inputs;

    [SerializeField] float _maxStamina, _staminaRegenRate, _staminaRegenDelay, _speed, _jumpStr, _stepStr;

    [Header("Stamina costs")]
    [SerializeField] float _jumpCost, _stepCost;

    float _stamina, _currentStaminaDelay = 0;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _movement = new Movement(transform, _rb, _speed, _jumpStr, _stepStr);
        _inputs = new Inputs(_movement, this);
    }

    protected override void Start()
    {
        base.Start();

        UIManager.instance.UpdateHpBar(_hp, _maxHp);

        _stamina = _maxStamina;
        UIManager.instance.UpdateStaminaBar(_stamina, _maxStamina);

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

        while (_rb.velocity.magnitude > 0)
        {
            Debug.Log("a");
            yield return null;
        }

        _inputs.inputUpdate = _inputs.Unpaused;
    }

    void StaminaRegeneration()
    {
        if (_stamina < _maxStamina)
        {
            if (_currentStaminaDelay <= 0)
            {
                _stamina += Time.deltaTime * _staminaRegenRate;
                UIManager.instance.UpdateStaminaBar(_stamina);
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
            UIManager.instance.UpdateStaminaBar(_stamina);
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

        UIManager.instance.UpdateHpBar(_hp);
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
