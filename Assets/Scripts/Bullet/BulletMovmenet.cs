using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovmenet : MonoBehaviour, IStateManager
{
    //FOR TRACKING
    public Transform targetPos;
    public bool withTracking = false;
    private Vector3 direction;
    //DEFAULT
    public float bulletSpeed;
    [SerializeField] private float destroyTimeInSeconds;
    private Rigidbody2D rb;
    public float bulletDamage = 1;
    public bool stopMovement;

    void Start()
    {
        StartCoroutine(SelfDestruct());
        rb = this.GetComponent<Rigidbody2D>();
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(destroyTimeInSeconds);
        Destroy(gameObject);
    }

    void Update()
    {
        if (stopMovement)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            if (targetPos != null && withTracking)
            {
                direction = (targetPos.position - transform.position).normalized;
                transform.position += direction * bulletSpeed * Time.deltaTime;
            }
            else if (withTracking)
            {
                transform.position += direction * bulletSpeed * Time.deltaTime;
            }
            else
            {
                rb.velocity = new Vector2(bulletSpeed * (this.transform.rotation.y >= 0 ? 1 : -1), 0);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        gameObject.GetComponent<Animator>().SetTrigger("Hit");
        if (col.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            col.gameObject.GetComponent<EnemyBehaviourWithMovement>().takeDamage(bulletDamage);
        }
    }

    public void FreezeObject(bool state) => stopMovement = state;
}
