using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO tempCharacterData;
    public CharacterData_SO characterData;
    public BaseAttackData_SO baseAttackData;

    [HideInInspector]
    public bool isCritical;
    public Action<CharacterStats,int,int> UpdateDefenderHealth;
    public Action<int,int> UpdateSelfHealth;

    private void Awake() {
        if (tempCharacterData != null)
        {
            characterData = Instantiate(tempCharacterData);
        }
    }
    
    #region Read from Data_SO

    public int MaxHealth //最大血量
    {
        get 
        {
            if (characterData != null)
            {
                return characterData.maxHealth;
            }else{
                return 0;
            }
        }
        set
        {
            characterData.maxHealth = value;
        }
    }

    public int CurrentHealth //当前血量
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentHealth;
            }else{
                return 0;
            }
        }
        set
        {
            characterData.currentHealth = value;
        }
    }

    public int Defense //防御力
    {
        get
        {
            if (characterData != null)
            {
                return characterData.defense;
            }else{
                return 0;
            }
        }
        set
        {
            characterData.defense = value;
        }
    }

    #endregion

    #region Take Attack Damage

    public void TakeDamage(CharacterStats defender)
    {
        float damage = UnityEngine.Random.Range(baseAttackData.minDamage,baseAttackData.maxDamage);
        if (isCritical)
        {
            damage *= baseAttackData.criticalMultiplier;
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }

        //计算真实伤害
        int finalDamage = Mathf.Max((int)damage - defender.Defense,1);
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - finalDamage,0);
        //更新血条
        if (!defender.gameObject.CompareTag("Player"))
        {
       
            UpdateDefenderHealth?.Invoke(defender,defender.CurrentHealth,defender.MaxHealth);
        }
        //升级
        if (defender.CurrentHealth <= 0 && !defender.gameObject.CompareTag("Player"))
        {
            characterData.UpdateExp(defender.characterData.exp);
        }
    }

    //该方法用于对自身造成伤害
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - Defense,1);
        CurrentHealth = Mathf.Max(CurrentHealth - finalDamage,0);
        if (!this.gameObject.CompareTag("Player"))
        {
            UpdateSelfHealth?.Invoke(CurrentHealth,MaxHealth);
        }
       
        if (CurrentHealth <= 0 && !this.gameObject.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().characterData.UpdateExp(characterData.exp);
        }
    }

    #endregion
}
