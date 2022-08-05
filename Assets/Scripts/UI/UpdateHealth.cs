using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHealth : MonoBehaviour
{
    [Header("Must Get Setting")]
    public GameObject healthBarParfab;
    public Transform follow;
    public bool alwaysVisible;
    public float lifeTime;

    [Space]

    [Header("Base Setting")]
    public CharacterStats selfStats;
    public Transform mainCamera;
    public Transform healthBar;
    public Image currentHealthBar;
    public float currentTime;

    private void Awake() {
        selfStats = GetComponent<CharacterStats>();
        selfStats.UpdateDefenderHealth += UpdateDefenderHealth;
        selfStats.UpdateSelfHealth += UpdateSelfHealth;
    } 

    private void OnEnable() {
        mainCamera = Camera.main.transform;

        var canvas = GameObject.FindGameObjectWithTag("UiCanvas");
        if (canvas != null)
        {
            healthBar = Instantiate(healthBarParfab,canvas.transform).transform;
            currentHealthBar = healthBar.GetChild(0).GetComponent<Image>();
            healthBar.gameObject.SetActive(alwaysVisible);
        }
    }

    private void LateUpdate() {
        if (healthBar != null)
        {
            healthBar.position = follow.position;
            healthBar.forward = -mainCamera.forward;
            if (currentTime<=0 && !alwaysVisible)
                healthBar.gameObject.SetActive(false);
            else
                currentTime -= Time.deltaTime;
        }
    }

    private void UpdateDefenderHealth(CharacterStats defender,int currentHealth,int maxHealth)
    {
        defender.gameObject.GetComponent<UpdateHealth>().UpdateSelfHealth(currentHealth,maxHealth);
    }

    public void UpdateSelfHealth(int currentHealth,int maxHealth)
    {
        if (!healthBar)
            return;

        if (currentHealth <= 0)
        {
            Destroy(healthBar.gameObject);
        }

        healthBar.gameObject.SetActive(true);
        currentTime = lifeTime;
        currentHealthBar.fillAmount = (float)currentHealth/maxHealth;
    }
}
