using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    // Start is called before the first frame update
    void Start()
    {
        SetupIntroGoals();
    }

    void SetupIntroGoals()
    {
        for(int i =0; i< levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform); 
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
