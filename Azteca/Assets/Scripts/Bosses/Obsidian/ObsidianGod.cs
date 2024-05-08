using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObsidianGod : Entity
{
    [SerializeField] PlayerController _player;

    Rigidbody _rb;

    public GameObject placeholderSwingHitboxFeedback;

    [SerializeField] LayerMask playerMask;

    [SerializeField] int _comboChanceReduction;
    [SerializeField] float _fightTriggerRange;

    [Header("Walk")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _baseWalkDuration, _walkDurationPerCombo;

    [Header("Swing")]
    [SerializeField] Transform _meleeHitboxCenter;
    [SerializeField] float _swingForwardForce, _swingPreparation, _swingDamage, _swingDuration, _swingRecovery, _meleeBoxX, _meleeBoxY, _meleeBoxZ;
    Vector3 _meleeBox;

    [Header("Shards")]
    [SerializeField] Projectile _shard;
    [SerializeField] int _shardAmount;
    [SerializeField] float _shardsPreparation, _shardAngle, _shardSpeed, _shardDamage, _shardsInterval, _shardsRecovery;

    [Header("Wave")]
    [SerializeField] Projectile _wave;
    [SerializeField] float _wavePreparation, _waveSpeed, _waveDamage, _waveRecovery;

    [Header("Spikes")]
    [SerializeField] Hazard _spikes;
    [SerializeField] float _spikesPreparation, _spikesDuration, _spikesDamage, _spikesRecovery;

    [Header("Dash")]
    [SerializeField] float _dashStrength;
    [SerializeField] float _dashPreparation, _dashDuration, _dashRecovery;

    FiniteStateMachine _fsm;

    public enum ObsidianStates
    {
        Inactive,
        Walk,
        Swing,
        Spikes,
        Shards,
        Wave,
        Dash
    }

    [HideInInspector] public bool takingAction = false;

    public Renderer renderer;

    bool _lookAtPlayer, _move;

    [HideInInspector] public int comboCount;

    ObsidianStates _lastAction;

    public bool LookAtPlayer
    {
        get
        {
            return _lookAtPlayer;
        }

        set
        {
            _rb.constraints = value ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ : RigidbodyConstraints.FreezeRotation;
            _lookAtPlayer = value;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _meleeBox = new Vector3(_meleeBoxX, _meleeBoxY, _meleeBoxZ);
    }

    protected override void Start()
    {
        base.Start();

        UIManager.instance.UpdateBar(UIManager.Bar.BossHp, _hp, _maxHp);

        _fsm = new FiniteStateMachine();

        _fsm.AddState(ObsidianStates.Inactive, new InactiveState(this, _player.transform, _fightTriggerRange));
        _fsm.AddState(ObsidianStates.Walk, new WalkState(this, _baseWalkDuration, _walkDurationPerCombo));
        _fsm.AddState(ObsidianStates.Swing, new SwingState(this));
        _fsm.AddState(ObsidianStates.Spikes, new SpikesState(this));
        _fsm.AddState(ObsidianStates.Shards, new ShardsState(this));
        _fsm.AddState(ObsidianStates.Wave, new WaveState(this));
        _fsm.AddState(ObsidianStates.Dash, new DashState(this));

        _fsm.ChangeState(ObsidianStates.Inactive);
        _lastAction = ObsidianStates.Inactive;
    }

    private void Update()
    {
        _fsm.Update();
    }

    private void FixedUpdate()
    {
        if (_move)
        {
            _rb.MovePosition(transform.position + transform.forward * _walkSpeed * Time.fixedDeltaTime);
        }
        if (LookAtPlayer)
        {
            _rb.MoveRotation(Quaternion.LookRotation((_player.transform.position - transform.position).MakeHorizontal()));
        }
    }

    public void ToggleWalk(bool start)
    {
        _move = start;
    }

    public void Shards(int waves)
    {
        takingAction = true;

        StartCoroutine(ThrowingShards(_shardAmount + waves));
    }

    public void Spikes()
    {
        takingAction = true;

        StartCoroutine(Spiking());
    }

    public void Wave()
    {
        takingAction = true;

        StartCoroutine(ThrowingWave());
    }

    public void Dash()
    {
        takingAction = true;

        StartCoroutine(Dashing());
    }

    public void Swing()
    {
        takingAction = true;
        
        StartCoroutine(Swinging());
    }

    IEnumerator Swinging()
    {
        yield return new WaitForSeconds(_swingPreparation);

        LookAtPlayer = false;

        _rb.AddForce(transform.forward * _swingForwardForce);

        placeholderSwingHitboxFeedback.SetActive(true);

        bool hit = false;
        float timer = 0;

        while (_swingDuration > timer)
        {
            timer += Time.deltaTime;

            if (Physics.CheckBox(_meleeHitboxCenter.position, _meleeBox, transform.rotation, playerMask) && !hit)
            {
                _player.TakeDamage(_swingDamage);
                hit = true;
            }

            yield return null;
        }

        _rb.velocity = Vector3.zero;
        placeholderSwingHitboxFeedback.SetActive(false);

        yield return new WaitForSeconds(_swingRecovery);

        LookAtPlayer = true;
        takingAction = false;
    }

    IEnumerator ThrowingShards(int shardLimit)
    {
        yield return new WaitForSeconds(_shardsPreparation);

        LookAtPlayer = false;

        for (int i = _shardAmount; i < shardLimit; i++)
        {
            var baseRotationChange = i % 2 == 0 ? _shardAngle * 0.5f : 0;

            for (int j = 0; j < i; j++)
            {
                var individualRotationChange = j % 2 == 0 ? _shardAngle * Mathf.CeilToInt(j * 0.5f) : -_shardAngle * Mathf.CeilToInt(j * 0.5f);

                var finalRotation = transform.rotation.AddYRotation(baseRotationChange + individualRotationChange);

                var shard = Instantiate(_shard, transform.position, finalRotation);
                shard.speed = _shardSpeed;
                shard.damage = _shardDamage;
            }

            yield return new WaitForSeconds(_shardsInterval);
        }

        yield return new WaitForSeconds(_shardsRecovery);

        LookAtPlayer = true;
        takingAction = false;
    }

    IEnumerator WIPThrowingShards(int shardLimit)
    {
        yield return new WaitForSeconds(_shardsPreparation);

        LookAtPlayer = false;

        for (int i = _shardAmount; i < shardLimit; i++)
        {
            var baseRotationChange = i % 2 == 0 ? _shardAngle * 0.5f : 0;

            var baseRotation = 360 / i;

            for (int j = 0; j < i; j++)
            {
                var individualRotationChange = j % 2 == 0 ? _shardAngle * Mathf.CeilToInt(j * 0.5f) : -_shardAngle * Mathf.CeilToInt(j * 0.5f);

                var finalRotation = transform.rotation.AddYRotation(baseRotationChange + individualRotationChange);

                var shard = Instantiate(_shard, transform.position, finalRotation);
                shard.speed = _shardSpeed;
                shard.damage = _shardDamage;
            }

            yield return new WaitForSeconds(_shardsInterval);
        }

        yield return new WaitForSeconds(_shardsRecovery);

        LookAtPlayer = true;
        takingAction = false;
    }

    IEnumerator ThrowingWave()
    {
        yield return new WaitForSeconds(_wavePreparation);

        LookAtPlayer = false;

        var wave = Instantiate(_wave, transform.position + transform.forward, transform.rotation);
        wave.speed = _waveSpeed;
        wave.damage = _waveDamage;

        yield return new WaitForSeconds(_waveRecovery);

        LookAtPlayer = true;
        takingAction = false;
    }

    IEnumerator Spiking()
    {
        LookAtPlayer = false;

        yield return new WaitForSeconds(_spikesPreparation);

        var spikes = Instantiate(_spikes, transform.position - Vector3.up * 1.35f, transform.rotation);
        spikes.duration = _spikesDuration;
        spikes.damage = _spikesDamage;

        yield return new WaitForSeconds(_spikesRecovery);

        LookAtPlayer = true;
        takingAction = false;
    }

    IEnumerator Dashing()
    {
        yield return new WaitForSeconds(_dashPreparation);

        _rb.AddForce(transform.forward * _dashStrength);

        LookAtPlayer = false;

        float timer = 0;

        while (timer < _dashDuration)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        _rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(_spikesRecovery);

        LookAtPlayer = true;
        takingAction = false;
    }

    public ObsidianStates GetAction()
    {
        if (_lastAction == ObsidianStates.Dash)
        {
            _lastAction = PickAction(false);
            Debug.Log("elijo accion que no sea dash sin subir combo");
            return _lastAction;
        }
        else if (_lastAction == ObsidianStates.Walk)
        {
            _lastAction = PickAction(true);
            Debug.Log("elijo primer accion del combo");
            return _lastAction;
        }
        else if (Random.Range(0,100) < 100 - _comboChanceReduction * comboCount)
        {
            Debug.Log("gane la chance de combo, elijo accion");
            comboCount++;
            _lastAction = PickAction(true);
            return _lastAction;
        }
        else
        {
            Debug.Log("perdi la chance de combo, camino");
            _lastAction = ObsidianStates.Walk;
            return _lastAction;
        }
    }

    ObsidianStates PickAction(bool canDash)
    {
        var possibleActions = new List<ObsidianStates>();

        var dist = GetPlayerDistance();

        if (canDash) possibleActions.Add(ObsidianStates.Dash);

        if (dist > 13)
        {
            possibleActions.Add(ObsidianStates.Wave);
            possibleActions.Add(ObsidianStates.Shards);
        }
        else if (dist > 8.5f)
        {
            possibleActions.Add(ObsidianStates.Spikes);
            possibleActions.Add(ObsidianStates.Shards);
        }
        else
        {
            possibleActions.Remove(ObsidianStates.Dash);
            possibleActions.Add(ObsidianStates.Swing);
            possibleActions.Add(ObsidianStates.Spikes);
        }

        possibleActions.Remove(_lastAction);

        return possibleActions.Skip(Random.Range(0, possibleActions.Count)).First();
    }

    public float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        UIManager.instance.UpdateBar(UIManager.Bar.BossHp, _hp);
    }

    public override void Die()
    {
        UIManager.instance.TurnOffBossBar();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_meleeHitboxCenter.position, _meleeBox * 2);
    }
}
