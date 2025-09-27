using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateUI : MonoBehaviour
{
    // 싱글톤 : 여러 클래스에서 접근을 용이하도록 하는 설계
    private static UpdateUI instance = null;
    public static UpdateUI Instance => instance;

    [SerializeField] private TextMeshProUGUI tmpPlayerHp;
    [SerializeField] private TextMeshProUGUI tmpPlayerGold;
    [SerializeField] private TextMeshProUGUI tmpPlayerMana;
    [SerializeField] private TextMeshProUGUI tmpStage;
    [SerializeField] private TextMeshProUGUI[] tmpTowerPrice;
    [SerializeField] private TextMeshProUGUI[] tmpMonsterPrice;
    [SerializeField]private TowerDatabase[] towerDatabase;
    private LandHp playerHp;
    private PlayerGold playerGold;
    private StageSystem stage;
    private SpawnEnemyList spawnEnemyList;

    private void Awake()
    {
        if (instance == null) instance = this;

        // 각 클래스들의 스타트 함수의 호출 순서가 랜덤이므로 순서를 정해줘야 한다.
        playerHp = GetComponent<LandHp>();
        playerGold = GetComponent<PlayerGold>();
        stage = GetComponent<StageSystem>();
        spawnEnemyList = GetComponent<SpawnEnemyList>();
    }

    private void Start()
    {
        UpdateLandownerGold();
        UpdateLuciferMana();
        UpdatePlayerHp();
        UpdateStage();
        UpdateTowerAndMonsterPrice();
    }

    // 타워 열심히 건설했는데 적이 빠져나감 -> 이런 경우가 과연 우리 게임 플레이동안 몇 번 나오나?
    // 5초에 한 마리씩 놓친다. => 5초에 한 번씩 호출

    public void UpdatePlayerHp()
    {
        tmpPlayerHp.text = $"<color=red>HP</color> {playerHp.CurrentHp}";
    }

    public void UpdateLandownerGold()
    {
        tmpPlayerGold.text = playerGold.CurrentGold + "<color=yellow>G</color>";
    }
    public void UpdateLuciferMana()
    {
        tmpPlayerMana.text = playerGold.CurrentMana + "<color=purple>M</color>";
    }

    public void UpdateStage()
    {
        tmpStage.text = $"<color=orange>wave</color> {stage.CurrentStage}";
    }

    public void UpdateTowerAndMonsterPrice()
    {
        Debug.Log("가격 측정");
        tmpTowerPrice[0].text = $"{towerDatabase[0].TowerDatas[0].price}<color=yellow>G</color>";
        tmpTowerPrice[1].text = $"{towerDatabase[1].TowerDatas[0].price}<color=yellow>G</color>";
        tmpTowerPrice[2].text = $"{towerDatabase[2].TowerDatas[0].price}<color=yellow>G</color>";
        tmpMonsterPrice[0].text = $"{spawnEnemyList.EnemyPrefabs[0].GetComponent<Enemy>().Price}<color=purple>M</color>";
        tmpMonsterPrice[1].text = $"{spawnEnemyList.EnemyPrefabs[1].GetComponent<Enemy>().Price}<color=purple>M</color>";
        tmpMonsterPrice[2].text = $"{spawnEnemyList.EnemyPrefabs[2].GetComponent<Enemy>().Price}<color=purple>M</color>";
        Debug.Log("측정 완료");
    }

}
