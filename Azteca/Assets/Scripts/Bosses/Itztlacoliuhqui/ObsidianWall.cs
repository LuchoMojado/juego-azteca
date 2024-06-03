using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianWall : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject _completeWall, _brokenWall;

    bool _broken = false;

    public void Break()
    {
        _completeWall.SetActive(false);
        _brokenWall.SetActive(true);

        _broken = true;
    }

    public void TakeDamage(float amount)
    {
        if (_broken)
        {
            Die();
        }
        else
        {
            Break();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

}
