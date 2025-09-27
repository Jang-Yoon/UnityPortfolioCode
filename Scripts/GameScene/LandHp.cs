using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandHp : MonoBehaviour
{
    [SerializeField] private Image hitEffect;

    [SerializeField] private float maxHp = 10;
    private float currentHp;
    private GameResult gameResult;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Start()
    {
        gameResult = GetComponent<GameResult>();
        currentHp = maxHp;
    }

    public void Hit(float damage)
    {
        currentHp -= damage;

        StopCoroutine(HitAnimation());
        StartCoroutine(HitAnimation());

        if (currentHp <= 0)
        {
            if (Config.isAttacker) StartCoroutine(gameResult.Win());
            else StartCoroutine(gameResult.Lose());
        }
    }

    // 플레이어 체력 감소 효과
    private IEnumerator HitAnimation()
    {
        // 이미지의 컬러의 알파값 조절
        Color color = hitEffect.color;
        color.a = 0.4f;
        hitEffect.color = color;

        // 알파가 0이상일 때 점차 감소
        while (color.a >= 0)
        {
            color.a -= Time.deltaTime;
            hitEffect.color = color;

            yield return null;
        }
    }
}
