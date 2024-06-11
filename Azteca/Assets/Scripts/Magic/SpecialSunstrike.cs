using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSunstrike : SpecialMagic
{
    float _damage, _preparation, _delay, _recovery;

    public SpecialSunstrike(PlayerController player, Inputs inputs, float cost, float damage, float preparation, float delay, float recovery)
    {
        _player = player;
        _inputs = inputs;
        staminaCost = cost;
        _damage = damage;
        _preparation = preparation;
        _delay = delay;
        _recovery = recovery;
    }

    public override void Activate()
    {
        if (_player.currentBoss != null) _player.StartCoroutine(Sunstriking());
    }

    IEnumerator Sunstriking()
    {
        _inputs.inputUpdate = _inputs.FixedCast;
        //animacion

        yield return new WaitForSeconds(_preparation);

        //_player.currentBoss
    }
}
