using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] string[] questNames;
    [SerializeField] bool[] questMarkersCompleted;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        questMarkersCompleted = new bool[questNames.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print(CheckIfComplete("Defeat a robot"));
            MarkQuestComplete("Get weapon");
            MarkQuestIncomplete("Enter area");
        }
    }

    public int GetQuestNumber(string questToFind)
    {
        for(int i = 0; i < questNames.Length; i++)
        {
            if (questNames[i] == questToFind)
            {
                return i;
            }
        }

        Debug.LogWarning("Quest: " + questToFind + " does not exist");
        return 0;
    }

    public bool CheckIfComplete(string questToCheck)
    {
        int questNumberToCheck = GetQuestNumber(questToCheck);

        if(questNumberToCheck != 0)
        {
            return questMarkersCompleted[questNumberToCheck];
        }

        return false;
    }

    public void UpdateQuestObjects()
    {
        QuestObject[] questObjects = FindObjectsOfType<QuestObject>();

        if(questObjects.Length > 0)
        {
            foreach(QuestObject questObject in questObjects)
            {
                questObject.CheckForCompletion();
            }
        }
    }

    public void MarkQuestComplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);
        questMarkersCompleted[questNumberToCheck] = true;

        UpdateQuestObjects();
    }
    public void MarkQuestIncomplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);
        questMarkersCompleted[questNumberToCheck] = false;

        UpdateQuestObjects();
    }

}
