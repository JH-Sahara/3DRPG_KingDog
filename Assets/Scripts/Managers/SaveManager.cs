using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    private string sceneName = "_sceneName";
    public string SceneName
    {
        get{return PlayerPrefs.GetString(sceneName);}
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SavePlayerData();
            SceneController.Instance.TransitionToMenu();
        }
    }

    private void Save(object data,string key)
    {
        PlayerPrefs.SetString(sceneName,SceneManager.GetActiveScene().name);
        var jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key,jsonData);
        PlayerPrefs.Save();
    }
    private void Load(object data,string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key),data);
        }
    }

    //为Player创建保存数据的方法
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData,GameManager.Instance.playerStats.characterData.name);
    }
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData,GameManager.Instance.playerStats.characterData.name);
    }
}
