using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 죽음 타입 정의
public enum DeathType
{
    Die,    
    Finish  
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private int gold = 10;
    [SerializeField] private int price;

    private int waypointCount;      // 웨이포인트 개수
    private Transform[] waypoints;  // 웨이포인트 트랜스폼 배열
    private int currentIndex;       // 웨이포인트 인덱스
    private Movement movement;
    private EnemySpawner enemySpawner;
    public float CurrentDistance { get; set; }
    private float maxDistance;
    public int Price => price;

    // 적 탈출 개선 코드
    private float timer;

    public void Setup(Transform[] way, EnemySpawner spawner)
    {
        // Enemy, Movement스크립트가 동일한 게임오브젝트에 할당되어야 한다.
        movement = GetComponent<Movement>();

        enemySpawner = spawner;

        // 적 이동 경로 설정
        waypointCount = way.Length; // 웨이포인트의 개수는 배열의 크기
        waypoints = new Transform[waypointCount]; // 웨이포인트 개수만큼 배열 초기화
        waypoints = way; // 매개변수로 전달받은 배열을 웨이포인트 배열에 할당

        for (int i = 0; i < waypoints.Length-1; i++)
        {
            maxDistance += Vector2.Distance(waypoints[i].position, waypoints[i + 1].position);
            //Debug.Log(maxDistance);
        }

        // 적의 최초 위치를 첫 번째 웨이포인트(스타트 타일)로 설정
        transform.position = waypoints[currentIndex].position;
        // 이동 명령
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        // 이동 방향 설정
        SetDestination();
            //Debug.Log(maxDistance);
        // 무한 반복
        while (true)
        {
            CurrentDistance = Vector2.Distance(transform.position, waypoints[currentIndex-1].position);
            //Debug.Log(CurrentDistance);
            timer -= Time.deltaTime;
            // 도착지점까지 갔다면 시간은 0이하가 된다.
            if (timer <= 0)
            {
                //다음 이동 방향 설정
                SetDestination();
            }
            yield return null; // 한 프레임 쉰다. while문 안에 yield return 없으면 유니티 정지(무한)
        }
    }

    // 방향 설정
    private void SetDestination()
    {
        // 아직 이동할 웨이포인트가 남아있다면
        // index는 0부터 시작
        if (currentIndex < waypointCount - 1)
        {
            // 제대로 된 위치로 고정
            transform.position = waypoints[currentIndex].position;

            // 다음 웨이포인트로 향하는 방향을 설정
            currentIndex++;
            CalculateTimer();

            // 방향 구하기(기하 벡터) ★★★★★
            // 구간 별 벡터의 크기가 다르기때문에 구간마다 다른 속도로 이동할 것이다.

            // 목표지점으로 향하는 방향벡터 구하는 공식
            // 다음 벡터 - 현재 벡터 : 다음 벡터로 향하는 벡터
            Vector3 direction = (waypoints[currentIndex].position - transform.position).normalized;

            // 벡터는 크기가 서로 다르기 때문에 정규화해야한다.
            // 정규화 : 크기가 1인 벡터로 만듦. (방향만을 가지도록 한다.)
            // 벡터.normalized;

            // 방향 적용
            movement.MoveDirection = direction;
        }
        else // 마지막 위치라면(finish타일)
        {
            Die(DeathType.Finish);
        }
    }

    // 감속이나 원복될 때마다 타이머를 다시 계산
    public void CalculateTimer()
    {
        // 속력 구하는 공식(속력 = 거리/시간) 이용
        float distance = Vector2.Distance(transform.position, waypoints[currentIndex].position);

        // 시간을 구한다. (시간 = 거리 / 속력)
        timer = distance / movement.MoveSpeed;
    }

    public void Die(DeathType type)
    {
        enemySpawner.DestroyEnemy(this, type, gold);

    }

    // 벡터 : 벡터는 [방향]과 [크기]를 동시에 지닌다.
}
