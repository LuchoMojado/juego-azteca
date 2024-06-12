using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Entity
{
    Movement _movement;
    Inputs _inputs;

    public Inputs Inputs
    {
        get
        {
            return _inputs;
        }
    }

    [SerializeField] float _maxStamina, _staminaRegenRate, _staminaRegenDelay, _damageCooldown, _speed, _explorationSpeed, _speedOnCast, _turnRate, _jumpStr, _stepStr, _castStepStr, _stepCooldown/*(variable del step viejo)_stepStopVelocity*/;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _aimTarget;

    [Header("Stamina costs")]
    [SerializeField] float _jumpCost;
    [SerializeField] float _stepCost, _sunBaseCost, _sunHoldCost, _obsidianCost;

    [Header("Sun Magic")]
    [SerializeField] SunMagic _sunMagic;
    [SerializeField] float _sunBaseDamage, _sunDamageGrowRate, _sunSpeed, _sunMaxChargeTime, _sunCastDelay, _sunRecovery, _sunCooldown, _sunAbsorbTime, _sunMeleeDuration,_sunHitboxX, _sunHitboxY, _sunHitboxZ, _sunRange;
    Vector3 _sunHitbox;

    [Header("Obsidian Magic")]
    [SerializeField] PlayerProjectile _obsidianShard;
    [SerializeField] float _obsidianDamage, _obsidianCastDelay, _obsidianComboInterval, _obsidianCooldown, _shardAngleOffset, _shardSpeed;
    [SerializeField] int _shardsPerWave, _maxWaves;

    float _stepCurrentCooldown = 0, _obsidianCurrentCooldown = 0, _sunCurrentCooldown = 0, _damageCurrentCooldown = 0;

    float _stamina, _currentStaminaDelay = 0;

    public Boss currentBoss;

    MagicType _activeMagic;

    [SerializeField] CinemachineCameraController _cameraController;

    public GameObject camaraFinal;

    public Renderer renderer;
    private bool _joystickActive=true, _usingSun = false, _stopSun = false;

    [SerializeField] Material _VignetteAmountClamps;

    [SerializeField] ParticleSystem dustPlayer;
    [SerializeField] GameObject ForceFieldPlayer;

    SpecialsManager _specials;

    public bool StopSun
    {
        set
        {
            _stopSun = value;
        }
    }

    public bool UsingSun
    {
        get
        {
            return _usingSun;
        }
    }

    public Animator anim;

    AudioSource _myAS;
    [SerializeField] AudioClip sideStep,jump, damage, chargingSun;

    public enum MagicType
    {
        Sun,
        Obsidian
    }

    protected override void Awake()
    {
        base.Awake();
        _myAS = GetComponent<AudioSource>();
        _movement = new Movement(transform, _rb, _speed, _explorationSpeed, _speedOnCast, _turnRate, _jumpStr, _stepStr, _castStepStr, _groundLayer);
        _inputs = new Inputs(_movement, this, _cameraController);
        _specials = GetComponent<SpecialsManager>();

        _activeMagic = MagicType.Sun;
        renderer.material.color = Color.red;
        _sunHitbox = new Vector3(_sunHitboxX, _sunHitboxY, _sunHitboxZ);
    }

    void Start()
    {
        UIManager.instance.UpdateBar(UIManager.Bar.PlayerHp, _hp, _maxHp);

        _stamina = _maxStamina;
        UIManager.instance.UpdateBar(UIManager.Bar.PlayerStamina, _stamina, _maxStamina);

        _inputs.inputUpdate = _inputs.Unpaused;
        Joistick();
    }

    void Update()
    {
        if (transform.position.y < -87) Die();

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
        _inputs.InputsLateUpdate();
    }

    public void Jump()
    {
        if (_movement.IsGrounded() && _stepCurrentCooldown <= _stepCooldown * 0.5f && CheckAndReduceStamina(_jumpCost))
        {
            StartCoroutine(PrendoPlayerDust(true));
            anim.SetTrigger("IsJumping");
            _movement.Jump();
            ChangeAudio(jump);
            //anim.SetBool("IsJumping", false);
        }
    }

    public void Step(float horizontalInput, float verticalInput)
    {
        if (_movement.IsGrounded() && _stepCurrentCooldown <= 0 && CheckAndReduceStamina(_stepCost))
        {
            StartCoroutine(PrendoPlayerDust(true));
            //anim.SetBool("IsStrafeRight", true);
            ChangeAudio(sideStep);
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

    public void UseSpecial(int slot)
    {
        if (_movement.IsGrounded() && _specials.IsOffCooldown(slot) && CheckAndReduceStamina(_specials.GetCost(slot)))
        {
            _specials.ActivateSpecial(slot);
        }
    }

    public void ChangeActiveMagic(MagicType type)
    {
        _activeMagic = type;
    }

    public void ActivateMagic()
    {
        switch (_activeMagic)
        {
            case MagicType.Sun:
                //ActivateSunMagic();
                break;
            case MagicType.Obsidian:
                ActivateObsidianMagic();
                break;
        }
    }

    public void ActivateSecondaryMagic()
    {
        switch (_activeMagic)
        {
            case MagicType.Sun:
                ActivateAimedSunMagic();
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
            StartCoroutine(NewSunMagic());
        }
        else
        {
            _inputs.PrimaryAttack = false;
            //_inputs.SecondaryAttack = false;
        }
    }

    void ActivateAimedSunMagic()
    {
        if (!_usingSun && _movement.IsGrounded() && _sunCurrentCooldown <= 0 && CheckAndReduceStamina(_sunBaseCost))
        {
            StartCoroutine(AimedSunMagic());
        }
        else
        {
            //_inputs.PrimaryAttack = false;
            _inputs.SecondaryAttack = false;
        }
    }

    IEnumerator SunMagic()
    {
        //_inputs.inputUpdate = _inputs.MovingCast;
        var damage = _sunBaseDamage;
        _movement.Cast(true);

        yield return new WaitForSeconds(_sunCastDelay);

        _sunMagic.gameObject.SetActive(true);

        while (_inputs.PrimaryAttack && CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
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
        _inputs.PrimaryAttack = false;
        _movement.Cast(false);
    } // en desuso

    IEnumerator NewSunMagic()
    {
        _usingSun = true;
        //_inputs.inputUpdate = _inputs.MovingCast;

        anim.SetBool("IsAttacking", true);
        ControlFullScreen.instance.ChangeDemond(true);
        yield return new WaitForSeconds(_sunCastDelay);
        
        _movement.Cast(true);
        ChangeAudio(chargingSun);
        anim.SetBool("IsAttacking", false);

        var sun = Instantiate(_sunMagic, transform.position + transform.forward * 1f + transform.up * 0.6f, transform.rotation, transform);
        sun.player = this;
        sun.damage = _sunBaseDamage;

        float timer = 0;

        if (_inputs.PrimaryAttack)
        {
            while (!_stopSun && _inputs.PrimaryAttack && timer < _sunMaxChargeTime && CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
            {
                timer += Time.deltaTime;

                /*var lookAt = Camera.main.transform.forward.MakeHorizontal();
                transform.forward = lookAt;

                if (Physics.BoxCast(transform.position, _sunHitbox, transform.forward, out var hit, transform.rotation, _sunRange))
                {
                    if (hit.collider.TryGetComponent(out Entity entity))
                    {
                        entity.TakeDamage(damage * Time.deltaTime);
                    }
                }*/

                sun.damage += _sunDamageGrowRate * Time.deltaTime;

                yield return null;
            }

            if (timer >= _sunMaxChargeTime)
            {
                sun.ChargeFinished();
            }

            while (!_stopSun && _inputs.PrimaryAttack)
            {
                CheckAndReduceStamina(0);
                yield return null;
            }

            if (_stopSun)
            {
                sun.transform.SetParent(null);
                sun.StartCoroutine(sun.Death());
                ControlFullScreen.instance.ChangeDemond(false);
            }
            else
            {
                _inputs.inputUpdate = _inputs.FixedCast;

                var lookAt = Camera.main.transform.forward.MakeHorizontal();
                transform.forward = lookAt;

                anim.SetBool("IsAttacking", true);

                yield return new WaitForSeconds(_sunCastDelay);

                _inputs.inputUpdate = _inputs.Unpaused;
                sun.transform.SetParent(null);
                sun.speed = _sunSpeed;
                _rb.AddForce(-transform.forward * 75 * timer);
                sun.Shoot();
                ControlFullScreen.instance.ChangeDemond(false);
                yield return new WaitForSeconds(_sunRecovery);
            }
        }
        else
        {
            if (_stopSun)
            {
                sun.transform.SetParent(null);
                sun.StartCoroutine(sun.Death());
                ControlFullScreen.instance.ChangeDemond(false);
            }
            else
            {
                var lookAt = Camera.main.transform.forward.MakeHorizontal();
                transform.forward = lookAt;

                _inputs.inputUpdate = _inputs.Unpaused;
                sun.transform.SetParent(null);
                sun.speed = _sunSpeed;
                _rb.AddForce(-transform.forward * 75 * timer);
                sun.Shoot();
                ControlFullScreen.instance.ChangeDemond(false);
                yield return new WaitForSeconds(_sunRecovery);
            }
        }

        _stopSun = false;
        _usingSun = false;
        anim.SetBool("IsAttacking", false);
        _sunCurrentCooldown = _sunCooldown;
        _movement.Cast(false);
    } // en desuso

    IEnumerator NewerSunMagic()
    {
        _usingSun = true;
        //_inputs.inputUpdate = _inputs.MovingCast;

        anim.SetBool("IsAttacking", true);
        
        yield return new WaitForSeconds(_sunCastDelay);
        _movement.Cast(true);
        ChangeAudio(chargingSun);
        anim.SetBool("IsAttacking", false);
        _inputs.PrimaryAttack = false;
        _inputs.SecondaryAttack = false;

        var sun = Instantiate(_sunMagic, transform.position + transform.forward * 0.75f + transform.up * 0.6f, transform.rotation, transform);
        sun.player = this;
        sun.damage = _sunBaseDamage;

        float timer = 0;

        while (_damageCurrentCooldown <= 0 && !_inputs.PrimaryAttack && !_inputs.SecondaryAttack && timer < _sunMaxChargeTime && CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
        {
            timer += Time.deltaTime;

            sun.damage += _sunDamageGrowRate * Time.deltaTime;

            yield return null;
        }

        if (timer >= _sunMaxChargeTime || !CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
        {
            sun.ChargeFinished();
        }

        while (!_inputs.PrimaryAttack && !_inputs.SecondaryAttack)
        {
            if (_damageCurrentCooldown > 0)
            {
                sun.StartCoroutine(sun.Death());
                _usingSun = false;
                _sunCurrentCooldown = _sunCooldown;
                _movement.Cast(false);
                yield break;
            }

            CheckAndReduceStamina(0);
            yield return null;
        }

        var lookAt = Camera.main.transform.forward.MakeHorizontal();
        transform.forward = lookAt;

        _inputs.inputUpdate = _inputs.FixedCast;

        if (_inputs.PrimaryAttack)
        {
            anim.SetBool("IsAttacking", true);

            yield return new WaitForSeconds(_sunCastDelay);

            sun.transform.SetParent(null);
            sun.speed = _sunSpeed;
            _rb.AddForce(-transform.forward * 75 * timer);
            sun.Shoot();
        }
        else if (_inputs.SecondaryAttack)
        {
            var hitDamage = sun.damage * 1.5f;
            sun.Absorb(_sunAbsorbTime);

            //podria ir una animacion de absorber y cuando termine pasar a la piña
            anim.SetBool("IsAttacking", true); //aca iria animacion de piña

            yield return new WaitForSeconds(_sunAbsorbTime);

            _rb.velocity = Vector3.zero;
            _rb.AddForce(transform.forward * _stepStr);

            float timer2 = 0;

            while (_sunMeleeDuration > timer2)
            {
                timer2 += Time.deltaTime;

                if (Physics.BoxCast(transform.position, _sunHitbox, transform.forward, out var hit, transform.rotation, _sunRange))
                {
                    if (hit.collider.gameObject.layer == 3)
                    {
                        currentBoss.KnockBack(transform.forward, 100 * timer);
                        currentBoss.TakeDamage(hitDamage);
                        _rb.velocity = Vector3.zero;
                        break;
                    }
                }

                yield return null;
            }
        }

        _sunCurrentCooldown = _sunCooldown;

        yield return new WaitForSeconds(_sunRecovery);

        anim.SetBool("IsAttacking", false);
        _usingSun = false;
        _inputs.PrimaryAttack = false;
        _inputs.SecondaryAttack = false;
        _movement.Cast(false);
    } // en desuso

    IEnumerator AimedSunMagic()
    {
        _rb.angularVelocity = Vector3.zero;
        _inputs.ToggleAim(true);
        _usingSun = true;
        _inputs.inputUpdate = _inputs.Aiming;

        yield return new WaitForSeconds(_sunCastDelay);

        ControlFullScreen.instance.ChangeDemond(true);

        _movement.Cast(true);
        ChangeAudio(chargingSun);

        do
        {
            var sun = Instantiate(_sunMagic, transform.position + transform.forward * 1f + transform.up * 0.6f, transform.rotation, transform);
            sun.player = this;
            sun.damage = _sunBaseDamage;

            float timer = 0;

            while (!_stopSun && _inputs.SecondaryAttack && !_inputs.launchAttack && timer < _sunMaxChargeTime && CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
            {
                timer += Time.deltaTime;

                sun.damage += _sunDamageGrowRate * Time.deltaTime;

                yield return null;
            }

            if (timer >= _sunMaxChargeTime || !CheckAndReduceStamina(_sunHoldCost * Time.deltaTime))
            {
                sun.ChargeFinished();
            }

            while (!_stopSun && _inputs.SecondaryAttack && !_inputs.launchAttack)
            {
                CheckAndReduceStamina(0);
                yield return null;
            }

            if (_stopSun || !_inputs.SecondaryAttack)
            {
                sun.transform.SetParent(null);
                sun.StartCoroutine(sun.Death());
                ControlFullScreen.instance.ChangeDemond(false);
            }
            else
            {
                _inputs.launchAttack = false;

                anim.SetBool("IsAttacking", true);

                yield return new WaitForSeconds(_sunCastDelay);

                anim.SetBool("IsAttacking", false);

                sun.transform.SetParent(null);

                Vector3 dir;
                var cameraTransform = _cameraController.AimCamera.transform;

                Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit);
                if (hit.collider)
                {
                    dir = hit.point - sun.transform.position;
                }
                else
                {
                    dir = Camera.main.transform.forward;
                }

                sun.transform.forward = dir;
                sun.speed = _sunSpeed;
                sun.Shoot();

                ControlFullScreen.instance.ChangeDemond(false);

                timer = 0;

                while (timer < _sunRecovery && _inputs.SecondaryAttack)
                {
                    timer += Time.deltaTime;

                    yield return null;
                }
            }

            _inputs.launchAttack = false;
        } while (!_stopSun && _inputs.SecondaryAttack && CheckAndReduceStamina(_sunBaseCost));

        _movement.Cast(false);
        _inputs.ToggleAim(false);
        //_sunCurrentCooldown = _sunCooldown

        yield return new WaitForSeconds(0.25f);

        _inputs.SecondaryAttack = false;
        _stopSun = false;
        _usingSun = false;
        //_sunCurrentCooldown = _sunCooldown;
    }

    void ActivateObsidianMagic()
    {
        if (_movement.IsGrounded() && _obsidianCurrentCooldown <= 0 && CheckAndReduceStamina(_obsidianCost))
        {
            StartCoroutine(ObsidianMagic());
        }
        else
        {
            _inputs.PrimaryAttack = false;
        }
    }

    IEnumerator ObsidianMagic()
    {
        _inputs.inputUpdate = _inputs.FixedCast;
        var lookAt = Camera.main.transform.forward.MakeHorizontal();
        transform.forward = lookAt;
        anim.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(_obsidianCastDelay);

        for (int i = _shardsPerWave; i < _shardsPerWave + _maxWaves; i++)
        {
            var baseRotationChange = i % 2 == 0 ? _shardAngleOffset * 0.5f : 0;

            for (int j = 0; j < i; j++)
            {
                var individualRotationChange = j % 2 == 0 ? _shardAngleOffset * Mathf.CeilToInt(j * 0.5f) : -_shardAngleOffset * Mathf.CeilToInt(j * 0.5f);

                var finalRotation = transform.rotation.AddYRotation(baseRotationChange + individualRotationChange);

                var shard = Instantiate(_obsidianShard, transform.position + transform.forward * 0.65f, finalRotation);
                shard.speed = _shardSpeed;
                shard.damage = _obsidianDamage;
            }

            float timer = 0;

            while (timer < _obsidianComboInterval)
            {
                lookAt = Camera.main.transform.forward.MakeHorizontal();
                transform.forward = lookAt;

                timer += Time.deltaTime;

                if (!_inputs.PrimaryAttack)
                {
                    _obsidianCurrentCooldown = _obsidianCooldown;
                    anim.SetBool("IsAttacking", false);
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
        anim.SetBool("IsAttacking", false);
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
        if (_damageCurrentCooldown > 0)
        {
            _damageCurrentCooldown -= Time.deltaTime;
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
        //_inputs.PrimaryAttack = false;
        if (_usingSun) _stopSun = true;

        if (_damageCurrentCooldown > 0) return;
        ChangeAudio(damage);
        _cameraController.CameraShake(1, 0.5f);

        _damageCurrentCooldown = _damageCooldown;

        anim.SetBool("IsHit", true);
        anim.SetBool("IsHit", false);

        base.TakeDamage(amount);
        
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
        _inputs.PrimaryAttack = false;
    }

    public void FightStarts(Boss boss)
    {
        _cameraController.CameraShake(2, 1);
        _movement.Combat(true);
        currentBoss = boss;
    }

    public void FightEnds()
    {
        _cameraController.CameraShake(2, 1);
        _movement.Combat(false);
        currentBoss = null;
    }

    public void StopCast()
    {
        anim.SetBool("IsAttacking",false);
        _inputs.PrimaryAttack = false;
    }

    public void RunningAnimation(bool play)
    {
        anim.SetBool("IsRunning", play);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            _rb.velocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            _inputs.inputUpdate = _inputs.Nothing;
            camaraFinal.SetActive(true);
            _cameraController.Final();
            UIManager.instance.Final();
        }
    }
    public void ChangeAudio(AudioClip clip)
    {
        _myAS.clip = clip;
        _myAS.PlayOneShot(_myAS.clip);
    }
    public IEnumerator PrendoPlayerDust(bool prendo)
    {
        ForceFieldPlayer.SetActive(prendo);
        if(prendo)
        {
            dustPlayer.Play();
        }
        yield return new WaitForSeconds(1f);
        ForceFieldPlayer.SetActive(false);
    }
}
