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
        Debug.Log("hit");

        if (other.TryGetComponent(out TestEnemy enemy))
        {
            Debug.Log("hit en enemigo");
            _target = enemy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out TestEnemy entity))
        {
            _target = null;
        }
    }
}
