using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject levelPanel;
    public GameObject logPanel;
    void Start()
    {
        startPanel.SetActive(true);
        levelPanel.SetActive(false);

    }

    public void PlayGame()
    {
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void Home()
    {
        startPanel.SetActive(true);
        levelPanel.SetActive(false);
    }
    public void LogIn()
    {
        startPanel.SetActive(false);
        logPanel.SetActive(true);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // В противном случае выходим из игры
    Application.Quit();
#endif
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
