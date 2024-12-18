using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
public enum GameType
{
    Moves,
    Time
}
[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public TMP_Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private Board board;
    private float timerSeconds;
    public DifficultyLevel difficulty; //
    public enum DifficultyLevel //
    {
        Normal, // Обычный
        Hard    // Сложный
    }
    private string dbPath; //
    // Start is called before the first frame update
    void Start()
    {
        string difficultyString = PlayerPrefs.GetString("GameDifficulty", "Normal");
        difficulty = (difficultyString == "Normal") ? DifficultyLevel.Normal : DifficultyLevel.Hard;
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
    #if UNITY_EDITOR
            dbPath = "URI=file:" + Application.dataPath + "/Plugins/SQLiter/RegDB.db";
    #elif UNITY_STANDALONE_WIN
            dbPath = "URI=file:" + Application.dataPath + "/Plugins/SQLiter/RegDB.db";
    #else
            dbPath = "URI=file:" + Application.persistentDataPath + "/Plugins/SQLiter/RegDB.db";
    #endif
    }

    void SetGameType()
    {
        if(board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                    AdjustCounterValueForDifficulty();
                }
            }
        }
    }
    void AdjustCounterValueForDifficulty()
    {
        // Проверяем уровень сложности
        if (difficulty == EndGameManager.DifficultyLevel.Hard)
        {
            // Если сложность Hard, делим значения на 2
            requirements.counterValue = Mathf.CeilToInt(requirements.counterValue / 2f);
        }
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        if (difficulty == EndGameManager.DifficultyLevel.Hard)
        {
            // Если сложность Hard, делим значения на 2
            requirements.counterValue = Mathf.CeilToInt(requirements.counterValue * 2f);
        }
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

        int levelToUnlock = PlayerPrefs.GetInt("Current Level", 2);
        UnlockLevelInDatabase(levelToUnlock); // Разблокируем уровень в базе данных

        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        if (difficulty == EndGameManager.DifficultyLevel.Hard)
        {
            // Если сложность Hard, делим значения на 2
            requirements.counterValue = Mathf.CeilToInt(requirements.counterValue * 2f);
        }
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("Поражение");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <=0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
    public void UnlockLevelInDatabase(int levelId) //
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            string query = "UPDATE Levels SET IsUnlocked = 1 WHERE LevelID = @levelID";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@levelID", levelId);
                command.ExecuteNonQuery();
            }
        }
    }
}
