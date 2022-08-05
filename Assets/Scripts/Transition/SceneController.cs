using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPerfab;
    public FaderCanvas faderCanvasPerfab;
    GameObject player;
    NavMeshAgent playerAgent;
    bool fadeFinished;
    
    override protected void Awake() {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start() {
        fadeFinished = false;
        GameManager.Instance.AddObserver(this);
    }

    public void TransitionToDesitiation(TransitionPortal transitionPortal)
    {
        player = GameManager.Instance.playerStats.gameObject;
        playerAgent = player.GetComponent<NavMeshAgent>();
        
        switch(transitionPortal.transitionType)
        {
            case TransitionPortal.TransitionType.SameScene :
                StartCoroutine(SameTransition(transitionPortal.desitiationTag));
                break;
            case TransitionPortal.TransitionType.DifferentScene :
                StartCoroutine(DifferentTransition(transitionPortal.sceneName,transitionPortal.desitiationTag));
                break;
        }
    }

    //同场景传送
    IEnumerator SameTransition(TransitionDesitiation.DesitiationTag tag)
    {
        playerAgent.enabled = false;
        var desitiation = GetDesitiation(tag);
        player.transform.SetPositionAndRotation(desitiation.transform.position,desitiation.transform.rotation);
        playerAgent.enabled = true;
        yield return null;
    }

    //不同场景传送
    IEnumerator DifferentTransition(string sceneName,TransitionDesitiation.DesitiationTag tag)
    {
        SaveManager.Instance.SavePlayerData();
        yield return SceneManager.LoadSceneAsync(sceneName);
        var desitiation = GetDesitiation(tag);
        yield return Instantiate(playerPerfab,desitiation.transform.position,desitiation.transform.rotation);
        SaveManager.Instance.LoadPlayerData();
    }

    //获取tag
    private TransitionDesitiation GetDesitiation(TransitionDesitiation.DesitiationTag tag)
    {
        var desitiations = FindObjectsOfType<TransitionDesitiation>();
        for (int i=0; i<desitiations.Length; i++)
        {
            if (desitiations[i].desitiationTag == tag)
                return desitiations[i];
        }

        return null;
    }

    //获取每个场景初始点的位置Tag
    private Transform GetEnter()
    {
        foreach(var item in FindObjectsOfType<TransitionDesitiation>())
        {
            if (item.desitiationTag == TransitionDesitiation.DesitiationTag.ENTER)
                return item.transform;
        }
        return null;
    }

    //NewGame
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Scene1"));
    }

    public void TransitionToContinueLevel()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    
    public void TransitionToMenu()
    {
        StartCoroutine(LoadMenu());
    }

    IEnumerator LoadLevel(string sceneName)
    {
        FaderCanvas fade = Instantiate(faderCanvasPerfab);

        if (sceneName != "")
        {
            yield return fade.FadeIn(fade.fadeInTime);
            yield return SceneManager.LoadSceneAsync(sceneName);
            var enterPoint = GetEnter();
            yield return Instantiate(playerPerfab,enterPoint.position,enterPoint.rotation);
            SaveManager.Instance.SavePlayerData();
            yield return fade.FadeOut(fade.fadeOutTime);
            yield break;
        }
    }

    IEnumerator LoadMenu()
    {
        FaderCanvas fade = Instantiate(faderCanvasPerfab);
        yield return fade.FadeIn(fade.fadeInTime);
        yield return SceneManager.LoadSceneAsync("Menu");
        yield return fade.FadeOut(fade.fadeOutTime);
        yield break;
    }

    //暂停时间的携程
    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
        yield return LoadMenu();
        yield break;
    }

    public void EndNotify()
    {
        if (!fadeFinished)
        {
            fadeFinished = true;
            StartCoroutine(WaitTime(3f));
        }
    }
}
