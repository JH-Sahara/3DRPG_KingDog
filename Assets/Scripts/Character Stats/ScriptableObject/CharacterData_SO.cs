using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Character Data")]
public class CharacterData_SO : ScriptableObject
{
    public int maxHealth; //最大血量
    public int currentHealth; //当前血量
    public int defense; //防御力

    [Header("Enemy Exp")]
    public int exp;

    [Header("Player Update")]
    public int maxLevel;
    public int currentLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;
    public float LevelMultiplier
    {
        get
        {
            return 1 + (currentLevel-1) * levelBuff;
        }
    }

    public void UpdateExp(int exp)
    {
        if (currentLevel >= maxLevel)
        {
            return;
        }
        
        currentExp += exp;

        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        //所有想要升级的属性
        currentLevel = Mathf.Clamp(currentLevel+1,1,maxLevel);
        baseExp += (int)(baseExp*LevelMultiplier);
        currentExp = 0;
        maxHealth += (int)(maxHealth*levelBuff);
        currentHealth = maxHealth;
        defense += (int)(defense*levelBuff);
    }
}
