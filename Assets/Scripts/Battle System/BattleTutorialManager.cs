using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TutorialStep
{
    [Header("Dialogue")]
    public Sprite characterIcon;

    [TextArea(3, 10)]
    public string dialogueText;

    [Header("Objects")]
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;

    [Header("Animation")]
    public Animator animator;
    public string animationTrigger;

    [Header("Step Events")]
    public UnityEvent onStepStarted;
    public UnityEvent onStepCompleted;

    [Header("Progression")]
    public bool waitForAction;
}

public class BattleTutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject tutorialButtons;

    [SerializeField]
    private Image characterIcon;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private Button nextButton;

    [Header("Tutorial Steps")]
    [SerializeField]
    private TutorialStep[] tutorialSteps;

    [Header("Text Settings")]
    [SerializeField]
    private float typingSpeed = 0.03f;

    private int currentStepIndex = -1;
    private Coroutine typingCoroutine;
    private bool isTyping;
    private bool waitingForAction;

    public bool IsTutorialActive { get; private set; }

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ContinueTutorial);
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(ContinueTutorial);
        }
    }

    private void Update()
    {
        if (!IsTutorialActive || waitingForAction)
        {
            return;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            ContinueTutorial();
        }
    }

    public void StartTutorial()
    {
        if (tutorialSteps == null || tutorialSteps.Length == 0)
        {
            Debug.LogError(
                "BattleTutorialManager: No tutorial steps assigned.",
                this
            );

            return;
        }

        StopTyping();
        tutorialButtons.SetActive(true);
        IsTutorialActive = true;
        currentStepIndex = -1;
        waitingForAction = false;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        ShowNextStep();
    }

    public void ContinueTutorial()
    {
        if (!IsTutorialActive)
        {
            return;
        }

        if (waitingForAction)
        {
            return;
        }

        if (isTyping)
        {
            FinishTyping();
            return;
        }

        CompleteCurrentStep();
        ShowNextStep();
    }

    private void ShowNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Length)
        {
            EndTutorial();
            return;
        }

        TutorialStep step = tutorialSteps[currentStepIndex];

        if (step == null)
        {
            ShowNextStep();
            return;
        }

        waitingForAction = false;

        SetObjectsActive(
            step.objectsToShow,
            true
        );

        SetObjectsActive(
            step.objectsToHide,
            false
        );

        if (characterIcon != null)
        {
            characterIcon.sprite = step.characterIcon;
            characterIcon.enabled = step.characterIcon != null;
        }

        if (step.animator != null &&
            !string.IsNullOrEmpty(step.animationTrigger))
        {
            step.animator.SetTrigger(step.animationTrigger);
        }

        step.onStepStarted?.Invoke();

        typingCoroutine = StartCoroutine(
            TypeLine(step.dialogueText)
        );
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = string.Empty;

        foreach (char character in line)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;

        TutorialStep step = tutorialSteps[currentStepIndex];

        if (step.waitForAction)
        {
            waitingForAction = true;
        }
    }

    private void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        TutorialStep step = tutorialSteps[currentStepIndex];

        dialogueText.text = step.dialogueText;
        isTyping = false;

        if (step.waitForAction)
        {
            waitingForAction = true;
        }
    }

    private void CompleteCurrentStep()
    {
        if (currentStepIndex < 0 ||
            currentStepIndex >= tutorialSteps.Length)
        {
            return;
        }

        TutorialStep step = tutorialSteps[currentStepIndex];

        if (step != null)
        {
            step.onStepCompleted?.Invoke();
        }
    }

    public void AdvanceAfterAction()
    {
        if (!IsTutorialActive || !waitingForAction)
        {
            return;
        }

        waitingForAction = false;
        CompleteCurrentStep();
        ShowNextStep();
    }

    public void EndTutorial()
    {
        StopTyping();

        IsTutorialActive = false;
        waitingForAction = false;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
    }

    private void SetObjectsActive(
        GameObject[] objects,
        bool isActive
    )
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject objectToChange in objects)
        {
            if (objectToChange != null)
            {
                objectToChange.SetActive(isActive);
            }
        }
    }
}