using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianGod : Entity
{
    [SerializeField] PlayerController _player;

    Rigidbody _rb;

    public GameObject placeholderSwingHitboxFeedback;

    [SerializeField] LayerMask playerMask;

    [Header("Walk")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _walkDuration;

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

    FiniteStateMachine _fsm;

    public enum ObsidianStates
    {
        Walk,
        Swing,
        Spikes,
        Shards,
        Wave
    }

    public bool takingAction = false;

    public Renderer renderer;

    bool _lookAtPlayer, _move;

    public bool LookAtPlayer
    {
        get
        {
            return _lookAtPlayer;
        }

        set
        {
            _rb.velocity = Vector3.zero;
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

        _fsm.AddState(ObsidianStates.Walk, new WalkState(this, _walkDuration));
        _fsm.AddState(ObsidianStates.Swing, new SwingState(this));
        _fsm.AddState(ObsidianStates.Spikes, new SpikesState(this));
        _fsm.AddState(ObsidianStates.Shards, new ShardsState(this));
        _fsm.AddState(ObsidianStates.Wave, new WaveState(this));

        _fsm.ChangeState(ObsidianStates.Walk);
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

    public void Walk()
    {
        LookAtPlayer = true;
        _move = true;
    }

    public void StopWalking()
    {
        _move = false;
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

    public void Leap()
    {

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

        takingAction = false;
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
