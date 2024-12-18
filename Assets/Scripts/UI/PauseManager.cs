using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public SoundManager sound;

    void Start()
    {
        sound = FindAnyObjectByType<SoundManager>();
        board = FindAnyObjectByType<Board>();
        pausePanel.SetActive(false);
        if (PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOffSprite;
            }
            else
            {
                soundButton.sprite = musicOnSprite;
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }
        pausePanel.SetActive(false);
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                sound.adjustVolume();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                sound.adjustVolume();
            }
        }
        else
        {
            soundButton.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
            sound.adjustVolume();
        }
    }

    void Update()
    {
        if (paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
        }
        if (!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
        }
    }

    public void PauseGame()
    {
        paused = !paused;
    }

    public void ExitGame()
    {
        EndGameManager endGameManager = FindObjectOfType<EndGameManager>();
        if (endGameManager != null)
        {
            if (endGameManager.difficulty == EndGameManager.DifficultyLevel.Hard)
            {
                endGameManager.requirements.counterValue = Mathf.CeilToInt(endGameManager.requirements.counterValue * 2f);
            }
        }
        SceneManager.LoadScene("Splash");
    }
}
