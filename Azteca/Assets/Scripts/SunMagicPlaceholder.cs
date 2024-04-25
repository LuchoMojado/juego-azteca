using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMagicPlaceholder : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public Transform pos;

    Entity _target = null;

    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_target != null)
        {
            _target.TakeDamage(damage * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if ( pos != null)
        {
            _rb.MovePosition(pos.position);
            _rb.MoveRotation(pos.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");

        if (other.TryGetComponent(out ObsidianGod enemy))
        {
            Debug.Log("hit en enemigo");
            _target = enemy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ObsidianGod entity))
        {
            _target = null;
        }
    }
}
