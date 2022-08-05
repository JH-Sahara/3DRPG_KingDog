using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private Text levelText;
    private Image healthSlider;
    private Image expSlider;

    private void Awake() {
        levelText = transform.GetChild(0).GetComponent<Text>();
        healthSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(2).GetChild(0).GetComponent<Image>();
    }

    private void Update() {
        var playerStats = GameManager.Instance.playerStats;
        levelText.text = "Level   " + playerStats.characterData.currentLevel.ToString("00");
        UpdateHealth(playerStats);
        UpdateExp(playerStats);
    }

    private void UpdateHealth(CharacterStats playerStats)
    {
        float percent = (float)playerStats.CurrentHealth/playerStats.MaxHealth;
        healthSlider.fillAmount = percent;
    }

    private void UpdateExp(CharacterStats playerStats)
    {
        float percent = (float)playerStats.characterData.currentExp/playerStats.characterData.baseExp;
        expSlider.fillAmount = percent;
    }
}
