using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class BuffList : MonoBehaviour
{
    [SerializeField] private Transform buffGroup;
    [SerializeField] private GameObject towerBuffPrefab;
    [SerializeField] private Sprite[] towerSprites; //0 : 파워 / 1 : 스피드
    private TowerType towerType;
    private TowerDatabase towerDatabase;

    private void Start()
    {
        if (GameManager.Instance.isRain && !GameManager.Instance.isSnow) // 비올때
        {
            TowerBuffList(towerSprites[0], "<color=red>A -10%</color>");
            TowerBuffList(towerSprites[1], "<color=#00FFFF>S +20%</color>");
            return;
        }
        if (!GameManager.Instance.isRain && !GameManager.Instance.isSnow) // 안올때
        {
            TowerBuffList(towerSprites[0], "<color=#00FFFF>A +20%</color>");
            return;
        }
        if (!GameManager.Instance.isRain && GameManager.Instance.isSnow) // 눈올때
        {
            TowerBuffList(towerSprites[0], "<color=red>A -20%</color>");
            TowerBuffList(towerSprites[1], "<color=#00FFFF>A +20%</color>");
            return;
        }
        if (GameManager.Instance.isRain && GameManager.Instance.isSnow) // 다올때
        {
            TowerBuffList(towerSprites[0], "<color=red>A -40%</color>");
            TowerBuffList(towerSprites[1], "<color=#00FFFF>A +20%</color>");
            TowerBuffList(towerSprites[1], "<color=#00FFFF>S +20%</color>");
            return;
        }
    }

    public float SetDamage(float damage, float value)
    {
        int lastDamage;
        lastDamage = (int)(damage * value);
        return lastDamage;
    }

    public void TowerBuffList(Sprite sprite,string value)
    {
        if (GameManager.Instance.isRain && GameManager.Instance.isSnow)
        {
            GameObject temp = Instantiate(towerBuffPrefab, buffGroup);
            temp.GetComponent<Image>().sprite = sprite;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = value;
            return;
        }
        if(GameManager.Instance.isRain && !GameManager.Instance.isSnow)
        {
            GameObject temp = Instantiate(towerBuffPrefab, buffGroup);
            temp.GetComponent<Image>().sprite = sprite;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = value;
            return;
        }
        if (!GameManager.Instance.isRain && GameManager.Instance.isSnow)
        {
            GameObject temp = Instantiate(towerBuffPrefab, buffGroup);
            temp.GetComponent<Image>().sprite = sprite;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = value;
            return;
        }
        if (!GameManager.Instance.isRain && !GameManager.Instance.isSnow)
        {
            GameObject temp = Instantiate(towerBuffPrefab, buffGroup);
            temp.GetComponent<Image>().sprite = sprite;
            temp.GetComponentInChildren<TextMeshProUGUI>().text = value;
            return;
        }
    }
}
