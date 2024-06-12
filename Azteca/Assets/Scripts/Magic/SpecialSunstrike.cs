using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSunstrike : SpecialMagic
{
    GameObject _firstRay, _secondRay;
    float _damage, _radius, _preparation, _delay, _linger, _cooldown;

    public SpecialSunstrike(PlayerController player, Inputs inputs, GameObject firstRay, GameObject secondRay, float cost, float damage, float radius, float preparation, float delay, float linger, float cooldown)
    {
        _firstRay = firstRay;
        _secondRay = secondRay;
        _player = player;
        _inputs = inputs;
        staminaCost = cost;
        _damage = damage;
        _radius = radius;
        _preparation = preparation;
        _delay = delay;
        _linger = linger;
        _cooldown = cooldown;
    }

    public override float Activate()
    {
        if (_player.currentBoss != null)
        {
            _player.StartCoroutine(Sunstriking());
            return _cooldown;
        }
        else return 0;
    }

    IEnumerator Sunstriking()
    {
        var boss = _player.currentBoss;

        _inputs.inputUpdate = _inputs.FixedCast;
        _player.transform.forward = _player.transform.position - boss.transform.position;
        _player.anim.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(_preparation);

        _player.anim.SetBool("IsAttacking", false);
        _inputs.inputUpdate = _inputs.Unpaused;

        if (!boss) yield break;

        Vector3 strikePos = boss.transform.position;
        var ray1 = Object.Instantiate(_firstRay, strikePos, Quaternion.identity);

        yield return new WaitForSeconds(_delay);

        Object.Destroy(ray1);
        var ray2 = Object.Instantiate(_secondRay, strikePos, Quaternion.identity);
        ray2.transform.localScale = new Vector3(_radius, ray2.transform.localScale.y, _radius);

        if (Physics.CheckCapsule(strikePos, strikePos + Vector3.up * 5, _radius, boss.gameObject.layer))
        {
            boss.TakeDamage(_damage);
        }

        yield return new WaitForSeconds(_linger);

        //destruir segundo rayo
    }
}
