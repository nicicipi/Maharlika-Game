using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] string[] questNames;
    [SerializeField] bool[] questMarkersCompleted;

    [Header("Main Quest Order")]
    [SerializeField] string[] mainQuestNames;

    public static QuestManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (questMarkersCompleted == null ||
            questMarkersCompleted.Length != questNames.Length)
        {
            questMarkersCompleted = new bool[questNames.Length];
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

        Debug.LogWarning("Quest: " + questToFind + " does not exist", this);
        return -1;
    }

    public bool CheckIfComplete(string questToCheck)
    {
        int questNumberToCheck = GetQuestNumber(questToCheck);

        return questNumberToCheck >= 0 &&
               questNumberToCheck < questMarkersCompleted.Length &&
               questMarkersCompleted[questNumberToCheck];
    }

    public bool ArePreviousMainQuestsComplete(string mainQuestToUnlock)
    {
        int mainQuestNumber = GetMainQuestNumber(mainQuestToUnlock);

        if (mainQuestNumber < 0)
        {
            return false;
        }

        for (int i = 0; i < mainQuestNumber; i++)
        {
            if (!CheckIfComplete(mainQuestNames[i]))
            {
                return false;
            }
        }

        return true;
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

        if (questNumberToCheck < 0)
        {
            return;
        }

        questMarkersCompleted[questNumberToCheck] = true;

        UpdateQuestObjects();
    }
    public void MarkQuestIncomplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);

        if (questNumberToCheck < 0)
        {
            return;
        }

        questMarkersCompleted[questNumberToCheck] = false;

        UpdateQuestObjects();
    }

    private int GetMainQuestNumber(string mainQuestToFind)
    {
        if (mainQuestNames == null)
        {
            return -1;
        }

        for (int i = 0; i < mainQuestNames.Length; i++)
        {
            if (mainQuestNames[i] == mainQuestToFind)
            {
                return i;
            }
        }

        Debug.LogWarning(
            "Main quest: " + mainQuestToFind + " does not exist",
            this
        );

        return -1;
    }
     
    public void SaveQuestData()
    {
        for(int i = 0; i < questNames.Length; i++)
        {
            if (questMarkersCompleted[i])
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 1);
            }
            else
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 0);
            }
        }

        PlayerPrefs.Save();
    }

    public void LoadQuestData()
    {
        for (int i = 0; i < questNames.Length; i++)
        {
            int valueToSet = 0;
            string keyToUse = "QuestMarker_" + questNames[i];

            if (PlayerPrefs.HasKey(keyToUse))
            {
                valueToSet = PlayerPrefs.GetInt(keyToUse);
            }

            if (valueToSet == 0)
                questMarkersCompleted[i] = false;
            else
                questMarkersCompleted[i] = true;
        }

        UpdateQuestObjects();
    }

}
