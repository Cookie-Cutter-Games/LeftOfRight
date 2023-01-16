using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    //ENEMY TRACKING
    [SerializeField] private GameObject _target = null;
    [SerializeField] private float _distance = 20f;
    //SHOTING
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _shootingCooldown;
    [SerializeField] private int _level;
    private float _timeSinceLastShoot = 0;
    public bool stopAttacking;
    public float distance() => _distance;

    void Update()
    {
        if (!stopAttacking && _level > 0)
            _timeSinceLastShoot += Time.deltaTime;
        if (_target == null)
        {
            findNewEnemy();
        }
        else
        {
            shootBullet();
        }
    }

    public void LevelUp()
    {
        _level += 1;
        _shootingCooldown -= 0.1f;
        changeDamage(2);
    }

    public void changeDamage(float damage, bool overwrite = false)
    {
        _bulletDamage = overwrite ? damage : _bulletDamage + damage;
    }


    public void findNewEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance && _distance <= distance)
            {
                nearestEnemy = enemy;
                nearestDistance = distance;
            }
        }
        _target = nearestEnemy;
    }

    public void shootBullet()
    {
        if (_timeSinceLastShoot > _shootingCooldown)
        {
            GameObject instantiate = Instantiate(_bulletPrefab, _shootPosition.position, _shootPosition.rotation);
            BulletMovmenet bs = instantiate.GetComponent<BulletMovmenet>();
            bs.bulletDamage = _bulletDamage;
            bs.targetPos = _target.transform;
            bs.withTracking = true;
            _timeSinceLastShoot = 0;
        }
    }
}
