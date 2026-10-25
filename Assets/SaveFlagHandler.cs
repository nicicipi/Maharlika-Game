using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles save flag logic for tracking and responding to triggered game events using unique identifiers.
/// </summary>
/// <remarks>Invokes UnityEvents based on the triggered state of the save flag and provides a method to generate a
/// unique identifier for the flag.</remarks>
public class SaveFlagHandler : MonoBehaviour
{
    public string saveID;
    [SerializeField] private bool checkOnAwake = true;
    [SerializeField] private bool checkOnStart;
    [SerializeField] private bool checkOnEnable;

    public UnityEvent OnAlreadyTriggered;
    public UnityEvent OnNotYetTriggered;
    public UnityEvent OnRuntimeTrigger;

    [ContextMenu("Generate Unique ID")]
    private void GenerateUniqueID()
    {
        saveID = Guid.NewGuid().ToString();
    }
    private void Awake()
    {
        if (!checkOnAwake)
            return;
        
        if (IsTriggered())
            OnAlreadyTriggered?.Invoke();
        else
            OnNotYetTriggered?.Invoke();
    }

    private void OnEnable()
    {
        if (!checkOnEnable)
            return;
        
        if (IsTriggered())
            OnAlreadyTriggered?.Invoke();
        else
            OnNotYetTriggered?.Invoke();
    }

    private void Start()
    {
        if (!checkOnStart)
            return;
        
        if (IsTriggered())
            OnAlreadyTriggered?.Invoke();
        else
            OnNotYetTriggered?.Invoke();
    }


    private bool IsTriggered()
    {
        return PlayerDataHandler.Singleton.IsTriggered(saveID);
    }
    public void TriggerSaveFlag()
    {
        PlayerDataHandler.Singleton.AddToTriggeredFlag(saveID);
        OnRuntimeTrigger?.Invoke();
    }
}
