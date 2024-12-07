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

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
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
