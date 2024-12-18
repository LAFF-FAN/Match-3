using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;
    public Image[] stars;
    public int level;
    // Start is called before the first frame update
    void Start()
    {
        ActiveStars();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActiveStars()
    {
        //BD
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetString("GameDifficulty", "Normal");
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }
    public void PlayHard()
    {
        PlayerPrefs.SetString("GameDifficulty", "Hard");
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }
}
