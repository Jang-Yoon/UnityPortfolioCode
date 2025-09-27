using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Image leftOtherPlayerBackground;
    private void Start()
    {
        //buttonStart.onClick.AddListener(Matching);
        buttonExit.onClick.AddListener(Exit);
    }

    private void Exit()
    {
        GameManager.Instance.GameExit();
    }

    private void Matching()
    {
        PhotonManager.Instance.ConnectToPhoton();
    }

    public IEnumerator LeftOtherPlayer()
    {
        yield return StartCoroutine(FadeIO.Instance.FadeOut());
        leftOtherPlayerBackground.gameObject.SetActive(true);
        Color color = leftOtherPlayerBackground.color;
        color.a = 1;
        leftOtherPlayerBackground.color = color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime;
            leftOtherPlayerBackground.color = color;
            yield return null;
        }
        leftOtherPlayerBackground.gameObject.SetActive(false);
    }    
}
