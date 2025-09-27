using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 포톤 기능
using Photon.Realtime; // 포톤 실시간 기능
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacks : 모노비헤이비어 기능 + 포톤 콜백 기능
// CallBack : 특정 조건 달성했을 시 호출되는 함수
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager instance = null;
    public static PhotonManager Instance => instance;

    // 게임 버전이 서로 같은 유저끼리만 만날 수 있다.
    private string gameVersion = "0.0.1b";

    private string roomName = "Test";

    // 특정 상황 달성 시 호출되는 것 : 이벤트
    // "플레이어가 생성되었다는 것" 을 이벤트로 정의한다.
    public delegate void PlayerCreated(); // 임시 함수
    //public event PlayerCreated playerCreated;

    // Start, Awake는 씬이 바뀌면 호출 안 됨 (인스턴싱은 1회만 되기 때문에) 
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject); // 중복 매니저 방지하기 위해서 작성

        // 씬이 바뀌어도 해당 오브젝트를 유지한다.
        DontDestroyOnLoad(gameObject);

        gameVersion = Config.isRelease ? "0.0.1" : "0.0.1b";
    }

    // 게임오브젝트가 활성화될 때 호출 (씬이 바뀌어도 호출됩니다.)
    // 포톤에서 재정의 해놓은 OnEnable호출
    public override void OnEnable()
    {
        base.OnEnable();

        // 씬이 로드될 때 호출할 함수를 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 플레이어 생성
    private IEnumerator CreatePlayer(Vector3 createPosition)
    {
        Debug.Log("데이터 받아오는 중...");
        // 우리 게임은 아직 서버 연동이 안 되어 있지만, 
        // 차후 서버 연동을 고려해서 임의로 "서버에서 데이터 가져오는 시간"으로 2초를 주었음.
        yield return new WaitForSeconds(2f);

        // 플레이어 생성 전 포톤의 닉네임을 할당해준다.
        PhotonNetwork.NickName = Config.playerNickName;

        // 플레이어의 회전값
        Quaternion rotation = Quaternion.identity;
        // 생성하려는 오브젝트의 프리펩이 반드시 Resources폴더에 위치해야 한다.

        // 플레이어의 생성 이벤트를 호출한다.
        //playerCreated.Invoke();

        // 생성하려는 오브젝트에 반드시 photonview 컴포넌트가 할당되어 있어야 한다.
    }

    // 씬이 로드될 때 동작할 코드를 분기한다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == (int)SceneList.MainScene)
        {
            // 포톤 셋업

            // 씬을 자동으로 동기화하는 기능 On
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            Debug.Log("포톤 서버와 데이터 통신 수 : " + PhotonNetwork.SendRate);
            //GameManager.Instance.CheckMatching();
        }
    }


    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            StartCoroutine(ConnectToPhotonCoroutine());
        }
    }

    // 포톤 서버 세팅의 정보로 포톤에 연결하는 함수
    private IEnumerator ConnectToPhotonCoroutine()
    {
        yield return StartCoroutine(FadeIO.Instance.FadeIn(2));
        PhotonNetwork.ConnectUsingSettings();
    }

    // 방에 접속하는 함수x
    public void JoinRoom()
    {
        // 방의 속성 정의
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = 2; // 0으로 설정 시 최대 인원
        option.IsOpen = true; // 방 개폐 여부
        option.IsVisible = true; // 공개방, 비밀방 여부

        // 포톤은 방 이름이 동일한 유저끼리 만날 수 있다.
        // 방에 접속하거나, 해당 방이 없다면 생성 후 접속하는 함수
        PhotonNetwork.JoinOrCreateRoom(roomName, option, TypedLobby.Default);
    }

    // 포톤과 연결되면 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 서버에 접속 성공 : " + PhotonNetwork.IsConnected);

        // 로비에 접속 시도
        PhotonNetwork.JoinLobby();
    }
    // 씬 전환
    private void LoadMainScene(SceneList scene)
    {
        Debug.Log("메인 씬 로드중...");
        // 씬이 동기화된다. (즉, 다른 유저와 함께 게임 가능하다.)
        // 마스터 클라이언트가 아닌 다른 유저는 자동으로 동기화된 씬으로 접속한다.
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel((int)scene);
        else Debug.Log("마스터 클라이언트가 아님.");
    }

    // 로비 서버에 접속 성공 시 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속 성공 : " + PhotonNetwork.InLobby);

        // 방에 접속 시도
        JoinRoom();
    }

    // 방 생성 시 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료 : " + PhotonNetwork.CurrentRoom.Name);
    }

    // 방에 접속 시 호출되는 콜백
    public override void OnJoinedRoom()
    {
        Debug.Log("방 접속 성공 : " + PhotonNetwork.InRoom + "현재 접속 인원 : " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    // 여러 이슈로 인해서 방이 접속 실패했을 때
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방에 접속 실패 : " + returnCode + " 원인 : " + message + " 방 생성 시도...");
        JoinRoom();
    }
}
