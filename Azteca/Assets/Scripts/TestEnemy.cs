using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Entity
{
    [SerializeField] float _speed, _contactDamage, _bulletDamage, _shootCooldown, _switchActionTimer;

    [SerializeField] PlayerController _player;
    [SerializeField] Projectile _bullet;

    float _timer, _currentCD = 0;

    protected override void Start()
    {
        base.Start();

        _timer = _switchActionTimer * 2;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer > _switchActionTimer)
        {
            Chase();
        }
        else if (_timer > 0)
        {
            Shoot();
        }
        else
        {
            _timer = _switchActionTimer * 2;
        }
    }

    public void Chase()
    {
        var dir = GetPlayerDir();
        dir.MakeHorizontal();

        transform.forward = dir;
        transform.position += dir * _speed * Time.deltaTime;
    }

    public void Shoot()
    {
        var dir = GetPlayerDir();

        if (_currentCD <= 0)
        {
            var bullet = Instantiate(_bullet, transform.position, Quaternion.identity);
            bullet.moveDirection = dir;
            bullet.damage = _bulletDamage;

            _currentCD = _shootCooldown;
        }
        else
        {
            _currentCD -= Time.deltaTime;
        }

        dir.MakeHorizontal();
        transform.forward = dir;
    }

    Vector3 GetPlayerDir()
    {
        var dir = _player.transform.position - transform.position;

        return dir.normalized;
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Entity entity))
        {
            entity.TakeDamage(_contactDamage);
        }
    }
}
