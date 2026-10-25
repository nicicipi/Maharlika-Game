using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleCharacters : MonoBehaviour
{
    public Sprite deathSprite;
    public Character CharacterData  => _characterData;
    private Character _characterData; 
    public List<string> AttackMovesAvailable()
    {
        return CharacterData.moveSets;
    }

    [Header("VFX")]
    public GameObject deathParticles;

    [Header("Death VFX (enemy)")]
    [SerializeField] float preDeathDelay = 0.5f;
    [SerializeField] float jitterDuration = 0.6f;
    [SerializeField] float jitterFrequency = 20f;
    [SerializeField] float jitterMagnitudeX = 0.2f;
    [SerializeField] float jitterMagnitudeY = 0.12f; 
    [SerializeField] float colorFlashFrequency = 10f; 
    [SerializeField] float fadeDuration = 0.8f;
    [SerializeField] float fallDistance = 0.5f;

    [Header("Custom")]
    public Slider customSlider;

    public bool isPlayer;
    public bool isDead;
    [SerializeReference] private SpriteRenderer _spriteRenderer;
    private Vector3 _initialLocalPosition;
    private UnityAction _onDeathAction;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _initialLocalPosition = transform.localPosition;
    }
    public void SetupCharacter(Character characterData, bool isPlayer, Sprite battleSprite, Sprite deadSprite) 
    {
        ClearData();
        if (deadSprite)
        {
            this.deathSprite = deadSprite;
        }
        this.isPlayer = isPlayer;
        this._characterData = characterData;
        _spriteRenderer.sprite = battleSprite;
        isDead = false;

        gameObject.SetActive(true);
        RefreshSlider();
    }
    public void SetupDeathAction(UnityAction OnDeath = null)
    {
        _onDeathAction = OnDeath;
    }
    public void ClearData()
    {
        _onDeathAction = null;
        _spriteRenderer.sprite = null;
        _characterData = null;
        deathSprite = null;
    }

    private IEnumerator EnemyDeathSequence()
    {
        Color startColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;

        if (preDeathDelay > 0f)
            yield return new WaitForSeconds(preDeathDelay);

        if (deathParticles)
            Instantiate(deathParticles, transform.position, transform.rotation);

        float elapsed = 0f;
        while (elapsed < jitterDuration)
        {
            float phase = elapsed * jitterFrequency * Mathf.PI * 2f;
            float offsetX = Mathf.Sin(phase) * jitterMagnitudeX;
            float offsetY = Mathf.Abs(Mathf.Sin(phase)) * jitterMagnitudeY;

            transform.localPosition = _initialLocalPosition + new Vector3(offsetX, offsetY, 0f);
            
            float flash = Mathf.PingPong(elapsed * colorFlashFrequency, 1f); // 0..1..0
            Color flashColor = Color.Lerp(Color.white, Color.black, flash);
            flashColor.a = startColor.a;
            if (_spriteRenderer != null)
                _spriteRenderer.color = flashColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _initialLocalPosition;

        if (_spriteRenderer != null)
        {
            Color fadeStart = _spriteRenderer.color;
            elapsed = 0f;

            Vector3 startPos = transform.localPosition;
            Vector3 targetPos = startPos + Vector3.down * fallDistance;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                float a = Mathf.Lerp(fadeStart.a, 0f, t);
                _spriteRenderer.color = new Color(fadeStart.r, fadeStart.g, fadeStart.b, a);

                transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.color = new Color(fadeStart.r, fadeStart.g, fadeStart.b, 0f);
            transform.localPosition = targetPos;
        }
        gameObject.SetActive(false);
        _spriteRenderer.color = Color.white;
    }

    public void TakeDamage()
    {

        StartCoroutine(TakeDamageRoutine());
        RefreshSlider();
    }
    private void RefreshSlider()
    {
        if (customSlider == null)
            return;
        customSlider.minValue = 0;
        customSlider.maxValue = CharacterData.stats.maxHP;
        customSlider.value = CharacterData.stats.currentHP;
    }
    public IEnumerator TakeDamageRoutine()
    {
        Color startColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;

        float elapsed = 0f;
        while (elapsed < 0.25f)
        {
            float phase = elapsed * jitterFrequency * Mathf.PI * 2f;
            float offsetX = Mathf.Sin(phase) * jitterMagnitudeX;
            float offsetY = Mathf.Abs(Mathf.Sin(phase)) * jitterMagnitudeY;

            transform.localPosition = _initialLocalPosition + new Vector3(offsetX, offsetY, 0f);

            float flash = Mathf.PingPong(elapsed * colorFlashFrequency, 1f); // 0..1..0
            Color flashColor = Color.Lerp(Color.white, Color.red, flash);
            flashColor.a = startColor.a;
            if (_spriteRenderer != null)
                _spriteRenderer.color = flashColor;

            elapsed += Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.color = Color.white;
    }
    public void KillCharacter()
    {
        if (isDead)
        {
            return;
        }

        if (isPlayer)
        {
            KillPlayer();
            return;
        }
        isDead = true;
        _onDeathAction?.Invoke();
        StartCoroutine(EnemyDeathSequence());
    }

    private void KillPlayer()
    {
        if (deathSprite)
        {
            if (_spriteRenderer) _spriteRenderer.sprite = deathSprite;
            if (deathParticles) Instantiate(deathParticles, transform.position, transform.rotation);
            isDead = true;
        }
    }

}
