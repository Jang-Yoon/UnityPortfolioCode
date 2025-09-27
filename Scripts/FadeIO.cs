using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class FadeIO : MonoBehaviourPunCallbacks
{
    public static FadeIO Instance { get; private set; }

    [SerializeField,Header("페이드 인/아웃 할 이미지 오브젝트")] private Image fadeIOBackground;
    [SerializeField,Header("페이드 인/아웃 스피드")] private float speed;

    private void Awake()
    {
        Instance = this;
        //SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeOut());
    }

    [PunRPC]
    public IEnumerator FadeIn(float timer)
    {
        Color color = fadeIOBackground.color;
        color.a = 0;
        fadeIOBackground.color = color;
        fadeIOBackground.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        while (color.a < 1)
        {
            color.a += Time.deltaTime * speed;
            fadeIOBackground.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(timer); // 페이드 인 후 대기시간
    }
    public IEnumerator FadeOut()
    {
        Color color = fadeIOBackground.color;
        color.a = 1;
        fadeIOBackground.color = color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * speed;
            fadeIOBackground.color = color;
            yield return null;
        }
        fadeIOBackground.gameObject.SetActive(false);
    }

    public IEnumerator WaitFade(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
    }
}
