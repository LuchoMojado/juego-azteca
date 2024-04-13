using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody _rb;
    Movement _movement;
    Inputs _inputs;

    [Header("Stats")]
    [SerializeField] float _maxStamina, _staminaRegenRate, _speed, _jumpStr, _stepStr;

    [Header("Stamina costs")]
    [SerializeField] int _jumpCost, _stepCost;

    float _stamina;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _movement = new Movement(transform, _rb, _speed, _jumpStr, _stepStr);
        _inputs = new Inputs(_movement, this);

        _stamina = _maxStamina;
    }

    private void Start()
    {
        _inputs.inputUpdate = _inputs.Unpaused;
    }

    void Update()
    {
        if (_inputs.inputUpdate != null) _inputs.inputUpdate();

        if (_stamina < _maxStamina)
        {
            _stamina += Time.deltaTime * _staminaRegenRate;
        }
    }

    private void FixedUpdate()
    {
        _inputs.InputsFixedUpdate();
    }

    public void Jump()
    {
        if (_movement.IsGrounded() && _stamina >= _jumpCost)
        {
            _movement.Jump();
        }
    }

    public void Step(float horizontalInput, float verticalInput)
    {
        if (_movement.IsGrounded() && _stamina >= _stepCost)
        {
            _movement.Step(horizontalInput, verticalInput);
        }
    }
}
