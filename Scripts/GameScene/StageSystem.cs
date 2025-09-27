using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// 구조체와 클래스의 차이점
// class : 상속이 가능하다, 비교적 복잡하고 다량의 데이터를 관리함. 참조타입
// struct : 상속 불가능, 비교적 간단한 데이터 관리. 값 타입

public class StageSystem : MonoBehaviourPunCallbacks
{
    [SerializeField] private int stage;

    [SerializeField] private Button buttonSkipTurn;

    private GameResult gameResult;
    private int currentStageIndex = -1;
    private PlayerGold playerGold;
    private bool isStartStage;


    public bool IsStartStage { set { isStartStage = value; } }
    // 현재 스테이지
    public int CurrentStage => currentStageIndex + 1;
    // 마지막 스테이지
    public int MaxStage => stage;
    public Button ButtonSkipTurn { get => buttonSkipTurn;set { buttonSkipTurn = value; } }


    private void Awake()
    {
        buttonSkipTurn.onClick.AddListener(ManaCheck);
    }

    private void Start()
    {
        playerGold = GetComponent<PlayerGold>();
        gameResult = GetComponent<GameResult>();
    }

    [PunRPC]
    public void StartStage()
    {
        if (currentStageIndex == MaxStage-1)
        {
            if (Config.isAttacker) StartCoroutine(gameResult.Lose());
            else StartCoroutine(gameResult.Win());
            return;
        }
        if (!isStartStage)
        {
            currentStageIndex++;
            PaymentGoldAndMana(60 + (CurrentStage * 120), 300 + (CurrentStage * 500));

            UpdateUI.Instance.UpdateStage();
        }
    }

    private void ManaCheck()
    {
        if (playerGold.CurrentMana < 15)
        {
            photonView.RPC("StartStage", RpcTarget.All);
        }
        else
        {
            return;
        }
    }

    private void PaymentGoldAndMana(float gold, float mana)
    {
        playerGold.CurrentGold += (int)gold;
        playerGold.CurrentMana += (int)mana;
        UpdateUI.Instance.UpdateLandownerGold();
        UpdateUI.Instance.UpdateLuciferMana();

    }
}
