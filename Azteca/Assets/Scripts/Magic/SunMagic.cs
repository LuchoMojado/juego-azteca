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
            transform.localScale += Vector3.one * Time.deltaTime * 0.2f;
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
        if (!_shot) return;
        
        if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            return;
        }

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

    public void Absorb(float absorbTime)
    {
        _chargingParticles.Stop();
        _charging = false;
        StartCoroutine(Shrink(absorbTime));
    }

    IEnumerator Shrink(float duration)
    {
        float timer = 0;

        var startingScale = transform.localScale;

        while (timer <= duration)
        {
            timer += Time.deltaTime;

            transform.localScale = Vector3.Lerp(startingScale, Vector3.zero, timer / duration);

            yield return null;
        }

        Die();
    }

    public IEnumerator Death()
    {
        _dead = true;
        _destroyedParticles.Play();
        GetComponentInChildren<Renderer>().enabled = false;

        yield return new WaitForSeconds(1);

        Die();
    }
}
