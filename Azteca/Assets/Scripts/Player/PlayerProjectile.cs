using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [HideInInspector] public float damage, speed;

    [SerializeField] float _deathTimer;

    protected virtual void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (_deathTimer <= 0)
        {
            Die();
        }
        else
        {
            _deathTimer -= Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            return;
        }

        if (other.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(damage);
        }

        Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
