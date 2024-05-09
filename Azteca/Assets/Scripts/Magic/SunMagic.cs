using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMagic : PlayerProjectile
{
    [SerializeField] ParticleSystem _chargingParticles, _readyParticles, _destroyedParticles;

    bool _charging = true, _shot = false;

    protected override void Update()
    {
        if (_charging)
        {
            transform.localScale += Vector3.one * Time.deltaTime * 0.15f;
        }
        else if (_shot)
        {
            base.Update();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (_charging) return;

        base.OnTriggerEnter(other);
    }

    public void ChargeFinished()
    {
        _chargingParticles.Stop();
        _readyParticles.Play();
        _charging = false;
    }

    public void Shoot(float dmg)
    {
        _chargingParticles.Stop();
        _charging = false;
        _shot = true;
        damage = dmg;
    }

    public override void Die()
    {
        _destroyedParticles.Play();

        //apagar renderer? esperar a que terminen las particulas de destruir (con corrutina?)

        base.Die();
    }
}
