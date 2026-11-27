using BattleSystem;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleCharacters : MonoBehaviour
{
    public Sprite deathSprite;
    public Character CharacterData  => _characterData;
    private Character _characterData;
    public List<BattleSkills> battleMovesList = new();
    public bool isPlayer;
    public bool isDead;
    [SerializeReference] private SpriteRenderer _spriteRenderer;

    [Header("VFX")]
    public GameObject deathParticles;
    public Material selectedMaterial;
    public Material normalMaterial;

    [Header("Death VFX (enemy)")]
    [SerializeField] float preDeathDelay = 0.5f;
    [SerializeField] float jitterDuration = 0.6f;
    [SerializeField] float jitterFrequency = 20f;
    [SerializeField] float jitterMagnitudeX = 0.2f;
    [SerializeField] float jitterMagnitudeY = 0.12f; 
    [SerializeField] float colorFlashFrequency = 10f; 
    [SerializeField] float fadeDuration = 0.8f;
    [SerializeField] float fallDistance = 0.5f;

    [Header("Health Slider")]
    public Slider customSlider;
    private Coroutine _sliderCoroutine;

    [Header("Status Effects")]
    [SerializeField] private List<ActiveStatusEffect> _activeStatusEffects = new();

    private Vector3 _initialLocalPosition;
    private UnityAction _onDeathAction;
    private UnityAction _onTakeDamageAction;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _initialLocalPosition = transform.localPosition;
    }
    public  void SelectCharacter(bool triggerSelect)
    {
        _spriteRenderer.material = triggerSelect ? selectedMaterial : normalMaterial;
    }
    public void SetupCharacter(Character characterData, List<BattleSkills> moves, bool isPlayer, Sprite battleSprite, Sprite deadSprite) 
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
        PopulateBattleMoves(moves);
        gameObject.SetActive(true);
        SetupSlider();
        _spriteRenderer.color = Color.white;
    }
    public void PopulateBattleMoves(List<BattleSkills> newMoves)
    {
        battleMovesList.Clear();

        foreach (var move in newMoves)
            battleMovesList.Add(move);
    }
    public void SetupOnTakeDamageAction(UnityAction OnTakeDamage = null)
    {
        _onTakeDamageAction = null;
        _onTakeDamageAction = OnTakeDamage;
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
        _onTakeDamageAction?.Invoke();
        if (CharacterData.stats.currentHP <= 0f)
            KillCharacter();
        RefreshSlider();
    }
    public void TakeHealing()
    {
        RefreshSlider();
        StartCoroutine(HealRoutine(0.5f));
    }
    public void DisplayDamageText(int amount)
    {
        CharacterDamageGUI characterDamageText = Instantiate(BattleManager.instance.characterDamageText, transform.position, Quaternion.identity);
        characterDamageText.SetText(amount);
    }
    public void DisplayHealText(int amount)
    {
        CharacterDamageGUI characterDamageText = Instantiate(BattleManager.instance.characterHealText, transform.position, Quaternion.identity);
        characterDamageText.SetText(amount);
    }
    public void ApplyVFX(GameObject vfxObject,float randomMultiplier = 1f)
    {
        GameObject newObject = Instantiate(vfxObject, transform.position, transform.rotation);
        newObject.transform.position += new Vector3(RandomRange() * randomMultiplier, RandomRange() * randomMultiplier, 0);
        Debug.Log("Applying impact effect: " + newObject.name);
    }

    public void SetupSlider()
    {
        if (customSlider == null)
            return;
        customSlider.minValue = 0;
        customSlider.maxValue = CharacterData.stats.maxHP;
        customSlider.value = CharacterData.stats.currentHP;
    }
    public void RefreshSlider()
    {
        if (customSlider == null) return;

        if (_sliderCoroutine != null)
            StopCoroutine(_sliderCoroutine);

        _sliderCoroutine = StartCoroutine(AnimateSlider());
    }
    private IEnumerator AnimateSlider()
    {
        float currentValue = customSlider.value;
        float newValue = (float)CharacterData.stats.currentHP;

        float duration = 0.25f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            customSlider.value = Mathf.Lerp(currentValue, newValue, t);
            yield return null;
        }

        customSlider.value = newValue;
        _sliderCoroutine = null;
    }
    private float RandomRange()
    {
        return UnityEngine.Random.Range(-3f, 3f);
    }
    public void ForceEndTurn()
    {
        // stuns the character, skipping the turn.
    }
    private IEnumerator HealRoutine(float duration)
    {
        Vector3 originalScale = transform.localScale;
        Color originalColor = transform.GetComponent<SpriteRenderer>().color;
        Color healColor = Color.green;

        SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();

        Vector3 squishScale = new(originalScale.x * 1.5f, originalScale.y * 0.8f, originalScale.z);
        Vector3 stretchScale = new(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            if (t < 0.5f)
                transform.localScale = Vector3.Lerp(squishScale, stretchScale, t * 2f);
            else
                transform.localScale = Vector3.Lerp(stretchScale, originalScale, (t - 0.5f) * 2f);

            spriteRenderer.color = Color.Lerp(healColor, originalColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;
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

    private Coroutine _flickerColorRoutine;
    public void FlickerColor(Color color, float duration, int pingpong)
    {
        if(_flickerColorRoutine != null)
        {
            StopCoroutine(_flickerColorRoutine);
            _flickerColorRoutine = null;
        }
        _spriteRenderer.color = Color.white;
        _flickerColorRoutine = StartCoroutine(FlickerColorRoutine(color, duration, pingpong));
    }
    private IEnumerator FlickerColorRoutine(Color color, float duration, int pingpong)
    {
        if (_spriteRenderer == null)
            yield break;

        Color startColor = _spriteRenderer.color;

        for (int i = 0; i < pingpong; i++)
        {
            _spriteRenderer.color = color;
            yield return new WaitForSeconds(duration / (pingpong * 2));

            _spriteRenderer.color = startColor;
            yield return new WaitForSeconds(duration / (pingpong * 2));
        }

        _spriteRenderer.color = startColor;
        _flickerColorRoutine = null;
    }
    public void AddStatusEffect(StatusEffectData effect, int duration)
    {
        if (_activeStatusEffects.Exists(x => x.data == effect))
            return;

        ActiveStatusEffect newEffect = new(effect, duration);
        _activeStatusEffects.Add(newEffect);

        effect.ApplyStatusEffect(this);

        if (effect.statusEffectVFX)
            ApplyVFX(effect.statusEffectVFX);
    }

    public IEnumerator ProcessStatusEffects(bool turnStart)
    {
        foreach (var effect in _activeStatusEffects.ToList())
        {
            if (turnStart)
            {
                if(effect.data.OnTurnStart(this))
                    yield return new WaitForSeconds(effect.data.duration);
                effect.ReduceDuration();
            }
            else
            {
                if(effect.data.OnTurnEnd(this))
                    yield return new WaitForSeconds(effect.data.duration);
            }

            if (effect.remainingDuration <= 0)
            {
                effect.data.RemoveStatusEffect(this);
                _activeStatusEffects.Remove(effect);
            }
            yield return null;
        }
    }
}
