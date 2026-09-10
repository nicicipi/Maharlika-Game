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
            GetComponent<SpriteRenderer>().sprite = deadSprites;
            Instantiate(deathParticles, transform.position, transform.rotation);
            isDead = true;
        }
    }
}
