using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerProjectileEffect : MonoBehaviour
{
    private ParticleSystem thisParticle;
    private CircleCollider2D circleCollider;

    private int damage;
    

    public int Damage { set { damage = value; } }

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        thisParticle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if(!thisParticle.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        collision.GetComponent<EnemyHp>().Hit(damage);
        circleCollider.enabled = false;

    }
}
