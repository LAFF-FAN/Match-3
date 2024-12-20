using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;
    public static string currentUsername; //
        public void Awake()
    {
        if(gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
            Load();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {

    }
    public void Save()
    {
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogWarning("��� ������������ �� �����������. ���������� ����������.");
            return;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + currentUsername + "_player.dat"; // ���������� ��� ������������ � ����
        FileStream file = File.Open(path, FileMode.Create);
        formatter.Serialize(file, saveData);
        file.Close();
        Debug.Log("Saved data to: " + path);
    }

    public void Load()
    {
        currentUsername = PlayerPrefs.GetString("Username", string.Empty);
        Debug.Log("Loaded username: " + currentUsername);
        if (string.IsNullOrEmpty(currentUsername))
        {
            Debug.LogWarning("��� ������������ �� �����������. �������� ����������.");
            return;
        }

        string path = Application.persistentDataPath + "/" + currentUsername + "_player.dat"; // ���������� ��� ������������ � ����
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("Loaded data from: " + path);
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + path);
            // ��������������� saveData, ���� ���� �� ������
            saveData = new SaveData
            {
                isActive = new bool[30], // ������ �������������
                highScores = new int[30],
                stars = new int[30]
            };
            // ���������� isActive[0] � true
            if (saveData.isActive.Length > 1)
            {
                saveData.isActive[0] = true;
            }
        }
    }
    private void OnDisaeble()
    {
        Save();
    }
    void Update()
    {

    }
}
