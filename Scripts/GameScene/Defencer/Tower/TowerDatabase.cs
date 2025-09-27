using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  ScriptableObject : 스크립팅하여 만드는 커스텀 오브젝트
/// </summary>
/// 

[CreateAssetMenu] // 프로젝트 탭에 마우스 우클릭 시 커스텀 메뉴 생성
public class TowerDatabase : ScriptableObject
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private TowerData[] towerDatas;
    [SerializeField] private GameObject tempTowerImage;

    public GameObject TowerPrefab => towerPrefab;
    public GameObject TempTowerImage => tempTowerImage;
    public TowerData[] TowerDatas => towerDatas;


    [System.Serializable]
    public struct TowerData
    {
        public Sprite sprite;
        public int damage;
        public int originDamage;
        public float speed;
        //public float range;
        public float slow; // 감속률 ex) 0.3 -> 30%의 감속률
        //public float buff; // 증가율 0.2 -> 20% 데미지 증가
        public int price;
    }
}
