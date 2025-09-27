using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


// 어떻게 원하는 위치에 생성 시킬까?
// 그라운드의 위치에 건설하면 된다.
public class TowerBuilder : MonoBehaviourPunCallbacks
{
    [SerializeField] private TowerDatabase[] towerDatabase;
    [SerializeField] private Transform towerGroup;
    [SerializeField] private Button[] buttonTower;
    [SerializeField] private TextMeshProUGUI tmpNotEnoughPlayerGold;
    [SerializeField] private TextMeshProUGUI[] tmpTowerCounts;
    private BuffList buffList;
    [SerializeField] private int[] towersMaxCount;

    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;

    private GameObject tempTowerImage;
    private bool isButtonClick;

    private TowerType towerType;
    private int basic;
    private int power;
    private int speed;
    private int towerIndex;

    // 타워 관리 리스트
    private List<Tower> towers = new List<Tower>();
    public List<Tower> Towers { get => towers; set => towers = value; }

    public BuffList BuffList => buffList;

    private void Start()
    {
        tmpTowerCounts[0].text = $"{basic} / {towersMaxCount[0]}";
        tmpTowerCounts[1].text = $"{power} / {towersMaxCount[1]}";
        tmpTowerCounts[2].text = $"{speed} / {towersMaxCount[2]}";

        enemySpawner = GetComponent<EnemySpawner>();
        playerGold = GetComponent<PlayerGold>();
        buffList = GetComponent<BuffList>();

        // 버튼 연결 시 매개변수 있는 함수를 연결하려면
        // 람다식 또는 델리게이트 사용
        buttonTower[(int)TowerType.Basic].onClick.AddListener(() => photonView.RPC("ReadyToBuild",RpcTarget.All, TowerType.Basic));
        buttonTower[(int)TowerType.Power].onClick.AddListener(() => photonView.RPC("ReadyToBuild", RpcTarget.All, TowerType.Power));
        buttonTower[(int)TowerType.Slow].onClick.AddListener(() => photonView.RPC("ReadyToBuild", RpcTarget.All, TowerType.Slow));
        buttonTower[(int)TowerType.Speed].onClick.AddListener(() => photonView.RPC("ReadyToBuild", RpcTarget.All, TowerType.Speed));
    }

    private void Update()
    {
        if (!Config.isAttacker && Input.GetKeyDown(KeyCode.Alpha1))
        {
            photonView.RPC("IsButtonClick", RpcTarget.All, false);
            Destroy(tempTowerImage);
            buttonTower[(int)TowerType.Basic].onClick.Invoke();
        }
        if (!Config.isAttacker && Input.GetKeyDown(KeyCode.Alpha2))
        {
            photonView.RPC("IsButtonClick", RpcTarget.All, false);
            Destroy(tempTowerImage);
            buttonTower[(int)TowerType.Power].onClick.Invoke();
        }
        if (!Config.isAttacker && Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("IsButtonClick", RpcTarget.All, false);
            Destroy(tempTowerImage);
            buttonTower[(int)TowerType.Speed].onClick.Invoke();
        }
        //if (!Config.isAttacker && Input.GetKeyDown(KeyCode.Alpha1)) buttonTower[(int)TowerType.Basic].onClick.Invoke();
    }

    [PunRPC]
    public void ReadyToBuild(TowerType type)
    {
        if (isButtonClick) return; // 중복 생성 방지

        if (type == TowerType.Basic && basic == towersMaxCount[0]) return;
        else if (type == TowerType.Power && power == towersMaxCount[1]) return;
        else if (type == TowerType.Speed && speed == towersMaxCount[2]) return;

        // enum을 매개변수로 전달 받아서 int로 형변환하여 index로 사용하기 위함.
        towerType = type;

        if (playerGold.CurrentGold < towerDatabase[(int)towerType].TowerDatas[0].price)
        {
            StartCoroutine(GoldAnimation(tmpNotEnoughPlayerGold));
            return;
        }
        //isButtonClick = true;

        photonView.RPC("IsButtonClick", RpcTarget.All, true);

        if(!Config.isAttacker) tempTowerImage = Instantiate(towerDatabase[(int)towerType].TempTowerImage);

        

        StartCoroutine(CancelTower());
    }

    private IEnumerator GoldAnimation(TextMeshProUGUI tmp)
    {
        tmp.gameObject.SetActive(true);
        tmp.text = $"{playerGold.CurrentGold}G";
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

    // 타워 건설 취소
    private IEnumerator CancelTower()
    {
        if (Config.isAttacker) yield break;
        while (true)
        {
            // esc or 우클릭
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                //isButtonClick = false;
                photonView.RPC("IsButtonClick", RpcTarget.All, false);
                Destroy(tempTowerImage);
                yield break; // 취소 후 반복문 탈출 (break : 반복문 탈출 / yield break : 코루틴 탈출)
            }
            yield return null;
        }
    }

    // 타워 건설 시 그라운드 타일 체크해서
    // 이미 타워 건설 돼 있으면 짓지 않기

    [PunRPC]
    public void BuildTower(Vector3 groundPosition)
    {
        //Debug.Log("타워 건설 함수 호출");
        if (!isButtonClick) return;

        GameObject tileObject = FindTileAtPosition(groundPosition);

        TileGround tile = tileObject.GetComponent<TileGround>();

        if (tile.IsBuilted)
        {
            return;
        }

        // 치트 방지 : 타워 건설 후 그자리에 겹쳐서 클릭한 후 업그레이드 후에 다른 타일에 건설 가능
        if (playerGold.CurrentGold < towerDatabase[(int)towerType].TowerDatas[0].price)
        {
            //isButtonClick = false;
            photonView.RPC("IsButtonClick", RpcTarget.All, false);
            Destroy(tempTowerImage);
            return;
        }

        //isButtonClick = false;
        photonView.RPC("IsButtonClick", RpcTarget.All, false);

        // 파라미터가 4개일 때
        // Instantiate(뭘 생성할지?, 어디에 생성할지?, 어떤회전 값으로 생성할지?, 어떤 부모에 생성할지?);
        // Quaternion.identity = 회전값 (0, 0, 0) 어떠한 회전값도 주고싶지 않을 때
        GameObject cloneTower = Instantiate(towerDatabase[(int)towerType].TowerPrefab, groundPosition + Vector3.back, Quaternion.identity, towerGroup);
        cloneTower.GetComponent<Tower>().Index = towerIndex;
        towerIndex++;
        // 타워 작동
        Tower tower = cloneTower.GetComponent<Tower>();
        tower.Setup(enemySpawner, playerGold, this);
        tile.IsBuilted = true;
        if (tower.TowerType == TowerType.Basic) basic++;
        else if (tower.TowerType == TowerType.Power) power++;
        else if (tower.TowerType == TowerType.Speed) speed++;
        tmpTowerCounts[0].text = $"{basic} / {towersMaxCount[0]}";
        tmpTowerCounts[1].text = $"{basic} / {towersMaxCount[1]}";
        tmpTowerCounts[2].text = $"{basic} / {towersMaxCount[2]}";

        playerGold.CurrentGold -= towerDatabase[(int)towerType].TowerDatas[0].price;
        UpdateUI.Instance.UpdateLandownerGold();

        if(!Config.isAttacker) Destroy(tempTowerImage); // 건설 후 임시 타워 제거

        towers.Add(tower);

        StopCoroutine(CancelTower());
    }

    private GameObject FindTileAtPosition(Vector3 position)
    {
        // 씬에서 모든 타일을 찾기
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("TileGround");  

        // 각 타일의 위치를 비교하여 맞는 타일 찾기
        foreach (GameObject tile in allTiles)
        {
            // 타일의 위치가 원하는 위치와 가까운지 비교 (오차 범위 허용)
            if (Vector3.Distance(tile.transform.position, position) < 0.1f)  // 위치 오차 허용 범위 (0.1f)
            {
                return tile;
            }
        }

        return null;  // 해당 위치에 타일이 없다면 null 반환
    }

    [PunRPC]
    private void IsButtonClick(bool isBool)
    {
        isButtonClick = isBool;
    }
}

// 두 오브젝트가 동일한 위치에 있을 대 서로 싸우는 것 Z-Fight라고 한다.
