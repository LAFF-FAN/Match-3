using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Collections;

public class LevelButton : MonoBehaviour
{
    [Header("Active Staff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;

    public Image[] stars;
    public TMP_Text levelText;
    public int level;
    public GameObject confirmPanel;

    private string dbPath;

    private bool dataLoaded = false;//
    void Start()
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();

#if UNITY_EDITOR
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/SQLiter/RegDB.db";
#elif UNITY_STANDALONE_WIN
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/SQLiter/RegDB.db";
#else
        dbPath = "URI=file:" + Application.persistentDataPath + "/Plugins/SQLiter/RegDB.db";
#endif
        //StartCoroutine(LoadLevelData());
        DecideSprite();
        ActiveStars();
        ShowLevel();
    }
  
    void ActiveStars()
    {
        //BD
        for(int i =0; i< stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = "" + level;
    }

    // Update is called once per frame
    void Update() //
    {
        DecideSprite();
        /* Проверяем, загружены ли данные
        if (dataLoaded)
        {
            //StartCoroutine(LoadLevelData()); // Периодически перезагружаем данные
            dataLoaded = false; // Сбрасываем флаг, чтобы избежать многократной загрузки
        }
        */
    }
    /*IEnumerator LoadLevelData()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // Запрос для получения IsUnlocked и IsCompleted для текущего уровня
            string query = "SELECT IsUnlocked, IsCompleted FROM Levels WHERE LevelID = @levelID";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@levelID", level);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool isUnlocked = reader.GetInt32(0) == 1; // IsUnlocked
                        bool isCompleted = reader.GetInt32(1) == 1; // IsCompleted

                        // Проверка на предыдущий уровень
                        if (level > 1) // Убедитесь, что не проверяете уровень 1
                        {
                            string previousQuery = "SELECT IsCompleted FROM Levels WHERE LevelID = @previousLevelID";
                            using (var previousCommand = new SqliteCommand(previousQuery, connection))
                            {
                                previousCommand.Parameters.AddWithValue("@previousLevelID", level - 1);
                                var previousResult = previousCommand.ExecuteScalar();

                                // Уровень открывается только если предыдущий уровень завершен
                                if (previousResult != null && Convert.ToInt32(previousResult) == 1)
                                {
                                    isActive = true; // Если предыдущий уровень пройден, разблокировать текущий
                                }
                                else
                                {
                                    isActive = false; // Заблокировать текущий уровень
                                }
                            }
                        }
                        else
                        {
                            // Если это первый уровень, его можно разблокировать
                            isActive = isUnlocked;
                        }
                    }
                }
            }
        }

        DecideSprite(); // Обновляем состояние кнопки
        yield return null;
    }
    */
    public void ConfirmPanel(int level)
    {
        PlayerPrefs.SetInt("Current Level", level); // Сохраняем текущий уровень
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
    /*
    public void UnlockLevel()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = "UPDATE Levels SET IsUnlocked = 1 WHERE LevelID = @levelID";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@levelID", level);
                command.ExecuteNonQuery();
            }
        }
    }
    public void LevelCompleted()
    {
        UnlockLevel(); // Обновляем базу данных
        // Дополнительные действия, если необходимо
    }
    */
}
