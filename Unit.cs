using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

public class Unit : MonoBehaviour
{
    public int maxHP;
    public int currentHP; 
    public int attack; 
    public int speed; 
    public int heal; 
    public string unitName;
    public int lvl; 
    public int xp;
    public int maxXP; 
    public bool isCurrent; 
    public bool unlocked; 
    public bool showAd; 

    public Animator animator; 
    
    void Start()
    {

    }

    void Update()
    {
        if(lvl >= 50)
        {
            lvl = 50; 
            maxXP = 40000; 
        }
    }
    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if(currentHP <= 0)
        {
            currentHP = 0;
            return true; 
        }
        else
        {
            return false;
        }
    }

    public void Heal()
    {
        currentHP += heal;
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void LevelUp(int earnedXP)
    {
        if(lvl < 50)
        {
            int xpOverflow = 0; 

            xp = xp + earnedXP; 

            if(xp >= maxXP)
            {
                for(int i=xp;i > maxXP; i--)
                {
                    xpOverflow = xpOverflow + 1; 
                }
                xp = 0 + xpOverflow; 
                if(xp > maxXP)
                {
                    LevelUp(maxXP - xp);
                }
                
                maxXP = maxXP + (maxXP/ 8); 
                lvl = lvl + 1;
                maxHP = maxHP + 3; 
                currentHP = maxHP; 
                heal = heal + 2; 
                attack = attack + 2; 
                speed = speed + 2; 
             
            }
        } 
        else
            return; 
    }

    
}
