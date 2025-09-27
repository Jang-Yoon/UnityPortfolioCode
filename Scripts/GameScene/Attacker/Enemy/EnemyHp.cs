using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHp : MonoBehaviour
{
    [SerializeField] private float maxHp;
    [SerializeField] private TextMeshPro tmpHp;
    private int currentHp;
    private bool isDie;
    private Enemy enemy;
    private StageSystem stage;

    [SerializeField]private float addedHp;
    public float AddedHp { set { addedHp = value; } }
    public float MaxHp => maxHp;

    private void Start()
    {
        stage = FindObjectOfType<StageSystem>();
        if(stage.CurrentStage == 0) maxHp *= stage.CurrentStage+1;
        else maxHp *=(stage.CurrentStage+1) * 0.8f;
        enemy = GetComponent<Enemy>();
        currentHp = (int)maxHp + (int)addedHp;
        tmpHp.text = currentHp.ToString();
    }

    public void Hit(int damage)
    {
        if (isDie) return;

        currentHp -= damage;
        tmpHp.text = currentHp.ToString();
        if(currentHp <= 0)
        {
            isDie = true;
            enemy.Die(DeathType.Die);
        }
    }


}
