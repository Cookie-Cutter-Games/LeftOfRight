using UnityEngine;
using System;

public class PlayerStatistics : MonoBehaviour
{
    [SerializeField] private float _maxMovementSpeed;
    [SerializeField] private float _maxDamage;
    [SerializeField] private float maxShootingCooldown;
    [SerializeField] private float _movementSpeed = 1;
    [SerializeField] private float _damage = 3;
    [SerializeField] private float _shootingCooldown = 1;
    [SerializeField] private int _money = 100;


    public delegate void playerChangedMovementSpeed(object sender, EventArgs e);
    public event playerChangedMovementSpeed playerChangedMovementSpeedEvent;
    public delegate void playerChangedShootingCooldown(object sender, EventArgs e);
    public event playerChangedShootingCooldown playerChangedShootingCooldownEvent;
    public delegate void playerChangedDamage(object sender, EventArgs e);
    public event playerChangedDamage playerChangedDamageEvent;

    public int money() => _money;
    public float shootingCooldown() => _shootingCooldown;
    public float movementSpeed() => _movementSpeed;
    public float damage() => _damage;


    public void startListeningNewEnemy(EnemyBehaviourWithMovement enemy)
    {
        enemy.onEnemyKilled += (e) => enemyHasBeenKilled(e.value());

    }

    public void changeMovementSpeed(float speed, bool overwrite = false)
    {
        _movementSpeed = overwrite ? speed : _movementSpeed += speed;
        playerChangedMovementSpeedEvent(this, new EventArgs());
    }

    /// <summary>
    /// this function append time !
    /// </summary>
    public void changeShootingCooldown(float cooldown, bool overwrite = false)
    {
        _shootingCooldown = overwrite ? cooldown : _shootingCooldown + cooldown;
        playerChangedShootingCooldownEvent(this, new EventArgs());
    }

    public void enemyHasBeenKilled(int enemyValue)
    {
        BattleSceneManager.Instance.enemyStatistics.locallyKilledEnemies++;
        BattleSceneManager.Instance.enemyStatistics.globallyKilledEnemies++;
        _money += enemyValue;
    }

    public void changeDamage(float damage, bool overwrite = false)
    {
        _damage = overwrite ? damage : _damage + damage;
        playerChangedDamageEvent?.Invoke(this, new EventArgs());
    }

    public void decreaseMoney(int amount)
    {
        _money -= amount;
    }


}
