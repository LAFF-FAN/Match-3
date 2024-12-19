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
    void Awake()
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
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.dat";
        FileStream file = File.Open(path, FileMode.Create);
        formatter.Serialize(file, saveData);
        file.Close();
        Debug.Log("Saved data to: " + path);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/player.dat";
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
            // Инициализируйте saveData, если файл не найден
            saveData = new SaveData
            {
                isActive = new bool[10], // Пример инициализации
                highScores = new int[10],
                stars = new int[10]
            };
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
