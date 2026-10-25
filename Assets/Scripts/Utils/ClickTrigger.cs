using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ClickTrigger : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;
    public UnityEvent onClick;
    

    void Update()
    {
        if (GameManager.Singleton.CurrentGameState != GameManager.GameState.Roaming || GameManager.Singleton.CurrentPlayingState != GameManager.PlayState.Gameplay)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            if(CheckBox())
                onClick?.Invoke();
        }
    }

    private bool CheckBox()
    {
        return Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0f).Any(c => c.CompareTag("Player"));
    }
}
