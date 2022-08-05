using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    public CinemachineFreeLook freeLookCamera;
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    override protected void Awake() {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;

        //获取跟踪相机
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        if (freeLookCamera != null)
        {
            freeLookCamera.Follow = playerStats.transform.GetChild(2);
            freeLookCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    //订阅EndGame
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //移除订阅
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //分发事件
    public void Notify()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
