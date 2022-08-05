using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Attack Data", menuName = "Attack Data/New Base Attack Data")]
public class BaseAttackData_SO : ScriptableObject
{
    public float attackRange; //近战距离
    public float remoteRange; //远程距离
    public float coolDown; //冷却时间
    public int minDamage; 
    public int maxDamage;
    public float criticalChance; //暴击率
    public float criticalMultiplier; //暴击倍率
}
