using UnityEngine;
using System;

public class CapitolManager : MonoBehaviour
{

    [SerializeField] private float _hp = 100;

    public delegate void capitolDeath(object sender, EventArgs e);
    public event capitolDeath capitolDeathEvent;
    public void takeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            capitolDeathEvent?.Invoke(this, new EventArgs());
            Destroy(gameObject);
        }
    }
}
