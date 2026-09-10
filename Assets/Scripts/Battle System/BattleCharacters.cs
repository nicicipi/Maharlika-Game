using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacters : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;

    public string characterName;
    public int currentHP, maxHP, currentSP, maxSP, dexterity, defence, wpnPower, armorDefence;
    public bool isDead;

    public Sprite deadSprites;
    public GameObject deathParticles;

    [Header("Death VFX (enemy)")]
    [SerializeField] float preDeathDelay = 0.5f;
    [SerializeField] float jitterDuration = 0.6f;
    [SerializeField] float jitterFrequency = 20f;
    [SerializeField] float jitterMagnitudeX = 0.2f;
    [SerializeField] float jitterMagnitudeY = 0.12f; // vertical magnitude for diagonal jitter
    [SerializeField] float colorFlashFrequency = 10f; // how fast color flashes between white/black
    [SerializeField] float fadeDuration = 0.8f;
    [SerializeField] float fallDistance = 0.5f; // how far down the sprite moves while fading

    SpriteRenderer spriteRenderer;
    Vector3 initialLocalPosition;

    //private void Update()
    //{
    //    if(!isPlayer && isDead)
    //    {
    //        FadeOutEnemy();
    //    }
    //}

    private void Awake() //if you want a simpler approach to the death sequence, you can use this method instead of the coroutine
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialLocalPosition = transform.localPosition;
    }

    //private void FadeOutEnemy()
    //{
    //    GetComponent<SpriteRenderer>().color = new Color(
    //        Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.r, 1f, 0.3f *Time.deltaTime),
    //        Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.g, 0f, 0.3f *Time.deltaTime),
    //        Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.b, 0f, 0.3f *Time.deltaTime),
    //        Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.a, 0f, 0.3f *Time.deltaTime)
    //        );

    //    if(GetComponent<SpriteRenderer>().color.a == 0f)
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    IEnumerator EnemyDeathSequence()
    {
        // store starting color (preserve alpha)
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        // wait before starting death animation/effects
        if (preDeathDelay > 0f)
            yield return new WaitForSeconds(preDeathDelay);

        // instantiate any death particles immediately after the delay
        if (deathParticles)
            Instantiate(deathParticles, transform.position, transform.rotation);

        // jitter diagonally: top-left <-> top-right repeatedly, while flashing color white<->black
        float elapsed = 0f;
        while (elapsed < jitterDuration)
        {
            // phase increases linearly, convert to full cycles
            float phase = elapsed * jitterFrequency * Mathf.PI * 2f;

            // horizontal oscillation: left <-> right
            float offsetX = Mathf.Sin(phase) * jitterMagnitudeX;

            // vertical stays positive to produce top-left / top-right pattern.
            float offsetY = Mathf.Abs(Mathf.Sin(phase)) * jitterMagnitudeY;

            transform.localPosition = initialLocalPosition + new Vector3(offsetX, offsetY, 0f);

            // color flash between white and black (preserve original alpha)
            float flash = Mathf.PingPong(elapsed * colorFlashFrequency, 1f); // 0..1..0
            Color flashColor = Color.Lerp(Color.white, Color.black, flash);
            flashColor.a = startColor.a;
            if (spriteRenderer != null)
                spriteRenderer.color = flashColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset position
        transform.localPosition = initialLocalPosition;

        // fade out sprite alpha while moving down
        if (spriteRenderer != null)
        {
            Color fadeStart = spriteRenderer.color;
            elapsed = 0f;

            Vector3 startPos = transform.localPosition;
            Vector3 targetPos = startPos + Vector3.down * fallDistance;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                float a = Mathf.Lerp(fadeStart.a, 0f, t);
                spriteRenderer.color = new Color(fadeStart.r, fadeStart.g, fadeStart.b, a);

                // move down over the same duration
                transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // ensure fully transparent and position set
            spriteRenderer.color = new Color(fadeStart.r, fadeStart.g, fadeStart.b, 0f);
            transform.localPosition = targetPos;
        }

        gameObject.SetActive(false);
    }


    public void KillEnemy()
    {
        //isDead = true;

        if (!isDead)
        {
            isDead = true;
            StartCoroutine(EnemyDeathSequence());
        }
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }

    public string[] AttackMovesAvailable()
    {
        return attacksAvailable;
    }

    public void TakeHPDamage(int damageToReceive)
    {
        currentHP -= damageToReceive;

        if(currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public void UseItemInBattle(ItemsManager itemToUse)
    {
        if(itemToUse.affectType == ItemsManager.AffectType.HP)
        {
            AddHP(itemToUse.amountOfAffect);
        }
        else if (itemToUse.affectType == ItemsManager.AffectType.SP)
        {
            AddSP(itemToUse.amountOfAffect);
        }
    }

    private void AddHP(int amountOfAffect)
    {
        currentHP += amountOfAffect;
    }

    private void AddSP(int amountOfAffect)
    {
        currentSP += amountOfAffect;
    }

    public void KillPlayer()
    {
        if (deadSprites)
        {
            //GetComponent<SpriteRenderer>().sprite = deadSprites;
            //Instantiate(deathParticles, transform.position, transform.rotation);
            //isDead = true;

            if (spriteRenderer) spriteRenderer.sprite = deadSprites;
            if (deathParticles) Instantiate(deathParticles, transform.position, transform.rotation);
            isDead = true;
        }
    }
}
