using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianGod : Entity
{
    [SerializeField] PlayerController _player;

    Rigidbody _rb;

    public GameObject placeholderSwingHitboxFeedback;

    [SerializeField] float _swingForwardForce, _swingHitboxDelay, _swingDamage, _swingDuration, _swingRecovery;
    [SerializeField] LayerMask playerMask;
    [SerializeField] Transform _meleeHitboxCenter;

    [SerializeField] float _meleeBoxX, _meleeBoxY, _meleeBoxZ;
    Vector3 _meleeBox;

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
                Swing();
            }
        }
    }

    void Shards()
    {

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
        _rb.AddForce(transform.forward * _swingForwardForce);
        StartCoroutine(Swinging());
    }

    IEnumerator Swinging()
    {
        yield return new WaitForSeconds(_swingHitboxDelay);

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
