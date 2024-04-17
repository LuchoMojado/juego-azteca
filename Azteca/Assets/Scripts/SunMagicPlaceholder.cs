using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMagicPlaceholder : MonoBehaviour
{
    [HideInInspector] public float damage;

    Entity _target = null;

    void Update()
    {
        if (_target != null)
        {
            _target.TakeDamage(damage * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            if (entity.TryGetComponent(out PlayerController player))
            {
                return;
            }

            _target = entity;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            if (entity.TryGetComponent(out PlayerController player))
            {
                return;
            }

            _target = null;
        }
    }
}
