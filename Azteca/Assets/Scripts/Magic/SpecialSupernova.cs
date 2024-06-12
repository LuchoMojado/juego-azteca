using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSupernova : SpecialMagic
{
    GameObject _supernova;
    float _radius, _damage, _preparation, _recovery, _cooldown;

    public SpecialSupernova(PlayerController player, Inputs inputs, GameObject supernova, float radius, float damage, float preparation, float recovery, float cooldown)
    {
        _player = player;
        _inputs = inputs;
        _supernova = supernova;
        _radius = radius;
        _damage = damage;
        _preparation = preparation;
        _recovery = recovery;
        _cooldown = cooldown;
    }

    public override float Activate()
    {
        _player.StartCoroutine(Supernova());

        return _cooldown;
    }

    IEnumerator Supernova()
    {
        _inputs.inputUpdate = _inputs.FixedCast;
        _player.anim.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(_preparation);

        var nova = Object.Instantiate(_supernova, _player.transform.position, Quaternion.identity);
        nova.transform.localScale *= _radius;

        var cols = Physics.OverlapSphere(_player.transform.position, _radius);

        foreach (var item in cols)
        {
            if (item.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_damage);
            }
        }

        yield return new WaitForSeconds(_recovery);

        Object.Destroy(nova);

        _player.anim.SetBool("IsAttacking", false);
        _inputs.inputUpdate = _inputs.Unpaused;
    }
}
