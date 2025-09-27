using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    //[SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyGroup;
    [SerializeField,Header("몬스터 스폰 간격")] private float spawnInterval = 1;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Button buttonSpawn;
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<GameObject> EnemyPrefabs { get => enemyPrefabs; set { enemyPrefabs = value; } }
    // 적 관리 리스트
    private List<Enemy> enemyList = new List<Enemy>();
    public List<Enemy> EnemyList => enemyList;
    private int count;
    private SpawnEnemyList spawnEnemyList;
    private LandHp landHp;
    private PlayerGold playerGold;
    private StageSystem stage;
    public float SpawnInterval { get => spawnInterval; set { spawnInterval = value; } }

    public Button ButtonSpawn { get => buttonSpawn; set { buttonSpawn = value; } }
    public int EnemyCount { get; set; }

    private void Start()
    {
        GameManager.Instance.IsMonstAlive = false;
        stage = GetComponent<StageSystem>();
        buttonSpawn.interactable = false;
        playerGold = GetComponent<PlayerGold>();
        landHp = GetComponent<LandHp>();
        spawnEnemyList = GetComponent<SpawnEnemyList>();
        buttonSpawn.onClick.AddListener(() => photonView.RPC("SpawnEnemy", RpcTarget.All));
    }
    [PunRPC]
    private IEnumerator SpawnEnemy()
    {
        buttonSpawn.interactable = false;
        GameManager.Instance.IsSpawn = true;
        GameManager.Instance.IsMonstAlive = true;
        stage.ButtonSkipTurn.interactable = false;
        stage.IsStartStage = true;
        Color color = Color.white;
        color.a = 0;
        while (count<enemyPrefabs.Count)
        {
            GameObject temp = Instantiate(enemyPrefabs[count], enemyGroup);
            Enemy enemy = temp.GetComponent<Enemy>();

            enemy.Setup(waypoints,this);
            enemyList.Add(enemy);
            spawnEnemyList.EnemyImages[count].color = color;
            count++;

            spawnEnemyList.EnemyCount--;
            spawnEnemyList.UpdateMonsterCount();
            yield return new WaitForSeconds(spawnInterval);
        }
        enemyPrefabs.Clear();
        count = 0;
        GameManager.Instance.IsSpawn = false;
    }
    // 적 제거
    public void DestroyEnemy(Enemy enemy, DeathType type, int gold)
    {
        if (type == DeathType.Finish)
        {
            landHp.Hit(1);
            UpdateUI.Instance.UpdatePlayerHp();
        }
        else if (type == DeathType.Die)
        {
            playerGold.CurrentGold += gold;
            UpdateUI.Instance.UpdateLandownerGold();
        }
        enemyList.Remove(enemy);
        EnemyCount--;
        //UpdateUI.Instance.UpdateEnemyCount();
        spawnEnemyList.MonsterCount--;
        if (spawnEnemyList.MonsterCount == 0)
        {
            stage.IsStartStage = false;
            stage.StartStage();
            GameManager.Instance.IsMonstAlive = false;
        }
        Destroy(enemy.gameObject);

        if(enemyList.Count == 0) stage.ButtonSkipTurn.interactable = true;

        // 아래 코드 사용했을 경우 타워의 타겟이 사라지지 않아서 계속 공격 상태임.
        //enemy.gameObject.SetActive(false);
    }
}