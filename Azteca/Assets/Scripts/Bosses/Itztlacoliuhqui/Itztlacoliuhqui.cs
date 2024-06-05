using IA2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itztlacoliuhqui : MonoBehaviour
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
        ArenaSpikes
    }

    EventFSM<Actions> _fsm;

    BehaviorNode _treeStart;

    Pathfinding _pf;
    List<Vector3> _path = new List<Vector3>();

    [Header("Search")]
    [SerializeField] float _searchSpeed;

    [Header("Spikes")]
    [SerializeField] Hazard _spikes;
    [SerializeField] ParticleSystem[] _preJumpParticles;
    [SerializeField] float _spikesPreparation, _spikesDuration, _spikesDamage, _spikesRecovery;

    [SerializeField] PlayerController _player;

    [SerializeField] ObsidianWall[] _spawnedWalls;

    [SerializeField] float _wallCloseRange, _wallBreakRange, _playerCloseRange;

    [SerializeField] LayerMask _wallLayer;

    ObsidianWall _wallBlockingLOS;

    Rigidbody _rb;

    Animator _anim;

    ObsidianPathfindManager _pfManager { get => ObsidianPathfindManager.instance; }

    bool _takingAction = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        #region FSM Setup

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
            .Done();

        StateConfigurer.Create(search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(spikes)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(swing)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(breakWall)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(shield)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(hide)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(wallSpike)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.Gatling, gatling)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
            .Done();

        StateConfigurer.Create(gatling)
            .SetTransition(Actions.Search, search)
            .SetTransition(Actions.Spikes, spikes)
            .SetTransition(Actions.Swing, swing)
            .SetTransition(Actions.BreakWall, breakWall)
            .SetTransition(Actions.Shield, shield)
            .SetTransition(Actions.Hide, hide)
            .SetTransition(Actions.WallSpike, wallSpike)
            .SetTransition(Actions.Charge, charge)
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
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
            .SetTransition(Actions.ArenaSpikes, arenaSpikes)
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
            .Done();

        search.OnEnter += x =>
        {
            _path = _pf.ThetaStar(_pfManager.FindNodeClosestToPos(transform.position), _pfManager.FindNodeClosestToPos(_player.transform.position), _wallLayer);
        };

        search.OnUpdate += () =>
        {
            if (transform.position.InLineOfSightOf(_player.transform.position, _wallLayer))
            {
                _treeStart.Execute();
            }
        };

        search.OnFixedUpdate += () =>
        {
            if (_path == null || _path.Count == 0) _treeStart.Execute();
            Vector3 posTarget = _path[0];
            posTarget.z = transform.position.z;
            Vector3 dir = posTarget - transform.position;
            if (dir.magnitude < 0.05f)
            {
                _rb.MovePosition(posTarget);
                _path.RemoveAt(0);
            }

            _rb.MoveRotation(Quaternion.LookRotation((_player.transform.position - transform.position).MakeHorizontal()));
            _rb.MovePosition(transform.position + transform.forward * _searchSpeed * Time.fixedDeltaTime);
        };

        spikes.OnEnter += x =>
        {
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

        #endregion

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

        return hit.collider.GetComponent<ObsidianWall>() == _wallBlockingLOS;
    }
    bool CanBreakWallBlockingLOS() => Vector3.Distance(transform.position, _wallBlockingLOS.transform.position) <= _wallBreakRange;
    bool IsPlayerClose() => Vector3.Distance(transform.position, _player.transform.position) <= _playerCloseRange;
    bool IsPlayerInLOS()
    {
        if (transform.position.InLineOfSightOf(_player.transform.position, _wallLayer, out var hit))
        {
            _wallBlockingLOS = hit.collider.GetComponent<ObsidianWall>();
            return true;
        }
        else
        {
            _wallBlockingLOS = null;
            return false;
        }
    }

    #endregion

    IEnumerator Spiking()
    {
        //LookAtPlayer = false;
        //ChangeAudio(dash);
        //preparacion
        yield return new WaitForSeconds(_spikesPreparation);
        //salen spikes del suelo
        //ChangeAudio(stomp);
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


}
