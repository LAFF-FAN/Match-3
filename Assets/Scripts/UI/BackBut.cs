using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackBut : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject logPanel;
    public GameObject ConfirmPass;
    public GameObject ConfirmPassText;
    public GameObject BackButt;
    public GameObject LogInButt;
    public GameObject NewBackButt;
    public GameObject RegButt;
    public GameObject CreatButt;
    public GameObject LogInText;
    public GameObject RegText;
    void Start()
    {
    }
    public void Back()
    {
        ConfirmPass.SetActive(false);
        ConfirmPassText.SetActive(false);
        BackButt.SetActive(true);
        LogInButt.SetActive(true);
        NewBackButt.SetActive(false);
        RegButt.SetActive(true);
        CreatButt.SetActive(false);
        LogInText.SetActive(true);
        RegText.SetActive(false);

        startPanel.SetActive(true);
        logPanel.SetActive(false);
    }

    public void BackToLogIn()
    {
        ConfirmPass.SetActive(false);
        ConfirmPassText.SetActive(false);
        BackButt.SetActive(true);
        LogInButt.SetActive(true);
        NewBackButt.SetActive(false);
        RegButt.SetActive(true);
        CreatButt.SetActive(false);
        LogInText.SetActive(true);
        RegText.SetActive(false);
    }
    public void Reg()
    {
        ConfirmPass.SetActive(true);
        ConfirmPassText.SetActive(true);
        BackButt.SetActive(false);
        LogInButt.SetActive(false);
        NewBackButt.SetActive(true);
        RegButt.SetActive(false);
        CreatButt.SetActive(true);
        LogInText.SetActive(false);
        RegText.SetActive(true);

    }

}
