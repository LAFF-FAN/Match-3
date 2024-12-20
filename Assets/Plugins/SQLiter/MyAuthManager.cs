using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class MyAuthManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TextMeshProUGUI feedbackText;
    private string dbPath;
    public static bool isLoggedIn = false;
    public static string currentUsername; //
    void Start()
    {
        
        dbPath = "URI=file:" + "C:\\Users\\ПАВЕЛ\\Desktop\\DB\\RegDB.db";
        Debug.Log("Database path: " + dbPath); // Отладочный вывод пути к базе данных
    }
    public void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password != confirmPassword)
        {
            feedbackText.text = "Пароли не совпадают!";
            return;
        }

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Имя пользователя и пароль не могут быть пустыми!";
            return;
        }

        if (password.Length < 8)
        {
            feedbackText.text = "Пароль должен быть минимум 8 символов!";
            return;
        }

        try
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                using (var checkCommand = new SqliteCommand(checkUserQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    int userExists = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (userExists > 0)
                    {
                        feedbackText.text = "Пользователь с таким именем уже существует!";
                        return;
                    }
                }

                string insertQuery = "INSERT INTO Users (Username, Password, RegDate) VALUES (@username, @password, @regDate)";
                using (var insertCommand = new SqliteCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@username", username);
                    insertCommand.Parameters.AddWithValue("@password", password);
                    insertCommand.Parameters.AddWithValue("@regDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    insertCommand.ExecuteNonQuery();
                }

                string getIdQuery = "SELECT UserID FROM Users WHERE Username = @username";
                using (var getIdCommand = new SqliteCommand(getIdQuery, connection))
                {
                    getIdCommand.Parameters.AddWithValue("@username", username);
                    var result = getIdCommand.ExecuteScalar();
                    if (result != null)
                    {
                        int newUserId = Convert.ToInt32(result);
                        PlayerPrefs.SetInt("UserID", newUserId);
                        PlayerPrefs.Save();
                    }
                }

                feedbackText.text = "Регистрация успешна!";
            }
        }
        catch (Exception ex)
        {
            feedbackText.text = "Ошибка регистрации: " + ex.Message;
        }
    }
    public void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Имя пользователя и пароль не могут быть пустыми!";
            return;
        }

        try
        {
            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();

                string loginQuery = "SELECT UserID FROM Users WHERE Username = @username AND Password = @password";
                using (var command = new SqliteCommand(loginQuery, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        int userId = Convert.ToInt32(result);
                        PlayerPrefs.SetInt("UserID", userId);
                        PlayerPrefs.SetString("Username", username);
                        PlayerPrefs.SetInt("IsLoggedIn", 1);
                        PlayerPrefs.Save();

                        Debug.Log(username);
                        feedbackText.text = "Вход выполнен успешно!";
                        isLoggedIn = true;
                    }
                    else
                    {
                        feedbackText.text = "Неправильное имя пользователя или пароль!";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            feedbackText.text = "Ошибка входа: " + ex.Message;
        }
    }
}
