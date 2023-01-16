using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IStateManager
{
    public void FreezeObject(bool state);
}

public class EnemyBehaviourWithMovement : MonoBehaviour, IStateManager
{
    [SerializeField] private GameObject target;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _hp;
    [SerializeField] private int _value;
    public bool stopMovement;

    public delegate void enemyDeath(object sender, EventArgs e);
    public event Action<EnemyBehaviourWithMovement> onEnemyKilled;

    public int value() => _value;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Capitol");
        BattleSceneManager.Instance._playerStatistics.startListeningNewEnemy(this);
    }

    void Update()
    {
        if (!stopMovement)
            gameObject.transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x, transform.position.y), Time.deltaTime * _moveSpeed);
        // transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x, target.transform.position.y), moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.transform.position) <= 1.1f)
        {
            target.GetComponent<CapitolManager>().takeDamage(_damage);
            Destroy(gameObject);
        }
        if (gameObject.transform.position.y <= -20f)
        {
            Destroy(gameObject);
        }
    }

    public void takeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            onEnemyKilled?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void FreezeObject(bool state) => stopMovement = state;
}
