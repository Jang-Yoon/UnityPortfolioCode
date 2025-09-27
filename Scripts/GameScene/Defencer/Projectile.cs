using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject powerProjectileEffect;

    private Movement movement;
    private Transform target;


    private int attackDamage;
    private bool isPower;


    public void Setup(Transform enemy, int damage, TowerType type)
    {
        if (type == TowerType.Power) isPower = true;
        movement = GetComponent<Movement>();
        target = enemy;
        attackDamage = damage;
    }

    private void Update()
    {
        if (target != null)
        {
            // 무브먼트에 방향 지정.
            Vector3 direction = (target.position - transform.position).normalized;
            movement.MoveDirection = direction;
        }
        else
        {
            // 총알 제거
            Destroy(gameObject);
        }
    }

    // 총알이 트리거 충돌했을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 반드시 타겟에만 충돌하도록하는 코드
        if (!collision.CompareTag("Enemy") || collision.transform != target) return;

        // 기획 요소로 사용할 수 있는 코드

        if (isPower)
        {
            GameObject temp = Instantiate(powerProjectileEffect, collision.transform.position, Quaternion.identity);
            temp.GetComponent<PowerProjectileEffect>().Damage = attackDamage;
            Destroy(gameObject);
            return;
        }
        collision.GetComponent<EnemyHp>().Hit(attackDamage);
        Destroy(gameObject);
    }
}
