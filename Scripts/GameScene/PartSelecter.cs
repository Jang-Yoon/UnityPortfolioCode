using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class PartSelecter : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI landowner;
    [SerializeField] private TextMeshProUGUI lucifer;
    [SerializeField] private Image selectPartBackground;
    [SerializeField] private GameObject MonsterSpawnBackground;
    [SerializeField] private GameObject TowerSpawnBackground;
    private string[] part = { "Landowner", "Lucifer" };

    private void Start()
    {
       StartCoroutine(FadeIO.Instance.FadeOut());
    
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {

                RandomPartSelect();
            }
            else
            {
                Debug.Log("플레이어 대기중");
            }
        }
    }


    private void RandomPartSelect()
    {
        // 각 플레이어에게 랜덤한 역할을 할당
        string partForPlayer1 = part[Random.Range(0, part.Length)];
        string partForPlayer2 = (partForPlayer1 == "Landowner") ? "Lucifer" : "Landowner"; // 두 역할이 하나씩 할당되도록
        // 각 플레이어에게 역할 할당
        photonView.RPC("SetPlayerPart", RpcTarget.All, partForPlayer1, partForPlayer2);
        //Debug.Log(Config.isAttacker);
    }

    private IEnumerator PartAnimation(TextMeshProUGUI part)
    {
        Color color = selectPartBackground.color;
        part.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        while (color.a>0)
        {
            color.a -= Time.deltaTime;
            selectPartBackground.color = color;
            part.color = color;
            yield return null;
        }
        selectPartBackground.gameObject.SetActive(false);
        part.gameObject.SetActive(false);
    }

    [PunRPC]
    void SetPlayerPart(string part1, string part2)
    {
        // 자신의 역할을 할당 (플레이어 1은 part1, 플레이어 2는 part2)
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            if(part1 == "Landowner")
            {
                StartCoroutine(PartAnimation(landowner));
                TowerSpawnBackground.SetActive(true);
                Config.isAttacker = false;
            }
            else
            {
                StartCoroutine(PartAnimation(lucifer));
                MonsterSpawnBackground.SetActive(true);
                Config.isAttacker = true;
            }
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            if (part2 == "Landowner")
            {
                StartCoroutine(PartAnimation(landowner));
                TowerSpawnBackground.SetActive(true);
                Config.isAttacker = false;
            }
            else
            {
                StartCoroutine(PartAnimation(lucifer));
                MonsterSpawnBackground.SetActive(true);
                Config.isAttacker = true;
            }
        }
    }

}
