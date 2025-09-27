using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField] private int currntGold = 100;
    [SerializeField] private int currentMana = 100;
    public int CurrentGold { get => currntGold; set => currntGold = Mathf.Max(0, value); }
    public int CurrentMana { get => currentMana; set => currentMana = Mathf.Max(0, value); }
    // Mathf.Max(a, b); a와 b 중 높은 값을 리턴
}
