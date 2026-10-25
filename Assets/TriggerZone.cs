using SaveSystem;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool oneShot = true;
    [SerializeField] private bool disableAfterTrigger = true;

    [SerializeField] private string collisionTag = "Player";

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool _alreadyEntered = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsCorrectCollision(collision))
        {
            return;
        }

        if (oneShot && _alreadyEntered)
        {
            return;
        }

        _alreadyEntered = true;
        onTriggerEnter?.Invoke();

        if (disableAfterTrigger)
        {
            enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsCorrectCollision(collision))
        {
            return;
        }

        onTriggerExit?.Invoke();

        if (!oneShot)
        {
            _alreadyEntered = false;
        }
    }

    private bool IsCorrectCollision(Collider2D collision)
    {
        return string.IsNullOrEmpty(collisionTag) || collision.CompareTag(collisionTag);
    }
}