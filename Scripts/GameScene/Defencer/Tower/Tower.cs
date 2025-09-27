    using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum TowerType
{
    Basic,
    Power,
    Speed,
    Slow,
}

public enum TowerState
{
    Search,
    AttackCannon,
    AttackPower,
    AttackSpeed
}

public class Tower : MonoBehaviour
{
    [Header("<공통>")]
    [SerializeField] private TowerDatabase towerDatabase;
    [SerializeField] private TowerType towerType;

    [Header("[캐논]")]
    [SerializeField] private GameObject projectilePrefab;

    private int level;

    private Transform target;
    private EnemySpawner enemySpawner;

    private TowerState towerState;

    private SpriteRenderer spriteRenderer;
    private PlayerGold playerGold;

    private TowerBuilder towerBuilder;
    private float buffedDamage; // 버프를 받은 데미지
    private int buffLevel; // 버프 레벨


    public float Damage { get => towerDatabase.TowerDatas[level].damage; set { Damage = (int)value; } }
    public float Speed { get => towerDatabase.TowerDatas[level].speed; set { Speed = (int)value; } }
    public float Slow => towerDatabase.TowerDatas[level].slow;
    public int Level => level + 1;
    public int MaxLevel => towerDatabase.TowerDatas.Length;
    public TowerType TowerType => towerType;
    public Sprite TowerSprite => towerDatabase.TowerDatas[level].sprite;
    public float BuffedDamage { set => buffedDamage = Mathf.Max(0, value); get => buffedDamage; }
    public int UpgradePrice => Level < MaxLevel ? towerDatabase.TowerDatas[level + 1].price : 0;
    public int SellPrice => towerDatabase.TowerDatas[level].price / 2;
    public int Index { get; set; }

    public void Setup(EnemySpawner spawner, PlayerGold gold, TowerBuilder builder)
    {
        towerBuilder = builder;
        playerGold = gold;
        enemySpawner = spawner;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        ChangeState(TowerState.Search);
        towerDatabase.TowerDatas[level].damage = towerDatabase.TowerDatas[level].originDamage;
        if (towerType == TowerType.Power && GameManager.Instance.isRain && !GameManager.Instance.isSnow) // 비올때 불타워
        {
            towerDatabase.TowerDatas[level].damage = (int)towerBuilder.BuffList.SetDamage(Damage, 0.9f);
            return;
        }
        if (towerType == TowerType.Power && !GameManager.Instance.isRain && !GameManager.Instance.isSnow) // 안올때 불타워
        {
            towerDatabase.TowerDatas[level].damage = (int)towerBuilder.BuffList.SetDamage(Damage, 1.2f);
            return;
        }
        if (towerType == TowerType.Power && !GameManager.Instance.isRain && GameManager.Instance.isSnow) // 눈올때 불타워
        {
            towerDatabase.TowerDatas[level].damage = (int)towerBuilder.BuffList.SetDamage(Damage, 0.8f);
            return;
        }
        if (towerType == TowerType.Power && GameManager.Instance.isRain && GameManager.Instance.isSnow) // 다올때 불타워
        {
            towerDatabase.TowerDatas[level].damage = (int)towerBuilder.BuffList.SetDamage(Damage, 0.6f);
            return;
        }
        if (towerType == TowerType.Speed && !GameManager.Instance.isRain && !GameManager.Instance.isSnow) // 안올때 바람타워
        {
            return;
        }
        if (towerType == TowerType.Speed && GameManager.Instance.isRain && !GameManager.Instance.isSnow) // 비올때 바람타워
        {
            towerDatabase.TowerDatas[level].speed = towerBuilder.BuffList.SetDamage(Speed, 1.2f);
            return;
        }
        if (towerType == TowerType.Speed && !GameManager.Instance.isRain && GameManager.Instance.isSnow) // 눈올때 바람타워
        {
            towerDatabase.TowerDatas[level].damage = (int)towerBuilder.BuffList.SetDamage(Damage, 1.2f);
            return;
        }
        if (towerType == TowerType.Speed && GameManager.Instance.isRain && GameManager.Instance.isSnow) // 다올때 바람타워
        {
            towerDatabase.TowerDatas[level].speed = towerBuilder.BuffList.SetDamage(Speed, 1.2f);
            towerDatabase.TowerDatas[level].damage = (int)towerBuilder.BuffList.SetDamage(Damage, 1.2f);
            return;
        }

        
    }
    public bool TryUpgrade()
    {
        // 다음 타워 레벨업에 필요한 골드보다 적다면 리턴 false;
        if (playerGold.CurrentGold < towerDatabase.TowerDatas[level + 1].price) return false;


        Upgrade();
        return true;

    }
    
    private void Upgrade()
    {
        level++;

        spriteRenderer.sprite = towerDatabase.TowerDatas[level].sprite;

        playerGold.CurrentGold -= towerDatabase.TowerDatas[level].price;

        UpdateUI.Instance.UpdateLandownerGold();
    }

    // 상태 전환 함수
    private void ChangeState(TowerState state)
    {
        // 이미 동작 중인 상태 정지 
        StopCoroutine(towerState.ToString());

        // 새로운 상태로 전환 및 새로운 상태 시작
        towerState = state;
        StartCoroutine(towerState.ToString()); // enum변수를 문자열로 전환
    }

    // 적이 다량 존재 -> 에너미 리스트
    // 
    private Transform FindTarget()
    {
        if (enemySpawner.EnemyList.Count == 0)
        {
            return null;
        }
        target = enemySpawner.EnemyList[0].transform;
        return target;
    }

    // 탐색 상태 => 타겟을 지정
    private IEnumerator Search()
    {
        while (true)
        {
            //float nearest = Mathf.Infinity;
            target = FindTarget();
            if (target != null)
            {
                if (towerType == TowerType.Basic) ChangeState(TowerState.AttackCannon);
                else if (towerType == TowerType.Power) ChangeState(TowerState.AttackCannon);
                else if (towerType == TowerType.Speed) ChangeState(TowerState.AttackCannon);
            }
            yield return null;
        }
    }

    private bool ReadyToAttack()
    {
        // 공격 상태를 빠져나오는 조건
        // 서치 상태로 전환되는 조건
        // 1. 적이 죽었을 때
        if (target == null)
        {
            return false;
        }
        return true;
    }


    private IEnumerator AttackCannon()
    {
        while (true)
        {
            if (!ReadyToAttack())
            {
                ChangeState(TowerState.Search);
                break;
            }

            Fire();
            
            yield return new WaitForSeconds(towerDatabase.TowerDatas[level].speed);

            // 공격
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().Setup(target, towerDatabase.TowerDatas[level].damage, towerType);
    }
}
