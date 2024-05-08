using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Entity
{
    Rigidbody _rb;
    Movement _movement;
    Inputs _inputs;

    [SerializeField] float _maxStamina, _staminaRegenRate, _staminaRegenDelay, _speed, _explorationSpeed, _speedOnCast, _turnRate, _jumpStr, _stepStr, _stepCooldown/*(variable del step viejo)_stepStopVelocity*/;
    [SerializeField] LayerMask _groundLayer;

    [Header("Stamina costs")]
    [SerializeField] float _jumpCost;
    [SerializeField] float _stepCost, _sunBaseCost, _sunHoldCost, _obsidianCost;

    [Header("Sun Magic")]
    [SerializeField] GameObject _sunMagic;
    [SerializeField] float _sunBaseDamage, _sunDamageGrowRate, _sunCastDelay, _sunRecovery, _sunCooldown, _sunHitboxX, _sunHitboxY, _sunHitboxZ, _sunRange;
    Vector3 _sunHitbox;

    [Header("Obsidian Magic")]
    [SerializeField] PlayerProjectile _obsidianShard;
    [SerializeField] float _obsidianDamage, _obsidianComboInterval, _obsidianCooldown, _shardAngleOffset, _shardSpeed;
    [SerializeField] int _shardsPerWave, _maxWaves;

    float _stepCurrentCooldown = 0, _obsidianCurrentCooldown = 0, _sunCurrentCooldown = 0;

    float _stamina, _currentStaminaDelay = 0;

    public Entity currentBoss;

    MagicType _activeMagic;

    public Renderer renderer;
    private bool _joystickActive=true;

    public enum MagicType
    {
        Sun,
        Obsidian
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _movement = new Movement(transform, _rb, _speed, _explorationSpeed, _speedOnCast, _turnRate, _jumpStr, _stepStr, _groundLayer);
        _inputs = new Inputs(_movement, this);

        _activeMagic = MagicType.Sun;
        renderer.material.color = Color.red;
        _sunHitbox = new Vector3(_sunHitboxX, _sunHitboxY, _sunHitboxZ);
    }

    protected override void Start()
    {
        base.Start();

        UIManager.instance.UpdateBar(UIManager.Bar.PlayerHp, _hp, _maxHp);

        _stamina = _maxStamina;
        UIManager.instance.UpdateBar(UIManager.Bar.PlayerStamina, _stamina, _maxStamina);

        _inputs.inputUpdate = _inputs.Unpaused;
        Joistick();
    }

    void Update()
    {
        if (_inputs.inputUpdate != null) _inputs.inputUpdate();

        ManageCooldowns();

        StaminaRegeneration();
    }

    private void FixedUpdate()
    {
        _inputs.InputsFixedUpdate();
    }

    private void LateUpdate()
    {
        _inputs.inputLateUpdate();
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
        if (_movement.IsGrounded() && _stepCurrentCooldown <= 0 && CheckAndReduceStamina(_stepCost))
        {
            _stepCurrentCooldown = _stepCooldown;
            _movement.Step(horizontalInput, verticalInput);
        }
    }

    /* Step viejo (desactiva inputs por su duracion)
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
    }*/

    public void ChangeActiveMagic(MagicType type)
    {
        _activeMagic = type;
    }

    public void ActivateMagic()
    {
        switch (_activeMagic)
        {
            case MagicType.Sun:
                ActivateSunMagic();
                break;
            case MagicType.Obsidian:
                ActivateObsidianMagic();
                break;
        }
    }

    void ActivateSunMagic()
    {
        if (_movement.IsGrounded() && _sunCurrentCooldown <= 0 && CheckAndReduceStamina(_sunBaseCost))
        {
            StartCoroutine(SunMagic());
        }
        else
        {
            _inputs.Attack = false;
        }
    }

    IEnumerator SunMagic()
    {
        _inputs.inputUpdate = _inputs.MovingCast;
        var damage = _sunBaseDamage;
        _movement.Casting();

        yield return new WaitForSeconds(_sunCastDelay);

        _sunMagic.gameObject.SetActive(true);

        while (_inputs.Attack && CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
        {
            var lookAt = Camera.main.transform.forward.MakeHorizontal();
            transform.forward = lookAt;

            if (Physics.BoxCast(transform.position, _sunHitbox, transform.forward, out var hit, transform.rotation, _sunRange))
            {
                if (hit.collider.TryGetComponent(out Entity entity))
                {
                    entity.TakeDamage(damage * Time.deltaTime);
                }
            }

            damage += _sunDamageGrowRate * Time.deltaTime;

            yield return null;
        }

        _sunMagic.gameObject.SetActive(false);

        yield return new WaitForSeconds(_sunRecovery);

        _sunCurrentCooldown = _sunCooldown;
        _inputs.Attack = false;
        _movement.StopCasting();
    }

    void ActivateObsidianMagic()
    {
        if (_movement.IsGrounded() && _obsidianCurrentCooldown <= 0 && CheckAndReduceStamina(_obsidianCost))
        {
            StartCoroutine(ObsidianMagic());
        }
        else
        {
            _inputs.Attack = false;
        }
    }

    IEnumerator ObsidianMagic()
    {
        _inputs.inputUpdate = _inputs.FixedCast;
        var lookAt = Camera.main.transform.forward.MakeHorizontal();
        transform.forward = lookAt;

        for (int i = _shardsPerWave; i < _shardsPerWave + _maxWaves; i++)
        {
            var baseRotationChange = i % 2 == 0 ? _shardAngleOffset * 0.5f : 0;

            for (int j = 0; j < i; j++)
            {
                var individualRotationChange = j % 2 == 0 ? _shardAngleOffset * Mathf.CeilToInt(j * 0.5f) : -_shardAngleOffset * Mathf.CeilToInt(j * 0.5f);

                var finalRotation = transform.rotation.AddYRotation(baseRotationChange + individualRotationChange);

                var shard = Instantiate(_obsidianShard, transform.position, finalRotation);
                shard.speed = _shardSpeed;
                shard.damage = _obsidianDamage;
            }

            float timer = 0;

            while (timer < _obsidianComboInterval)
            {
                lookAt = Camera.main.transform.forward.MakeHorizontal();
                transform.forward = lookAt;

                timer += Time.deltaTime;

                if (!_inputs.Attack)
                {
                    _obsidianCurrentCooldown = _obsidianCooldown;
                    yield break;
                }

                yield return null;
            }

            if (!CheckAndReduceStamina(_obsidianCost))
            {
                _obsidianCurrentCooldown = _obsidianCooldown;
                break;
            }
        }

        _obsidianCurrentCooldown = _obsidianCooldown;
    }

    void ManageCooldowns()
    {
        if (_stepCurrentCooldown > 0)
        {
            _stepCurrentCooldown -= Time.deltaTime;
        }
        if (_sunCurrentCooldown > 0)
        {
            _sunCurrentCooldown -= Time.deltaTime;
        }
        if (_obsidianCurrentCooldown > 0)
        {
            _obsidianCurrentCooldown -= Time.deltaTime;
        }
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

        _inputs.Attack = false;
        UIManager.instance.UpdateBar(UIManager.Bar.PlayerHp, _hp);
    }

    public override void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Joistick()
    {
        _joystickActive = !_joystickActive;
        _inputs.Altern(_joystickActive);
        _inputs.Attack = false;
    }
}
