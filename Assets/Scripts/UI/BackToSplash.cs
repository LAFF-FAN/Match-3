using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour
{
    public string SceneToLoad;
    private GameData gameData;
    private Board board;

    public void WinOK()
    {
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }
        SceneManager.LoadScene(SceneToLoad);
    }
    public void LoseOK()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData =  FindObjectOfType<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
