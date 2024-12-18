using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public TMP_Text scoreText;
    public int score;
    public Image scoreBar;
    private GameData gameData;
    private int numberStars;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();

    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + score;
        UpdateBar();
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        for(int i = 0; i < board.scoreGoals.Length; i++)
        {
            if(score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars ++;
            }
        }
        if(gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }
            int currentStars = gameData.saveData.stars[board.level];
            if(numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
            }
            gameData.Save();
        }
    }

    private void OnSceneLoad()
    {
        
    }

    private void UpdateBar()
    {
        if (board != null && scoreBar != null)
        {
            int lenght = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[lenght - 1];
        }
    }

}
