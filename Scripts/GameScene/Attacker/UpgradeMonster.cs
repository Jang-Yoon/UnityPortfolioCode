using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public enum MonsterUpgradeType
{
    Hp,
    Speed,
    Time
}

public class UpgradeMonster : MonoBehaviourPunCallbacks
{
    [SerializeField] private int[] price;
    [SerializeField] private TextMeshProUGUI tmpGuided;
    [SerializeField] private Button buttonHpUpgrade;
    [SerializeField] private Button buttonSpeedUpgrade;
    [SerializeField] private Button buttonTimeUpgrade;
    [SerializeField] private GameObject[] enemyPrefabs;
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;
    private SpawnEnemyList spawnEnemyList;

    private void Start()
    {
        spawnEnemyList = GetComponent<SpawnEnemyList>();
        playerGold = GetComponent<PlayerGold>();
        enemySpawner = GetComponent<EnemySpawner>();
        buttonHpUpgrade.onClick.AddListener(() => photonView.RPC("MonsterHpUpgrade", RpcTarget.All));
        buttonSpeedUpgrade.onClick.AddListener(() => photonView.RPC("MonsterSpeedUpgrade", RpcTarget.All));
        buttonTimeUpgrade.onClick.AddListener(() => photonView.RPC("MonsterTimeUpgrade", RpcTarget.All));
    }

    [PunRPC]
    private void MonsterHpUpgrade()
    {
        if (playerGold.CurrentMana < price[0])
        {
            StartCoroutine(spawnEnemyList.ManaAnimation(spawnEnemyList.TmpNotEnoughMana)); 
            return;
        }
        playerGold.CurrentMana -= price[0];
        UpdateUI.Instance.UpdateLuciferMana();
        buttonHpUpgrade.interactable = false;
        enemyPrefabs[0].GetComponent<EnemyHp>().AddedHp = enemyPrefabs[0].GetComponent<EnemyHp>().MaxHp * 0.3f;
        enemyPrefabs[1].GetComponent<EnemyHp>().AddedHp = enemyPrefabs[1].GetComponent<EnemyHp>().MaxHp * 0.3f;
        enemyPrefabs[2].GetComponent<EnemyHp>().AddedHp = enemyPrefabs[2].GetComponent<EnemyHp>().MaxHp * 0.3f;
    }
    [PunRPC]
    private void MonsterSpeedUpgrade()
    {
        if (playerGold.CurrentMana < price[1])
        {
            StartCoroutine(spawnEnemyList.ManaAnimation(spawnEnemyList.TmpNotEnoughMana));
            return;
        }
        playerGold.CurrentMana -= price[1];
        UpdateUI.Instance.UpdateLuciferMana();
        buttonSpeedUpgrade.interactable = false;
        enemyPrefabs[0].GetComponent<Movement>().MoveSpeed *= 1.3f;
        enemyPrefabs[1].GetComponent<Movement>().MoveSpeed *= 1.3f;
        enemyPrefabs[2].GetComponent<Movement>().MoveSpeed *= 1.3f;
    }
    [PunRPC]
    private void MonsterTimeUpgrade()
    {
        if (playerGold.CurrentMana < price[2])
        {
            StartCoroutine(spawnEnemyList.ManaAnimation(spawnEnemyList.TmpNotEnoughMana));
            return;
        }
        playerGold.CurrentMana -= price[2];
        UpdateUI.Instance.UpdateLuciferMana();
        buttonTimeUpgrade.interactable = false;
        enemySpawner.SpawnInterval *= 0.5f;
    }

    public void MonsterUpgrade(string type)
    {
        if (type == MonsterUpgradeType.Hp.ToString()) tmpGuided.text = $"<color=green>체력 강화</color>\n{price[0]}<color=purple>M</color>";
        if (type == MonsterUpgradeType.Speed.ToString()) tmpGuided.text = $"<color=#11FFFF>속도 강화</color>\n{price[1]}<color=purple>M</color>";
        if (type == MonsterUpgradeType.Time.ToString()) tmpGuided.text = $"<color=orange>소환 주기 강화</color>\n{price[2]}<color=purple>M</color>";
    }
}
