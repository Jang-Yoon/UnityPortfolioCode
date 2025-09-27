using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    private TextMeshProUGUI tmpTabToStart;
    private void Start()
    {
        tmpTabToStart = GetComponent<TextMeshProUGUI>();
        StartCoroutine(IntroTextAnimation());
    }

    private IEnumerator IntroTextAnimation()
    {
        Color color = tmpTabToStart.color;
        while(color.a > 0)
        {
            color.a -= Time.deltaTime;
            tmpTabToStart.color = color;
            yield return null;
        }
        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            tmpTabToStart.color = color;
            yield return null;
        }
        StartCoroutine(IntroTextAnimation());
    }
}
