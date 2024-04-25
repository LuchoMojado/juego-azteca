using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianGod : Entity
{
    [SerializeField] PlayerController _player;

    Rigidbody _rb;

    public GameObject placeholderSwingHitboxFeedback;

    [SerializeField] LayerMask playerMask;

    [Header("Swing")]
    [SerializeField] Transform _meleeHitboxCenter;
    [SerializeField] float _swingForwardForce, _swingPreparation, _swingDamage, _swingDuration, _swingRecovery, _meleeBoxX, _meleeBoxY, _meleeBoxZ;
    Vector3 _meleeBox;

    [Header("Shards")]
    [SerializeField] Projectile _shard;
    [SerializeField] int _shardAmount;
    [SerializeField] float _shardsPreparation, _shardAngle, _shardSpeed, _shardDamage, _shardsInterval, _shardsRecovery;

    bool _takingAction = false;

    float _timer = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _meleeBox = new Vector3(_meleeBoxX, _meleeBoxY, _meleeBoxZ);
    }

    private void Update()
    {
        if (!_takingAction)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                Shards(3);
            }
        }
    }

    void Shards(int waves)
    {
        _takingAction = true;

        StartCoroutine(ThrowingShards(_shardAmount + waves));
    }

    void Spikes()
    {

    }

    void Wave()
    {

    }

    void Leap()
    {

    }

    void Swing()
    {
        _takingAction = true;
        
        StartCoroutine(Swinging());
    }

    IEnumerator Swinging()
    {
        yield return new WaitForSeconds(_swingPreparation);

        _rb.AddForce(transform.forward * _swingForwardForce);

        placeholderSwingHitboxFeedback.SetActive(true);

        bool hit = false;
        float _timer = 0;

        while (_swingDuration > _timer)
        {
            _timer += Time.deltaTime;

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

        _takingAction = false;
        _timer = 1;
    }

    IEnumerator ThrowingShards(int shardLimit)
    {
        yield return new WaitForSeconds(_shardsPreparation);

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

        _takingAction = false;
        _timer = 1;
    }

    /*bool IsPlayerClose()
    {
        return GetPlayerDistance() < _closeDistance;
    }

    bool IsPlayerMidRange()
    {
        return GetPlayerDistance() < _midDistance;
    }*/

    float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_meleeHitboxCenter.position, _meleeBox * 2);
    }
}
