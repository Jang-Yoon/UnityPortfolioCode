using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchingAnimation : MonoBehaviour
{
    private WaitForSeconds wait;
    private int timer;
    private void Start()
    {
        wait = new WaitForSeconds(1f);
    }

    private void OnEnable()
    {
        timer = 0;
    }


    public IEnumerator TextAnimation(TextMeshProUGUI tmpAniamtion)
    {
        string text = tmpAniamtion.text;
        while (true)
        {
            tmpAniamtion.text = $"{text}\n\n대기 시간({timer})";
            yield return wait;
            timer++;
        }
    }
}
