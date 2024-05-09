using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMagic : PlayerProjectile
{
    [SerializeField] ParticleSystem _chargingParticles, _readyParticles, _destroyedParticles;

    [HideInInspector] public PlayerController player;

    bool _charging = true, _shot = false, _dead = false;

    protected override void Update()
    {
        if (_charging)
        {
            transform.localScale += Vector3.one * Time.deltaTime * 0.25f;
        }
        else if (_shot && !_dead)
        {
            transform.position += transform.forward * speed * Time.deltaTime;

            if (_deathTimer <= 0)
            {
                StartCoroutine(Death());
            }
            else
            {
                _deathTimer -= Time.deltaTime;
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            return;
        }

        if (_charging) player.StopCast();

        if (other.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(damage);
        }

        StartCoroutine(Death());
    }

    public void ChargeFinished()
    {
        _chargingParticles.Stop();
        _readyParticles.Play();
        _charging = false;
    }

    public void Shoot()
    {
        _chargingParticles.Stop();
        _charging = false;
        _shot = true;
    }

    protected IEnumerator Death()
    {
        _dead = true;
        _destroyedParticles.Play();
        GetComponentInChildren<Renderer>().enabled = false;

        yield return new WaitForSeconds(1);

        Die();
    }
}
