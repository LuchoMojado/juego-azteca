using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Entity
{
    [SerializeField] float _speed, _contactDamage, _bulletDamage, _shootCooldown, _switchActionTimer;

    [SerializeField] PlayerController _player;
    [SerializeField] Projectile _bullet;

    Rigidbody _rb;

    float _timer, _currentCD = 0;
    bool _chasing = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();

        UIManager.instance.UpdateBar(UIManager.Bar.BossHp, _hp, _maxHp);

        _timer = _switchActionTimer * 2;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer > _switchActionTimer)
        {
            _chasing = true;
        }
        else if (_timer > 0)
        {
            _chasing = false;
            Shoot();
        }
        else
        {
            _timer = _switchActionTimer * 2;
        }
    }

    private void FixedUpdate()
    {
        if (_chasing) 
        { 
            Chase();
        }
    }
    public void Chase()
    {
        var dir = GetPlayerDir();
        dir.MakeHorizontal();

        transform.forward = dir;
        _rb.MovePosition(transform.position + _speed * Time.fixedDeltaTime * dir);
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

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        print("taking damage: " + amount);
        UIManager.instance.UpdateBar(UIManager.Bar.BossHp, _hp);
    }

    public override void Die()
    {
        UIManager.instance.TurnOffBossBar();
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
