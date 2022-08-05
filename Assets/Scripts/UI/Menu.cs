using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class Menu : MonoBehaviour
{
    Button newGame;
    Button continueGame;
    Button quitGame;
    PlayableDirector director;

    private void Awake() {
        newGame = transform.GetChild(1).GetComponent<Button>();
        continueGame = transform.GetChild(2).GetComponent<Button>();
        quitGame = transform.GetChild(3).GetComponent<Button>();

        //添加功能
        newGame.onClick.AddListener(PlayTimeline);
        continueGame.onClick.AddListener(ContinueGame);
        quitGame.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    private void PlayTimeline()
    {
        director.Play();
    }

    private void NewGame(PlayableDirector obj)
    {
        //删除旧数据
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();
    }

    private void ContinueGame()
    {
        SceneController.Instance.TransitionToContinueLevel();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
