using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public GameObject[] panels;
    private int currentPanelIndex = 0;
    void Start()
    {
        UpdatePanels();
    }
    public void NextPanel()
    {
        currentPanelIndex++;

        if (currentPanelIndex >= panels.Length)
        {
            currentPanelIndex = 0;
        }

        UpdatePanels();
    }

    public void PreviousPanel()
    {
        currentPanelIndex--;

        if (currentPanelIndex < 0)
        {
            currentPanelIndex = panels.Length - 1;
        }

        UpdatePanels();
    }

    private void UpdatePanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentPanelIndex);
        }
    }
}
