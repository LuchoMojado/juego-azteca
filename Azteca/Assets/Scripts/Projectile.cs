using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public float damage;

    [SerializeField] float _speed, _deathTimer;

    void Update()
    {
        transform.position += moveDirection * _speed * Time.deltaTime;

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
        if (other.TryGetComponent(out PlayerController player))
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
