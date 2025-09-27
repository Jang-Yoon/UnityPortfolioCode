using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameResult : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject resultBackground;

    [SerializeField] private TextMeshProUGUI tmpLose;
    [SerializeField] private TextMeshProUGUI tmpWin;

    [SerializeField] private TextMeshProUGUI tmpLuciferWin;
    [SerializeField] private TextMeshProUGUI tmpLuciferLose;
    [SerializeField] private TextMeshProUGUI tmpLandownerWin;
    [SerializeField] private TextMeshProUGUI tmpLandownerLose;

    public IEnumerator Win()
    {
        resultBackground.SetActive(true);
        tmpWin.gameObject.SetActive(true);
        if (Config.isAttacker) tmpLuciferWin.gameObject.SetActive(true);
        else tmpLandownerWin.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);
        StartCoroutine(GameManager.Instance.LeftRoom());
    }
    public IEnumerator Lose()
    {
        resultBackground.SetActive(true);
        tmpLose.gameObject.SetActive(true);
        if (Config.isAttacker) tmpLuciferLose.gameObject.SetActive(true);
        else tmpLandownerLose.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);
        StartCoroutine(GameManager.Instance.LeftRoom());
    }
}
