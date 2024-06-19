using IA2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Itztlacoliuhqui : Boss
{
    public enum Actions
    {
        Inactive,
        Search,
        Spikes,
        Swing,
        BreakWall,
        Shield,
        Hide,
        WallSpike,
        Gatling,
        Charge,
        ArenaSpikes,
        Leap
    }

    EventFSM<Actions> _fsm;

    BehaviorNode _treeStart;

    Pathfinding _pf;
    List<Vector3> _path = new List<Vector3>();

    [SerializeField] Animator _anim;
    [SerializeField] float _aggroRange;
    [SerializeField] Transform _eyePos;
    [SerializeField] GameObject _edgeBlock;

    [Header("Walls")]
    [SerializeField] ObsidianWall _wallPrefab;
    [SerializeField] List<ObsidianWall> _spawnedWalls;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] float _wallCloseRange, _wallBreakRange, _playerCloseRange;

    [Header("Shards")]
    [SerializeField] Projectile _shardPrefab;
    [SerializeField] float _shardSpeed, _shardDamage;

    [Header("Search")]
    [SerializeField] float _searchSpeed;

    [Header("Spikes")]
    [SerializeField] Hazard _spikes;
    [SerializeField] ParticleSystem[] _preJumpParticles;
    [SerializeField] float _spikesPreparation, _spikesDuration, _spikesDamage, _spikesRecovery;

    [Header("Break Wall")]
    [SerializeField] int _shardAmount;
    [SerializeField] float _breakWallPreparation, _breakWallRecovery, _breakBaseSpawnOffsetY, _breakSpawnVariationY, _breakAimVariationX, _breakAimVariationY;

    [Header("Shield")]
    [SerializeField] float _shieldPreparation;
    [SerializeField] float _shieldRecovery, _forwardOffset;

    [Header("Hide")]
    [SerializeField] float _hideSpeed;
    [SerializeField] float _hideDuration;

    [Header("Wall Spike")]
    [SerializeField] GameObject _miniWall;
    [SerializeField] float _firstWallOffset, _miniWallOffset, _miniWallInterval, _wallSpikeKnockback,
                           _wallSpikeDamage, _wallSpikePreparation, _wallSpikeRecovery, _miniWallDestroyDelay;

    [Header("Gatling")]
    [SerializeField] Transform _handPos;
    [SerializeField] float _gatlingDuration, _gatlingShardInterval, _gatlingPreparation, _gatlingRecovery, _gatlingSpawnVariationX, _gatlingSpawnVariationY, _gatlingAimVariationX, _gatlingAimVariationY;

    [Header("Dash (Placeholder)")]
    [SerializeField] float _dashStrength;
    [SerializeField] float _dashPreparation, _dashDuration, _dashRecovery;

    [Header("Charge")]
    [SerializeField] Vector3 _chargeBoxSize;
    [SerializeField] float _chargeSpeed, _chargeHitRange, _chargeDamage, _chargeKnockback, _chargePreparation, _chargeRecovery;

    [SerializeField] PlayerController _player;
    [SerializeField] LayerMask _playerLayer, _magicLayer;

    [Header("Leap")]
    [SerializeField] float _leapHeight;
    [SerializeField] float _leapKnockback, _leapDamage, _leapPreparation, _leapDuration, _leapRecovery;

    ObsidianWall _wallBlockingLOS;

    ObsidianPathfindManager _pfManager { get => ObsidianPathfindManager.instance; }

    bool _takingAction = false, _lookAtPlayer = false, _move = false, _start = false;

    float _timer = 0, _currentSpeed = 0;

    //[SerializeField] Animator anim;

    AudioSource _myAS;
    [SerializeField] AudioClip stomp, dash, dashBox, dashFuerte, lanzaDardos, walk;

    [SerializeField] GameObject tornadoPiedras, caidaPiedras;

    public bool LookAtPlayer
    {
        get
        {
            return _lookAtPlayer;
        }

        set
        {
            //_rb.constraints = value ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ : RigidbodyConstraints.FreezeRotation;
            _lookAtPlayer = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        UIManager.instance.UpdateBar(UIManager.Bar.BossHp, _hp, _maxHp);

        StartCoroutine(SetupWait());
    }

    IEnumerator SetupWait()
    {
        yield return new WaitForSeconds(1);

        Setup();
        _start = true;
    }

    private void Setup()
    {
        _rb = GetComponent<Rigidbody>();
        _pf = new Pathfinding();

        #region FSM State Creation

        var inactive = new State<Actions>("Inactive");
        var search = new State<Actions>("Search");
        var spikes = new State<Actions>("Spikes");
        var swing = new State<Actions>("Swing");
        var breakWall = new State<Actions>("BreakWall");
        var shield = new State<Actions>("Shield");
        var hide = new State<Actions>("Hide");
        var wallSpike = new State<Actions>("WallSpike");
        var gatling = new State<Actions>("Gatling");
        var charge = new State<Actions>("Charge");
        var arenaSpikes = new State<Actions>("ArenaSpikes");
        var leap = new State<Actions>("Leap");

        StateConfigurer.Create(inactive)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(search)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(spikes)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(swing)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(breakWall)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(shield)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(hide)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(wallSpike)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(gatling)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(charge)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(arenaSpikes)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        StateConfigurer.Create(leap)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .SetTransition(Actions.Leap, leap)
            .Done();

        #endregion

        #region FSM State Setup

        inactive.OnUpdate += () =>
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= _aggroRange)
            {
                _player.FightStarts(this);
                _edgeBlock.SetActive(true);
                UIManager.instance.ToggleBossBar(true);
                _treeStart.Execute();
            }
        };

        search.OnEnter += x =>
        {
            Debug.Log("Start search");
            _currentSpeed = _searchSpeed;
            _path = _pf.ThetaStar(_pfManager.FindNodeClosestTo(transform.position), _pfManager.FindNodeClosestTo(_player.transform.position), _wallLayer);
            _move = true;
        };

        search.OnUpdate += () =>
        {
            if (transform.position.InLineOfSightOf(_player.transform.position, _wallLayer) || _path.Count == 0 || _path == null)
            {
                _treeStart.Execute();
            }
        };

        search.OnFixedUpdate += () =>
        {
            TravelPath();
        };

        search.OnExit += x =>
        {
            _currentSpeed = 0;
            _move = false;
        };

        spikes.OnEnter += x =>
        {
            Debug.Log("Start spikes");
            _takingAction = true;
            _anim.SetBool("IsStomp", true);
            StartCoroutine(Spiking());
        };

        spikes.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        spikes.OnExit += x =>
        {
            _anim.SetBool("IsStomp", false);
        };

        breakWall.OnEnter += x =>
        {
            Debug.Log("Start break wall");
            _takingAction = true;
            StartCoroutine(BreakingWall());
        };

        breakWall.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        shield.OnEnter += x =>
        {
            Debug.Log("Start shield");
            _takingAction = true;
            StartCoroutine(Shielding());
        };

        shield.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        hide.OnEnter += x =>
        {
            Debug.Log("Start hide");
            _currentSpeed = _hideSpeed;
            _timer = 0;
            var closestWall = _spawnedWalls.Where(x => !x.Broken).OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
            Vector3 hidingSpot = closestWall.transform.position + (_player.transform.position - closestWall.transform.position).normalized * -closestWall.Radius;
            _path = _pf.ThetaStar(_pfManager.FindNodeClosestTo(transform.position), _pfManager.FindNodeClosestTo(hidingSpot), _wallLayer);
            _move = true;
        };

        hide.OnUpdate += () =>
        {
            if (_timer < _hideDuration)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _treeStart.Execute();
            }
        };

        hide.OnFixedUpdate += () =>
        {
            if (_path.Count == 0 || _path == null) return;
            TravelPath();
        };

        hide.OnExit += x =>
        {
            _currentSpeed = 0;
            _move = false;
        };

        wallSpike.OnEnter += x =>
        {
            Debug.Log("Start wall spike");
            _takingAction = true;
            StartCoroutine(WallSpiking());
        };

        wallSpike.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        gatling.OnEnter += x =>
        {
            Debug.Log("Start gatling");
            _takingAction = true;
            StartCoroutine(UsingGatling());
        };

        gatling.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        charge.OnEnter += x =>
        {
            Debug.Log("Start charge");
            _takingAction = true;
            StartCoroutine(Charging());
        };

        charge.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        leap.OnEnter += x =>
        {
            _takingAction = true;
            StartCoroutine(Leaping());
        };

        leap.OnUpdate += () =>
        {
            if (!_takingAction) _treeStart.Execute();
        };

        #endregion

        _fsm = new EventFSM<Actions>(inactive);

        #region Decision Tree Setup

        var searchNode = new ActionNode(Search);
        var spikesNode = new ActionNode(Spikes);
        var swingNode = new ActionNode(Swing);
        var breakWallNode = new ActionNode(BreakWall);
        var shieldNode = new ActionNode(Shield);
        var hideNode = new ActionNode(Hide);
        var wallSpikeNode = new ActionNode(WallSpike);
        var gatlingNode = new ActionNode(Gatling);
        var chargeNode = new ActionNode(Charge);
        var arenaSpikesNode = new ActionNode(ArenaSpikes);

        var wallClose = new QuestionNode(hideNode, shieldNode, IsWallClose);
        var wallCloseToPlayer = new QuestionNode(gatlingNode, wallSpikeNode, IsWallCloseToPlayer);
        var defend = new QuestionNode(wallClose, wallCloseToPlayer, ShouldDefend);
        var playerSunning = new QuestionNode(defend, chargeNode, IsPlayerUsingSun);
        var playerInWallLOS = new QuestionNode(breakWallNode, searchNode, BreakableWallInPlayerLOS);
        var playerClose = new QuestionNode(spikesNode, playerSunning, IsPlayerClose);
        var breakWallInLOS = new QuestionNode(playerInWallLOS, searchNode, CanBreakWallBlockingLOS);
        _treeStart = new QuestionNode(playerClose, breakWallInLOS, IsPlayerInLOS);

        #endregion
    }

    private void Update()
    {
        if (!_start) return;

        _fsm.Update();
    }

    private void FixedUpdate()
    {
        if (!_start) return;

        _fsm.FixedUpdate();

        if (_move)
        {
            _rb.MovePosition(transform.position + transform.forward * _currentSpeed * Time.fixedDeltaTime);
        }
        if (LookAtPlayer)
        {
            _rb.MoveRotation(Quaternion.LookRotation((_player.transform.position - transform.position).MakeHorizontal()));
        }
    }

    #region Decision Tree Methods

    void Search() => _fsm.SendInput(Actions.Search);
    void Spikes() => _fsm.SendInput(Actions.Spikes);
    void Swing() => _fsm.SendInput(Actions.Swing);
    void BreakWall() => _fsm.SendInput(Actions.BreakWall);
    void Shield() => _fsm.SendInput(Actions.Shield);
    void Hide() => _fsm.SendInput(Actions.Hide);
    void WallSpike() => _fsm.SendInput(Actions.WallSpike);
    void Gatling() => _fsm.SendInput(Actions.Gatling);
    void Charge() => _fsm.SendInput(Actions.Charge);
    void ArenaSpikes() => _fsm.SendInput(Actions.ArenaSpikes);

    bool IsWallClose()
    {
        foreach (var item in _spawnedWalls)
        {
            if (Vector3.Distance(transform.position, item.transform.position) <= _wallCloseRange)
            {
                return true;
            }
        }

        return false;
    }
    bool IsWallCloseToPlayer()
    {
        foreach (var item in _spawnedWalls)
        {
            if (Vector3.Distance(_player.transform.position, item.transform.position) <= _wallCloseRange)
            {
                return true;
            }
        }

        return false;
    }
    bool ShouldDefend() => Random.Range(0, 2) == 0;
    bool IsPlayerUsingSun() => _player.UsingSun;
    bool BreakableWallInPlayerLOS()
    {
        _player.transform.position.InLineOfSightOf(_wallBlockingLOS.transform.position, _wallLayer, out var hit);

        if (hit.collider.GetComponentInParent<ObsidianWall>() == _wallBlockingLOS)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CanBreakWallBlockingLOS() => Vector3.Distance(transform.position, _wallBlockingLOS.transform.position) <= _wallBreakRange && !_wallBlockingLOS.Broken;
    bool IsPlayerClose() => Vector3.Distance(transform.position, _player.transform.position) <= _playerCloseRange;
    bool IsPlayerInLOS()
    {
        if (!_eyePos.position.InLineOfSightOf(_player.transform.position + Vector3.up, _wallLayer, out var hit))
        {
            _wallBlockingLOS = hit.collider.GetComponentInParent<ObsidianWall>();
            return false;
        }
        else
        {
            _wallBlockingLOS = null;
            return true;
        }
    }

    #endregion

    void FixRotation(bool fix)
    {
        _rb.constraints = fix ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void TravelPath()
    {
        Vector3 posTarget = _path[0];
        posTarget.y = transform.position.y;
        Vector3 dir = posTarget - transform.position;
        if (dir.magnitude < 0.1f)
        {
            _rb.MovePosition(posTarget);
            _path.RemoveAt(0);

            if (_path.Count == 0)
            {
                _currentSpeed = 0;
                _move = false;
            }
        }

        _rb.MoveRotation(Quaternion.LookRotation(dir.MakeHorizontal()));
        
    }

    public void WallDestroyed(ObsidianWall wall)
    {
        if (_spawnedWalls.Contains(wall)) _spawnedWalls.Remove(wall);
    }

    IEnumerator Spiking()
    {
        //LookAtPlayer = false;
        //ChangeAudio(dash);
        //preparacion
        for (int i = 0; i < _preJumpParticles.Length; i++)
        {
            _preJumpParticles[i].Play();
        }
        prenderCaidaPiedras(true);
        yield return new WaitForSeconds(_spikesPreparation);
        //salen spikes del suelo
        //ChangeAudio(stomp);
        prenderCaidaPiedras(false);
        var spikes = Instantiate(_spikes, transform.position - Vector3.up * 1.65f, transform.rotation);
        spikes.duration = _spikesDuration;
        spikes.damage = _spikesDamage;
        for (int i = 0; i < _preJumpParticles.Length; i++)
        {
            _preJumpParticles[i].Stop();
        }
        //recuperacion
        yield return new WaitForSeconds(_spikesRecovery);

        //LookAtPlayer = true;
        _takingAction = false;
    }

    IEnumerator BreakingWall()
    {
        _rb.MoveRotation(Quaternion.LookRotation((_wallBlockingLOS.transform.position - transform.position).MakeHorizontal()));

        yield return new WaitForSeconds(_breakWallPreparation);

        if (!_wallBlockingLOS.Broken && !IsPlayerInLOS())
        {
            _wallBlockingLOS.Break();

            Vector3 basePos = new Vector3(_wallBlockingLOS.transform.position.x, transform.position.y + _breakBaseSpawnOffsetY, _wallBlockingLOS.transform.position.z);
            float xPosVariation = _wallBlockingLOS.Radius;
            Vector3 baseDir = (_player.transform.position - basePos).normalized;

            for (int i = 0; i < _shardAmount; i++)
            {
                var shard = Instantiate(_shardPrefab, basePos.VectorVariation(1, xPosVariation, _breakSpawnVariationY), Quaternion.identity);
                shard.transform.forward = baseDir.VectorVariation(i * 0.5f, _breakAimVariationX, _breakAimVariationY);
                shard.speed = _shardSpeed;
                shard.damage = _shardDamage;
            }

            yield return new WaitForSeconds(_breakWallRecovery);
        }
       
        //LookAtPlayer = true;
        _takingAction = false;
    }

    IEnumerator Shielding()
    {
        LookAtPlayer = true;

        yield return new WaitForSeconds(_shieldPreparation);

        var wall = Instantiate(_wallPrefab, transform.position + transform.forward * _forwardOffset - Vector3.up * _distFromPivotToFloor, Quaternion.identity);
        _spawnedWalls.Add(wall);
        wall.boss = this;

        yield return new WaitForSeconds(_shieldRecovery);

        LookAtPlayer = false;
        _takingAction = false;
    }

    IEnumerator WallSpiking()
    {
        LookAtPlayer = true;

        yield return new WaitForSeconds(_wallSpikePreparation);

        LookAtPlayer = false;

        Vector3 target = _player.transform.position;
        Vector3 dir = (target - transform.position).MakeHorizontal().normalized;

        List<GameObject> miniWallList = new List<GameObject>();

        Vector3 nextSpawnPos = transform.position - transform.up * _distFromPivotToFloor + dir * _firstWallOffset;
        Vector3 movement = dir * _miniWallOffset;
        target = new Vector3(target.x, nextSpawnPos.y, target.z);

        do
        {
            miniWallList.Add(Instantiate(_miniWall, nextSpawnPos, Quaternion.identity));
            nextSpawnPos += movement;

            yield return new WaitForSeconds(_miniWallInterval);
        }
        while (Vector3.Distance(nextSpawnPos, target) > _wallPrefab.Radius);

        nextSpawnPos = target;
        var wall = Instantiate(_wallPrefab, nextSpawnPos, Quaternion.identity);
        _spawnedWalls.Add(wall);
        wall.boss = this;

        if (Physics.CheckCapsule(nextSpawnPos, nextSpawnPos + Vector3.up * 5, wall.Radius, _playerLayer))
        {
            _player.KnockBack(_player.transform.position - nextSpawnPos, _wallSpikeKnockback);
            _player.TakeDamage(_wallSpikeDamage);
        }

        yield return new WaitForSeconds(_wallSpikeRecovery);

        _takingAction = false;

        yield return new WaitForSeconds(_miniWallDestroyDelay);

        foreach (var item in miniWallList)
        {
            Destroy(item);
        }
    }

    IEnumerator UsingGatling()
    {
        LookAtPlayer = true;

        yield return new WaitForSeconds(_gatlingPreparation);

        float timer = 0, cooldown = 0;

        while (timer < _gatlingDuration)
        {
            if (cooldown >= _gatlingShardInterval)
            {
                var pos = _handPos.transform.position;
                var dir = (_player.transform.position - pos).normalized;

                var shard = Instantiate(_shardPrefab, pos.VectorVariation(1, _gatlingSpawnVariationX, _gatlingSpawnVariationY), Quaternion.identity);
                shard.transform.forward = dir.VectorVariation(1, _gatlingAimVariationX, _gatlingAimVariationY);
                shard.speed = _shardSpeed;
                shard.damage = _shardDamage;
                cooldown = 0;
            }
            else
            {
                cooldown += Time.deltaTime;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(_gatlingRecovery);

        LookAtPlayer = false;
        _takingAction = false;
    }

    IEnumerator Dashing()
    {
        LookAtPlayer = true;
        //preparacion
        yield return new WaitForSeconds(_dashPreparation);
        //comienza dash
        //ChangeAudio(dash);
        _rb.AddForce(transform.forward * _dashStrength);

        LookAtPlayer = false;
        FixRotation(true);

        float timer = 0;

        while (timer < _dashDuration)
        {
            timer += Time.deltaTime;

            yield return null;
        }
        //termina dash
        _rb.velocity = Vector3.zero;
        //espero
        yield return new WaitForSeconds(_dashRecovery);

        //LookAtPlayer = true;
        FixRotation(false);
        _takingAction = false;
    }

    IEnumerator Charging()
    {
        LookAtPlayer = true;
        //preparacion
        yield return new WaitForSeconds(_chargePreparation);

        LookAtPlayer = false;
        FixRotation(true);

        _currentSpeed = _chargeSpeed;
        _move = true;

        while (_move)
        {
            if (Physics.BoxCast(transform.position, _chargeBoxSize, transform.forward, out var hit, transform.rotation, _chargeHitRange))
            {
                if (hit.collider.gameObject.layer == Mathf.Log(_wallLayer.value, 2))
                {
                    Debug.Log("hit wall");
                    var wallHit = hit.collider.GetComponentInParent<ObsidianWall>();
                    var broken = wallHit.Broken;

                    wallHit.Die();

                    if (!broken)
                    {
                        _move = false;
                    }
                }
                else if (hit.collider.gameObject.layer == Mathf.Log(_playerLayer.value, 2))
                {
                    Debug.Log("hit player");
                    _player.KnockBack((_player.transform.position - transform.position + transform.up).normalized, _chargeKnockback);
                    _player.TakeDamage(_chargeDamage);
                    _move = false;
                }
                else if (hit.collider.gameObject.layer != Mathf.Log(_magicLayer.value, 2))
                {
                    Debug.Log("hit edge");
                    _move = false;
                }
            }

            yield return null;
        } 

        
        //espero
        yield return new WaitForSeconds(_chargeRecovery);

        //LookAtPlayer = true;
        FixRotation(false);
        _takingAction = false;
    }

    IEnumerator Leaping()
    {
        LookAtPlayer = true;
        //preparacion
        yield return new WaitForSeconds(_chargePreparation);

        LookAtPlayer = false;
        FixRotation(true);
        _rb.isKinematic = true;

        ObsidianWall wallToDestroy = null;
        Vector3 startPos = transform.position, slamPos, horPos;
        float timer = 0, timer2 = 0, yPos, highestPoint = startPos.y + _leapHeight;
        
        if (_player.Grounded && Mathf.Abs(_player.transform.position.y - startPos.y) > 1f)
        {
            wallToDestroy = _spawnedWalls.Where(x => x.Broken).OrderBy(x => Vector3.Distance(_player.transform.position, x.transform.position)).First();
            slamPos = wallToDestroy.transform.position;
        }
        else
        {
            slamPos = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
        }

        while (timer < _leapDuration * 0.5f)
        {
            horPos = Vector3.Lerp(startPos, slamPos, timer / _leapDuration);

            yPos = Mathf.Lerp(startPos.y, highestPoint, timer / (_leapDuration * 0.5f));

            _rb.MovePosition(new Vector3(horPos.x, yPos, horPos.z));

            timer += Time.deltaTime;

            yield return null;
        }

        bool hit = false;

        while (timer < _leapDuration)
        {
            horPos = Vector3.Lerp(startPos, slamPos, timer / _leapDuration);

            yPos = Mathf.Lerp(highestPoint, startPos.y, timer2 / (_leapDuration * 0.5f));

            _rb.MovePosition(new Vector3(horPos.x, yPos, horPos.z));

            timer += Time.deltaTime;
            timer2 += Time.deltaTime;

            if (!hit)
            {
                if (Physics.CheckCapsule(transform.position, transform.position + new Vector3(0, 4.35f), 1, _playerLayer))
                {
                    _player.KnockBack((_player.transform.position - transform.position).MakeHorizontal(), _leapKnockback);
                    _player.TakeDamage(_leapDamage);

                    hit = true;
                }
            }

            yield return null;
        }

        if (wallToDestroy != null)
        {
            // destruir pared y spawnear shards
        }

        _rb.isKinematic = false;

        yield return new WaitForSeconds(_leapRecovery);

        _takingAction = false;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        Debug.Log(_hp);
        UIManager.instance.UpdateBar(UIManager.Bar.BossHp, _hp);
    }

    public override void Die()
    {
        UIManager.instance.ToggleBossBar(false);
        Destroy(gameObject);
    }
    public void ChangeAudio(AudioClip clip)
    {
        _myAS.clip = clip;
        _myAS.PlayOneShot(_myAS.clip);
    }
    public void prenderTornado(bool prendo)
    {
        tornadoPiedras.SetActive(prendo);
    }
    public void prenderCaidaPiedras(bool prendo)
    {
        caidaPiedras.SetActive(prendo);
    }
    public IEnumerator oneShotTiroPiedras()
    {
        yield return new WaitForSeconds(0.5f);
        prenderCaidaPiedras(false);
    }
}
