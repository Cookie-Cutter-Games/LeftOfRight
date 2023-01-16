using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerShootBullets : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _shootingCooldown;
    [SerializeField] public Boolean isShooting;
    private Animator anim;
    private float _timeSinceLastShoot = 0;

    public void changeMovementSpeed(object sender, EventArgs e)
    {
        _shootingCooldown = gameObject.GetComponent<PlayerStatistics>().shootingCooldown();
    }
    public void changeBulletDamage(object sender, EventArgs e)
    {
        _bulletDamage = gameObject.GetComponent<PlayerStatistics>().damage();
    }

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        gameObject.TryGetComponent<PlayerStatistics>(out PlayerStatistics stats);
        _shootingCooldown = stats.shootingCooldown();
        _bulletDamage = stats.damage();
        stats.playerChangedShootingCooldownEvent += changeMovementSpeed;
        stats.playerChangedDamageEvent += changeBulletDamage;
    }

    void Update()
    {
        _timeSinceLastShoot += Time.deltaTime;
    }



    public void shootBullet()
    {
        if (_timeSinceLastShoot > _shootingCooldown)
        {
            isShooting = true;
            _timeSinceLastShoot = 0;
            anim.SetTrigger("Attack");

        }
    }

    public void trueShootBullet()
    {
        _timeSinceLastShoot = 0;
        GameObject instantiate = Instantiate(_bulletPrefab, _shootPosition.position, _shootPosition.rotation);
        BulletMovmenet bs = instantiate.GetComponent<BulletMovmenet>();
        bs.bulletDamage = _bulletDamage;
        bs.withTracking = false;
        isShooting = false;

    }
}
