using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private MainManager mainManager;


    public static GameManager Instance { get; private set; }
    public bool IsMonstAlive { get; set; }
    public bool IsSpawn { get; set; }

    [SerializeField]private GameObject[] enemyPrefabs;
    [SerializeField]private float[] enemyBasicSpeed;

    public GameObject[] EnemyPrefabs => enemyPrefabs;
    public float[] EnemyBasicSpeed => enemyBasicSpeed;

    public bool isRain;
    public bool isSnow;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void GameExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public void SuccessLogin()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        yield return StartCoroutine(FadeIO.Instance.FadeIn(2));
        SceneManager.LoadScene((int)SceneList.MainScene);
        yield return new WaitForSeconds(1);
    }

    public IEnumerator LeftRoom()
    {
        yield return StartCoroutine(FadeIO.Instance.FadeIn(1));
        SceneManager.LoadScene((int)SceneList.MainScene);   
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        StartCoroutine(LeftRoom());
        mainManager = FindObjectOfType<MainManager>();
        StartCoroutine(mainManager.LeftOtherPlayer());
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void SetWeather(bool rain,bool snow)
    {
        isRain = rain;
        isSnow = snow;
    }

}
