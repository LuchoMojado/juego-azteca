using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianWall : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject _completeWall, _brokenWall;

    [SerializeField] float _radius;

    List<Node> _overlappingNodes;

    bool _broken = false;

    private void Start()
    {
        foreach (var item in ObsidianPathfindManager.instance.allNodes)
        {
            if (Vector3.Distance(transform.position, item.transform.position) <= _radius)
            {
                _overlappingNodes.Add(item);
                item.SetBlock(true);
            }
        }
    }

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
        foreach (var item in _overlappingNodes)
        {
            item.SetBlock(false);
        }

        Destroy(gameObject);
    }

}
