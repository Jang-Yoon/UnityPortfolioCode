using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class TowerTooltip : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject towerTooltip;

    [SerializeField] private Image imageTower;
    [SerializeField] private TextMeshProUGUI tmpStat;

    [SerializeField] private Button buttonUpgrade;
    [SerializeField] private TextMeshProUGUI tmpNotEnoughPlayerGold;
    [SerializeField] private Transform towerGroup;
    private PlayerGold playerGold;

    private Tower currentTower;

    private void Start()
    {
        playerGold = GetComponent<PlayerGold>();
        buttonUpgrade.onClick.AddListener(()=>photonView.RPC("Upgrade",RpcTarget.All));
    }

    [PunRPC]
    private void Upgrade()
    {
        // 업그레이드 가능하면 업그레이드 수행을 한 것 이다.
        bool isUpgrage = currentTower.TryUpgrade();

        if (isUpgrage)
        {
            // 업그레이드 후 툴팁과 범위를 업데이트
            UpdateTooltip();
        }
        else
        {
            StartCoroutine(GoldAnimation(tmpNotEnoughPlayerGold));
        }
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

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)) TooltipOff();
    }

    [PunRPC]
    public void TooltipOn(int towerIndex)
    {
        currentTower = towerGroup.GetChild(towerIndex).GetComponent<Tower>();
        
        if (Config.isAttacker) return;
        towerTooltip.SetActive(true);
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        tmpStat.text = $"A : {currentTower.Damage}\nS : {currentTower.Speed}\n\n{currentTower.UpgradePrice}G";

        imageTower.sprite = currentTower.TowerSprite;

        // 삼항 연산자 = 조건 ? 값 : 값;
        // 조건이 참이면 값A, 거짓이면 B
        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    public void TooltipOff()
    {
        towerTooltip.SetActive(false);
    }
}
