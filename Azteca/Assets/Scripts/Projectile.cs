using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage, speed;

    [SerializeField] float _deathTimer;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (_deathTimer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _deathTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 || other.gameObject.layer == 7)
        {
            return;
        }

        if (other.TryGetComponent(out PlayerController player))
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
