using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string currentFighter; 

    public int bossGems; 

    public int clickCoins; 

    public int gameSpeed = 1; 

    public bool showAd; 

    void Start()
    {
        if(currentFighter == "")
        {
            currentFighter = "Egyptian Mage";
        }
    }

}
