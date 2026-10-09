using System.Collections;
using UnityEngine;

public class EnemyNoticeEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject noticePopup;

    [SerializeField]
    private Animator noticeAnimator;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip noticeSound;

    [SerializeField]
    private float displayDuration = 2f;

    [SerializeField]
    private string showTrigger = "showTrigger";

    [SerializeField]
    private string hideTrigger = "hideTrigger";

    private AgentAnimations agentAnimations;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        agentAnimations =
            GetComponentInParent<AgentAnimations>();

        if (noticePopup == null)
        {
            noticePopup = gameObject;
        }

        if (noticeAnimator == null)
        {
            noticeAnimator =
                noticePopup.GetComponent<Animator>();
        }

        HideNotice();
    }

    public void ShowNotice()
    {
        if (agentAnimations != null)
        {
            agentAnimations.FacePlayer();
        }

        if (noticeAnimator != null)
        {
            noticeAnimator.ResetTrigger(hideTrigger);
            noticeAnimator.SetTrigger(showTrigger);
        }

        if (audioSource != null &&
            noticeSound != null)
        {
            audioSource.PlayOneShot(noticeSound);
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(
            HideNoticeAfterDelay()
        );
    }

    private IEnumerator HideNoticeAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        HideNotice();
        hideCoroutine = null;
    }

    public void HideNotice()
    {
        if (noticeAnimator != null)
        {
            noticeAnimator.ResetTrigger(showTrigger);
            noticeAnimator.SetTrigger(hideTrigger);
        }
    }
}