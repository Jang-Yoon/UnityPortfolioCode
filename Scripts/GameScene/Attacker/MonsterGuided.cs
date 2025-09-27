using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class MonsterGuided : MonoBehaviour
{
    [SerializeField] private GameObject monsterInfoBackground;
    [SerializeField] private Button buttonMonsterInfo;
    private bool isOpen;

    private void Start()
    {
        buttonMonsterInfo.onClick.AddListener(OpenAndCloseMonsterInfo);
    }

    private void OpenAndCloseMonsterInfo()
    {
        isOpen = !isOpen;

        monsterInfoBackground.SetActive(isOpen);
    }
}
