using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using TMPro;

public class SpawnEnemyList : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("어떤 몬스터가 소환될지 보여줄 이미지들")] private List<Image> enemyImages = new List<Image>();    // 어떤 몬스터가 소환될지 보여줄 이미지들
    [SerializeField, Header("몬스터 버튼들")] private List<Button> buttonEnemys = new List<Button>(); // 몬스터버튼
    [SerializeField, Header("이미지에 들어갈 스프라이트들")] private List<Sprite> enemySprite = new List<Sprite>();  // 어떤 몬스터가 소환될지 보여줄 이미지에 들어갈 스프라이트
    [SerializeField, Header("몬스터 프리펩")] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField, Header("소환 될 몬스터의 수")] private TextMeshProUGUI tmpMonsterCount;
    [SerializeField] private TextMeshProUGUI tmpNotEnoughMana;
    [SerializeField]private int maxMonsterCount;  
    private int enemyCount;                                 // 몬스터 소환 갯수(최대 18마리)
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;
    private StageSystem stage;
    public int EnemyCount { get => enemyCount; set { enemyCount = value; } }

    public int MonsterCount { get; set; }
    public List<Image> EnemyImages { get => enemyImages; set { enemyImages = value; } }
    public TextMeshProUGUI TmpNotEnoughMana => tmpNotEnoughMana;
    public List<GameObject> EnemyPrefabs => enemyPrefabs;


    private void Start()
    {
        stage = GetComponent<StageSystem>();
        playerGold = GetComponent<PlayerGold>();
        enemySpawner = GetComponent<EnemySpawner>();
        buttonEnemys[0].onClick.AddListener(() => photonView.RPC("AddEnemyList", RpcTarget.All, enemySprite[0].name, enemyPrefabs[0].name));
        buttonEnemys[1].onClick.AddListener(() => photonView.RPC("AddEnemyList", RpcTarget.All, enemySprite[1].name, enemyPrefabs[1].name));
        buttonEnemys[2].onClick.AddListener(() => photonView.RPC("AddEnemyList", RpcTarget.All, enemySprite[2].name, enemyPrefabs[2].name));
    }
    [PunRPC]
    private void AddEnemyList(string enemy, string prefab)
    {
        Sprite spriteEnemy = Resources.Load<Sprite>("Sprites/" + enemy);
        GameObject enemyPrefab = Resources.Load<GameObject>(prefab);
        if (playerGold.CurrentMana < enemyPrefab.GetComponent<Enemy>().Price)
        {
            StartCoroutine(ManaAnimation(tmpNotEnoughMana));
            return;
        }
        if (GameManager.Instance.IsSpawn)
        {
            Debug.Log("몬스터가 소환중임 소환할 수 없음");
            return;
        }
        if (enemyCount == 18)
        {
            Debug.Log("더이상 몬스터를 소환할 수 없음.");
            return;
        }
        if (!GameManager.Instance.IsMonstAlive) enemySpawner.ButtonSpawn.interactable = true;
        else return;
        stage.ButtonSkipTurn.interactable = false;
        playerGold.CurrentMana -= enemyPrefab.GetComponent<Enemy>().Price;
        UpdateUI.Instance.UpdateLuciferMana();
        Color color = enemyImages[enemyCount].color;
        color.a = 1;
        enemyImages[enemyCount].color = color;
        enemyImages[enemyCount].sprite = spriteEnemy;
        enemySpawner.EnemyPrefabs.Add(enemyPrefab);
        enemyCount++;
        UpdateMonsterCount();
        MonsterCount++;
    }

    public void UpdateMonsterCount()
    {
        if (enemyCount == maxMonsterCount) tmpMonsterCount.color = Color.red;
        else tmpMonsterCount.color = Color.white;
        tmpMonsterCount.text = $"{enemyCount} / {maxMonsterCount}";
    }

    public IEnumerator ManaAnimation(TextMeshProUGUI tmp)
    {
        tmp.gameObject.SetActive(true);
        tmp.text = $"{playerGold.CurrentMana}M";
        Color color = tmp.color;
        color.a = 1;
        tmp.color = color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime;
            tmp.color = color;
            yield return null;
        }
        tmp.gameObject.SetActive(false);
    }
}
