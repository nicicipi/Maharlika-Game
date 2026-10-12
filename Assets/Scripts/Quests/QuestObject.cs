using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] GameObject objectToActivate;
    [SerializeField] string questToCheck;
    [SerializeField] bool activateIfComplete;

    private void Start()
    {
        CheckForCompletion();
    }

    public void CheckForCompletion()
    {
        if (QuestManager.instance == null || objectToActivate == null)
        {
            return;
        }

        bool questComplete = QuestManager.instance.CheckIfComplete(questToCheck);
        objectToActivate.SetActive(questComplete == activateIfComplete);
    }
}
