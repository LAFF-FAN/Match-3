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
    private int starsActive;

    [Header("Level UI")]
    public Image[] stars;
    public TMP_Text levelText;
    public int level;
    public GameObject confirmPanel;

    private GameData gameData; //

    private bool dataLoaded = false;//
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();

        LoadData();
        DecideSprite();
        ActiveStars();
        ShowLevel();
    }
  
    void LoadData()
    {
        if(gameData != null)
        {
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;   
            }
            starsActive = gameData.saveData.stars[level - 1];
        }
    }

    void ActiveStars()
    {
        //BD
        for(int i =0; i< starsActive; i++)
        {
            stars[i].enabled = true;
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
    }
    public void ConfirmPanel(int level)
    {
        PlayerPrefs.SetInt("Current Level", level); // Сохраняем текущий уровень
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}
