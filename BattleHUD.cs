using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class BattleHUD : MonoBehaviour
{
    public Text nameText; 
    public Text lvlText; 
    public Slider hpSlider; 
    public Text hpText; 

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName; 
        lvlText.text = "Lvl " + unit.lvl;
        hpSlider.maxValue = unit.maxHP; 
        hpSlider.value = unit.currentHP; 
        hpText.text = unit.currentHP + "/" + unit.maxHP;
    }

    public void SetHP(int hp, Unit unit)
    {
        hpSlider.value = hp; 
        hpText.text = unit.currentHP + "/" + unit.maxHP;
    }
}
