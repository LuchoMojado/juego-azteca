using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float _maxHp;

    protected float _hp;

    protected virtual void Start()
    {
        _hp = _maxHp;
    }

    public virtual void TakeDamage(float amount)
    {
        _hp -= amount;

        if (_hp <= 0)
        {
            Die();
        }
    }

    public abstract void Die();
}
