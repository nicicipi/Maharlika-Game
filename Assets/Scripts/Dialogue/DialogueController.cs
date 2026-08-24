using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DialogueController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText, nameText; 
    [SerializeField] GameObject dialogueBox, nameBox;
    [SerializeField] string[]  dialogueSentences;
    [SerializeField] int currentSentence;

    //TYPING EFFECT VARIABLES
    [SerializeField] float typingSpeed = 0.03f; // Speed of each character appearing
    private Coroutine typingCoroutine;          // Tracks the current typing routine
    private bool isTyping; // Tracks if the typewriter is still writing

    public static DialogueController instance;

    private bool dialogueJustStarted;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        // dialogueText.text = dialogueSentences[currentSentence]; ----- (Optional: left handled dynamically on activation)
    }
    //-----------------ORIGINAL CODE JUST SAVING IT --- THIS HAVE THE TYPEWRITER EFFECT BUT SKIPS DIALOGUE WHEN YOU CLICK AGAIN - SO BAD BUT KEEP IT
    //// Update is called once per frame
    //void Update()
    //{
    //    if (dialogueBox.activeInHierarchy)
    //    {
    //        if (Input.GetButtonUp("Fire1"))
    //        {
    //            if (!dialogueJustStarted)
    //            {
    //                currentSentence++;

    //                if (currentSentence >= dialogueSentences.Length)
    //                {
    //                    dialogueBox.SetActive(false);
    //                    GameManager.instance.dialogueBoxOpened = false;
    //                }
    //                else
    //                {
    //                    CheckForName();
    //                    //dialogueText.text = dialogueSentences[currentSentence]; ---- auto adds texts no effects
    //                    if (typingCoroutine != null) StopCoroutine(typingCoroutine);
    //                    typingCoroutine = StartCoroutine(TypeSentence(dialogueSentences[currentSentence]));

    //                }
    //            }

    //            else
    //            {
    //                dialogueJustStarted = false;
    //            }
    //        }
    //    }
    //}

    // Update is called once per frame

    //UPDATED TO shorten dialogue WHEN CLICKING
    void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                if (!dialogueJustStarted)
                {
                    // If still typing, stop the effect and fill the whole sentence immediately
                    if (isTyping)
                    {
                        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                        dialogueText.text = dialogueSentences[currentSentence];
                        isTyping = false;
                    }
                    // If already fully typed, advance to the next sentence
                    else
                    {
                        currentSentence++;

                        if (currentSentence >= dialogueSentences.Length)
                        {
                            dialogueBox.SetActive(false);
                            GameManager.instance.dialogueBoxOpened = false;
                        }
                        else
                        {
                            CheckForName();

                            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                            typingCoroutine = StartCoroutine(TypeSentence(dialogueSentences[currentSentence]));
                        }
                    }
                }
                else
                {
                    dialogueJustStarted = false;
                }
            }
        }
    }

    public void ActivateDialogue(string[] newSentencesToUse)
    {
        dialogueSentences = newSentencesToUse;
        currentSentence = 0;

        CheckForName();
        // dialogueText.text = dialogueSentences[currentSentence]; ----auto adds text no effect
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(dialogueSentences[currentSentence]));


        dialogueBox.SetActive(true);

        dialogueJustStarted = true;
        GameManager.instance.dialogueBoxOpened = true; 
    }

    void CheckForName()
    {
        if(dialogueSentences[currentSentence].StartsWith("#"))
        {
            nameText.text = dialogueSentences[currentSentence].Replace("#", "");
            currentSentence++;

        }
    }

    // not really important  but ADDED: Coroutine that writes out text letter-by-letter  
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true; //added for the shorten dialogue
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
            isTyping = false; // added for the shorten dialogue
    }

    public bool IsDialogueBoxActive()
    {
        return dialogueBox.activeInHierarchy;
    }
}
