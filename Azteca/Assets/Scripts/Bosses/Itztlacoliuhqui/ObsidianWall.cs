using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianWall : MonoBehaviour, IDamageable
{
    public Itztlacoliuhqui boss;

    [SerializeField] GameObject _completeWall, _brokenWall;

    [SerializeField] float _radius, _riseTime;

    public float Radius
    {
        get
        {
            return _radius;
        }
    }

    List<Node> _overlappingNodes = new List<Node>();

    bool _broken = false, _rising = true;

    public bool Broken
    {
        get
        {
            return _broken;
        }
    }

    private void Start()
    {
        StartCoroutine(Rise());

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
        if (_rising)
        {
            Die();
        }

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

        boss.WallDestroyed(this);

        Destroy(gameObject);
    }

    IEnumerator Rise()
    {
        var unburiedPos = _completeWall.transform.localPosition;
        var buriedPos = new Vector3(_completeWall.transform.localPosition.x, -_completeWall.transform.localPosition.y, _completeWall.transform.localPosition.z);

        _completeWall.transform.position = buriedPos;

        float timer = 0;

        while (timer < _riseTime)
        {
            _completeWall.transform.localPosition = Vector3.Lerp(buriedPos, unburiedPos, timer / _riseTime);

            timer += Time.deltaTime;

            yield return null;
        }

        _completeWall.transform.localPosition = unburiedPos;

        _rising = false;
    }
}