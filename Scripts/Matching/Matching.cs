using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Matching : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject m_panel_Loading; // 로딩 UI.
    [SerializeField] private TextMeshProUGUI m_text_CurrentPlayerCount; // 로딩 UI 중에서 현재 인원 수를 나타냄.
    [SerializeField] private Button buttonStart;

    void Awake()
    {
        // 마스터 클라이언트는 PhotonNetwork.LoadLevel()를 호출할 수 있고, 모든 연결된 플레이어는 자동적으로 동일한 레벨을 로드한다.
        PhotonNetwork.AutomaticallySyncScene = true;

        m_panel_Loading.SetActive(false);
    }

    void Start()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.Disconnect();
        print("서버 연결 시도.");
        PhotonNetwork.ConnectUsingSettings();
        buttonStart.interactable = false;
    }

    public void JoinRandomOrCreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // 인원 지정.

        // 방 참가를 시도하고, 실패하면 생성해서 참가함.
        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable() {}, expectedMaxPlayers: 2, // 참가할 때의 기준.
            roomOptions: roomOptions // 생성할 때의 기준.
        );
    }

    public void CancelMatching()
    {
        print("매칭 취소.");
        m_panel_Loading.SetActive(false);
        buttonStart.interactable = false;

        print("방 떠남.");
        PhotonNetwork.LeaveRoom();
    }

    private void UpdatePlayerCounts()
    {
        m_text_CurrentPlayerCount.text = $"사람을 찾는 중\n{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        
    }

    #region 포톤 콜백 함수

    public override void OnConnectedToMaster()
    {
        print("서버 접속 완료.");
        StartCoroutine(FadeIO.Instance.FadeOut());
        buttonStart.interactable = true;
    }
    public override void OnJoinedRoom()
    {
        print("방 참가 완료.");

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}은 인원수 {PhotonNetwork.CurrentRoom.MaxPlayers} 매칭 기다리는 중.");
        UpdatePlayerCounts();
        m_panel_Loading.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"플레이어 {newPlayer.NickName} 방 참가.");
        UpdatePlayerCounts();

        if (PhotonNetwork.IsMasterClient)
        {
            // 목표 인원 수 채웠으면, 맵 이동을 한다. 권한은 마스터 클라이언트만.
            // PhotonNetwork.AutomaticallySyncScene = true; 를 해줬어야 방에 접속한 인원이 모두 이동함.
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                StartCoroutine(StartGame());
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 {otherPlayer.NickName} 방 나감.");
        UpdatePlayerCounts();
    }

    #endregion
    private IEnumerator StartGame()
    {
        FadeIO.Instance.photonView.RPC("FadeIn", RpcTarget.Others, 1f);
        photonView.RPC("MonsterSetting", RpcTarget.Others);
        MonsterSetting();
        yield return StartCoroutine(FadeIO.Instance.FadeIn(1));


        PhotonNetwork.LoadLevel(SceneList.GameScene.ToString());
    }

    [PunRPC]
    private void MonsterSetting()
    {
        GameManager.Instance.EnemyPrefabs[0].GetComponent<Movement>().MoveSpeed = GameManager.Instance.EnemyBasicSpeed[0];
        GameManager.Instance.EnemyPrefabs[1].GetComponent<Movement>().MoveSpeed = GameManager.Instance.EnemyBasicSpeed[1];
        GameManager.Instance.EnemyPrefabs[2].GetComponent<Movement>().MoveSpeed = GameManager.Instance.EnemyBasicSpeed[2];
        GameManager.Instance.EnemyPrefabs[0].GetComponent<EnemyHp>().AddedHp = 0;
        GameManager.Instance.EnemyPrefabs[1].GetComponent<EnemyHp>().AddedHp = 0;
        GameManager.Instance.EnemyPrefabs[2].GetComponent<EnemyHp>().AddedHp = 0;
        GameManager.Instance.IsSpawn = false;
    }
}
