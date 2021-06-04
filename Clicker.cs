using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using BayatGames.SaveGameFree;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements; 

public class Clicker : MonoBehaviour, IUnityAdsListener
{
    Player player;

    public Sprite[] fighterSprites;  
    public GameObject[] fighters; 

    public Button ClickerBtn; 

    public Transform[] spotsTrans; 

    public Text clickTxt; 

    public Text currFighterTxt; 
    public Text hpAtkTxt; 
    public Text SpdHealTxt; 
    public Slider xpSlider; 
    public Text xpTxt; 
    public Text clickerCoinTxt;
    Unit unit; 

    [SerializeField]
    int clickCoins; 
    [SerializeField]
    int clickValue = 1;
    
   
   // SELF CLICK UPGRADE ETC. //
    [SerializeField] 
    int selfClickValue;
    [SerializeField] 
    int selfClick; 
    [SerializeField]
    int selfClickUpgrade = 1;
    [SerializeField] 
    int selfClickUpgradePrice = 250; 

    public Text clickerUpgradeInfoTxt; 
    public Text clickerUpgradePriceTxt; 
    public Text clickerUpgradeLvlTxt; 

    // POWER CLICK UPGRADE ETC. //
    [SerializeField]
    int powerClickUpgrade = 1; 
    [SerializeField]
    int powerClickUpgradePrice = 100; 

    public Text powerClickInfoTxt; 
    public Text powerClickPriceTxt;
    public Text powerClickLvlTxt; 
    
    //GAME SPEED UPGRADE
    int gameSpeedUpgrade = 1; 
    int gameSpeedUpgradePrice = 10000; 

    public Text gameSpeedInfoTxt; 
    public Text gameSpeedPriceTxt; 
    public Text gameSpeedLvlTxt;

    //ATTACK POWER UPGRADE
    [SerializeField]
    int atkPowerUpgrade = 1;
    [SerializeField] 
    int atkPowerUpgradePrice = 5000; 

    public Text atkPowerInfoTxt; 
    public Text atkPowerPriceTxt; 
    public Text atkPowerLvlTxt; 

    //MAX HP UPGRADE
    [SerializeField]
    int maxHPUpgrade = 1; 
    [SerializeField]
    int maxHPUpgradePrice = 5000; 

    public Text maxHPInfoTxt; 
    public Text maxHPPriceTxt; 
    public Text maxHPLvlTxt; 

    //OFFLINE UPGRADE
    [SerializeField]
    int offlineUpgrade = 1; 
    int offlineUpgradePrice = 100000; 
    
    public Text offlineInfoTxt; 
    public Text offlinePriceTxt; 
    public Text offlineLvlTxt; 

    [SerializeField]
    bool upgrading; 

    public Text timeAwayTxt; 
    public Text coinGainsTxt; 
    public Animation offlinePopUp; 

    DateTime currentTime; 

    string placement = "rewardedVideo"; 

    public Text rewardAmtTxt;
    public int rewardAmt;  

    public int overallUpgradeLvl; 

    public Text cursorTxt;
    public Text ringTxt; 

    void Start()
    {
        player = GameObject.Find("HomeSystem").GetComponent<Player>();
        AssignFighter();
        LoadFighter();
        HandleUI();
        AssignUpgradeUI();
        LoadOfflineProduction();
        Time.timeScale = 1; 
        InvokeRepeating("SelfClick", 0.5f, 1f); 
    }

    public void RewardedAd()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize("3922605", true); //game id test mode

        while(!Advertisement.IsReady(placement))
            return;
        
        Advertisement.Show(placement); 
    }

    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(showResult == ShowResult.Finished)
        {
            clickCoins = clickCoins + rewardAmt; 
        }
        else if(showResult == ShowResult.Failed)
        {
            print("failed to watch advertisement");
            return; 
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsReady(string placementId)
    {

    }

    public void LoadOfflineProduction()
    {
        
        if(SaveGame.Load<string>("offlinetime") == "")
            SaveGame.Save<string>("offlinetime", System.DateTime.Now.ToBinary().ToString());

        //offline time management
        if(selfClickUpgrade > 1)
        {
            long tempOfflineTime = Convert.ToInt64(SaveGame.Load<string>("offlinetime")); 
            DateTime oldTime = DateTime.FromBinary(tempOfflineTime); 
            currentTime = DateTime.Now; 

            var difference = currentTime.Subtract(oldTime); 
            var rawTime = (float)difference.TotalSeconds; 
            var offlineTime = rawTime / 10; 

            offlinePopUp.Play("msgBoxAnim"); 
            TimeSpan timer = TimeSpan.FromSeconds(rawTime); 
            timeAwayTxt.text = $"You were away for\n{timer:dd\\:hh\\:mm\\:ss}";
            if(offlineUpgrade == 1)
            {
                var coinsGains = SaveGame.Load<int>("selfclickvalue") * offlineTime;
                clickCoins += (int)coinsGains; 
                coinGainsTxt.text = $"You earned:\n{(int)coinsGains} Coins"; 
            }
            else if(offlineUpgrade == 2)
            {
                var coinsGains = SaveGame.Load<int>("selfclickvalue") * offlineTime * 1.5f;
                clickCoins += (int)coinsGains; 
                coinGainsTxt.text = $"You earned:\n{(int)coinsGains} Coins"; 
            }
            else if(offlineUpgrade == 3)
            {
                var coinsGains = SaveGame.Load<int>("selfclickvalue") * offlineTime * 2f;
                clickCoins += (int)coinsGains; 
                coinGainsTxt.text = $"You earned:\n{(int)coinsGains} Coins"; 
            }
            else if(offlineUpgrade == 4)
            {
                var coinsGains = SaveGame.Load<int>("selfclickvalue") * offlineTime * 3f;
                clickCoins += (int)coinsGains; 
                coinGainsTxt.text = $"You earned:\n{(int)coinsGains} Coins"; 
            }

        }
    }

    public void CloseOffline() 
    {
        offlinePopUp.Play("msgBoxAwayAnim"); 
        currentTime = DateTime.Now; 
        SaveGame.Save<string>("offlinetime", System.DateTime.Now.ToBinary().ToString()); 
    }


    public void ClickerButton()
    {
        int rand = UnityEngine.Random.Range(0,3);
        Instantiate(clickTxt, spotsTrans[rand]);
        clickTxt.text = "+" + clickValue; 
        clickCoins = clickCoins + clickValue; 
    }

    public void AssignFighter()
    {
        if(player.currentFighter == "Egyptian Warrior")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[0];
            unit = fighters[0].GetComponent<Unit>();
        }
        
        if(player.currentFighter == "Egyptian Mage")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[1];
            unit = fighters[1].GetComponent<Unit>();
        }
        
        if(player.currentFighter == "Egyptian Sphinx")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[2];
            unit = fighters[2].GetComponent<Unit>();
        }

        if(player.currentFighter == "Mario")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[3];
            unit = fighters[3].GetComponent<Unit>();
        }

        if(player.currentFighter == "Charizard")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[4];
            unit = fighters[4].GetComponent<Unit>();
        }

        if(player.currentFighter == "Luigi")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[5];
            unit = fighters[5].GetComponent<Unit>();
        }

        if(player.currentFighter == "Link")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[6];
            unit = fighters[6].GetComponent<Unit>();
        }

        if(player.currentFighter == "Mew")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[7];
            unit = fighters[7].GetComponent<Unit>();
        }

        if(player.currentFighter == "Blue Eyes White Dragon")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[8];
            unit = fighters[8].GetComponent<Unit>();
        }
        
        if(player.currentFighter == "Dracula")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[9];
            unit = fighters[9].GetComponent<Unit>();
        }

        if(player.currentFighter == "Bowser")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[10];
            unit = fighters[10].GetComponent<Unit>();
        }

        if(player.currentFighter == "Ganondorf")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[11];
            unit = fighters[11].GetComponent<Unit>();
        }
        
        if(player.currentFighter == "Mewtwo")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[12];
            unit = fighters[12].GetComponent<Unit>();
        }
                
        if(player.currentFighter == "Princess Zelda")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[13];
            unit = fighters[13].GetComponent<Unit>();
        }
                        
        if(player.currentFighter == "Blood Slime")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[14];
            unit = fighters[14].GetComponent<Unit>();
        }
                                
        if(player.currentFighter == "Elven Naturess")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[15];
            unit = fighters[15].GetComponent<Unit>();
        }
                                        
        if(player.currentFighter == "Earth Gem Golem")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[16];
            unit = fighters[16].GetComponent<Unit>();
        }
                                                
        if(player.currentFighter == "Colossal Rex")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[17];
            unit = fighters[17].GetComponent<Unit>();
        }
                                                        
        if(player.currentFighter == "Undeen")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[18];
            unit = fighters[18].GetComponent<Unit>();
        }
                                                                
        if(player.currentFighter == "Ignis")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[19];
            unit = fighters[19].GetComponent<Unit>();
        }
                                                                       
        if(player.currentFighter == "Scorpion")
        {
            ClickerBtn.GetComponent<Image>().sprite = fighterSprites[20];
            unit = fighters[20].GetComponent<Unit>();
        }

        
    }
    
    void Update()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("clickerScene"))
        {
            HandleUI();
            SaveFighter();
            AssignUpgradeUI();
            
            if(clickCoins > 100000000)
            clickCoins = 100000000; 

            
            overallUpgradeLvl = gameSpeedUpgrade + offlineUpgrade + atkPowerUpgrade + maxHPUpgrade + powerClickUpgrade + selfClickUpgrade;
            if(overallUpgradeLvl <= 8 && overallUpgradeLvl > 5)
            {
                rewardAmtTxt.text = "250 COINS"; 
                rewardAmt = 250; 
            }
            if(overallUpgradeLvl <= 12 && overallUpgradeLvl > 8)
            {
                rewardAmtTxt.text = "500 COINS"; 
                rewardAmt = 500; 
            }
            if(overallUpgradeLvl <= 16 && overallUpgradeLvl > 12)
            {
                rewardAmtTxt.text = "1000 COINS";
                rewardAmt = 1000; 
            }
            if(overallUpgradeLvl <= 20 && overallUpgradeLvl > 16)
            {
                rewardAmtTxt.text = "2000 COINS";
                rewardAmt = 2000; 
            }
            if(overallUpgradeLvl <= 25 && overallUpgradeLvl > 20)
            {
                rewardAmtTxt.text = "5000 COINS";
                rewardAmt = 5000; 
            }
            if(overallUpgradeLvl <= 30 && overallUpgradeLvl > 25)
            {
                rewardAmtTxt.text = "8000 COINS";
                rewardAmt = 8000; 
            }
            if(overallUpgradeLvl <= 36 && overallUpgradeLvl > 30)
            {
                rewardAmtTxt.text = "16000 COINS";
                rewardAmt = 16000; 
            }


            cursorTxt.text = "" + selfClickValue + " autoclicks p/s";
            ringTxt.text = "" + clickValue + " coin p/click"; 
            


        }

        player.clickCoins = clickCoins; 
    }


    public void HandleUI()
    {        
        currFighterTxt.text = "Lv." + unit.lvl + " - " + unit.unitName;
        hpAtkTxt.text = "HP: " + unit.maxHP + "     Atk: " + unit.attack;
        SpdHealTxt.text = "Spd: " + unit.speed + "     Heal: " + unit.heal;
        xpSlider.maxValue = unit.maxXP; 
        xpSlider.minValue = 0; 
        xpSlider.value = unit.xp;
        xpTxt.text = unit.xp + "/" + unit.maxXP;
        clickerCoinTxt.text = "" + clickCoins;
    }

    public void LoadFighter()
    {
        if(clickValue == 0)
        {
            clickValue = 1; 
        }
        #region FIGHTER INFO
        if(player.currentFighter == "Egyptian Warrior")
        { 
            if(SaveGame.Exists("[0]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[0]maxhp");

            if(SaveGame.Exists("[0]curhp"))
                unit.currentHP = SaveGame.Load<int>("[0]curhp");

            if(SaveGame.Exists("[0]atk"))
                unit.attack = SaveGame.Load<int>("[0]atk");
            
            if(SaveGame.Exists("[0]spd"))
                unit.speed = SaveGame.Load<int>("[0]spd"); 

            if(SaveGame.Exists("[0]heal"))
                unit.heal = SaveGame.Load<int>("[0]heal"); 

            if(SaveGame.Exists("[0]lvl"))
                unit.lvl = SaveGame.Load<int>("[0]lvl");

            if(SaveGame.Exists("[0]xp"))
                unit.xp = SaveGame.Load<int>("[0]xp");

            if(SaveGame.Exists("[0]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[0]maxxp");

            if(SaveGame.Exists("[0]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[0]unlocked");
        }

        if(player.currentFighter == "Egyptian Mage") 
        {
            if(SaveGame.Exists("[1]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[1]maxhp");

            if(SaveGame.Exists("[1]curhp"))
                unit.currentHP = SaveGame.Load<int>("[1]curhp");

            if(SaveGame.Exists("[1]atk"))
                unit.attack = SaveGame.Load<int>("[1]atk");
            
            if(SaveGame.Exists("[1]spd"))
                unit.speed = SaveGame.Load<int>("[1]spd"); 

            if(SaveGame.Exists("[1]heal"))
                unit.heal = SaveGame.Load<int>("[1]heal"); 

            if(SaveGame.Exists("[1]lvl"))
                unit.lvl = SaveGame.Load<int>("[1]lvl");

            if(SaveGame.Exists("[1]xp"))
                unit.xp = SaveGame.Load<int>("[1]xp");

            if(SaveGame.Exists("[1]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[1]maxxp");

            if(SaveGame.Exists("[1]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[1]unlocked");
        }
        
        if(player.currentFighter == "Egyptian Sphinx")
        {
            if(SaveGame.Exists("[2]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[2]maxhp");

            if(SaveGame.Exists("[2]curhp"))
                unit.currentHP = SaveGame.Load<int>("[2]curhp");

            if(SaveGame.Exists("[2]atk"))
                unit.attack = SaveGame.Load<int>("[2]atk");
            
            if(SaveGame.Exists("[2]spd"))
                unit.speed = SaveGame.Load<int>("[2]spd"); 

            if(SaveGame.Exists("[2]heal"))
                unit.heal = SaveGame.Load<int>("[2]heal"); 

            if(SaveGame.Exists("[2]lvl"))
                unit.lvl = SaveGame.Load<int>("[2]lvl");

            if(SaveGame.Exists("[2]xp"))
                unit.xp = SaveGame.Load<int>("[2]xp");

            if(SaveGame.Exists("[2]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[2]maxxp");

            if(SaveGame.Exists("[2]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[2]unlocked");
        }          
        
        if(player.currentFighter == "Mario")
        {
            if(SaveGame.Exists("[3]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[3]maxhp");

            if(SaveGame.Exists("[3]curhp"))
                unit.currentHP = SaveGame.Load<int>("[3]curhp");

            if(SaveGame.Exists("[3]atk"))
                unit.attack = SaveGame.Load<int>("[3]atk");
            
            if(SaveGame.Exists("[3]spd"))
                unit.speed = SaveGame.Load<int>("[3]spd"); 

            if(SaveGame.Exists("[3]heal"))
                unit.heal = SaveGame.Load<int>("[3]heal"); 

            if(SaveGame.Exists("[3]lvl"))
                unit.lvl = SaveGame.Load<int>("[3]lvl");

            if(SaveGame.Exists("[3]xp"))
                unit.xp = SaveGame.Load<int>("[3]xp");

            if(SaveGame.Exists("[3]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[3]maxxp");

            if(SaveGame.Exists("[3]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[3]unlocked");
        }

        if(player.currentFighter == "Charizard")
        {
            if(SaveGame.Exists("[4]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[4]maxhp");

            if(SaveGame.Exists("[4]curhp"))
                unit.currentHP = SaveGame.Load<int>("[4]curhp");

            if(SaveGame.Exists("[4]atk"))
                unit.attack = SaveGame.Load<int>("[4]atk");
            
            if(SaveGame.Exists("[4]spd"))
                unit.speed = SaveGame.Load<int>("[4]spd"); 

            if(SaveGame.Exists("[4]heal"))
                unit.heal = SaveGame.Load<int>("[4]heal"); 

            if(SaveGame.Exists("[4]lvl"))
                unit.lvl = SaveGame.Load<int>("[4]lvl");

            if(SaveGame.Exists("[4]xp"))
                unit.xp = SaveGame.Load<int>("[4]xp");

            if(SaveGame.Exists("[4]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[4]maxxp");

            if(SaveGame.Exists("[4]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[4]unlocked");
        }

        if(player.currentFighter == "Luigi")
        {
            if(SaveGame.Exists("[5]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[5]maxhp");

            if(SaveGame.Exists("[5]curhp"))
                unit.currentHP = SaveGame.Load<int>("[5]curhp");

            if(SaveGame.Exists("[5]atk"))
                unit.attack = SaveGame.Load<int>("[5]atk");
            
            if(SaveGame.Exists("[5]spd"))
                unit.speed = SaveGame.Load<int>("[5]spd"); 

            if(SaveGame.Exists("[5]heal"))
                unit.heal = SaveGame.Load<int>("[5]heal"); 

            if(SaveGame.Exists("[5]lvl"))
                unit.lvl = SaveGame.Load<int>("[5]lvl");

            if(SaveGame.Exists("[5]xp"))
                unit.xp = SaveGame.Load<int>("[5]xp");

            if(SaveGame.Exists("[5]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[5]maxxp");

            if(SaveGame.Exists("[5]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[5]unlocked");
        }

        if(player.currentFighter == "Link")
        {
            if(SaveGame.Exists("[6]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[6]maxhp");

            if(SaveGame.Exists("[6]curhp"))
                unit.currentHP = SaveGame.Load<int>("[6]curhp");

            if(SaveGame.Exists("[6]atk"))
                unit.attack = SaveGame.Load<int>("[6]atk");
            
            if(SaveGame.Exists("[6]spd"))
                unit.speed = SaveGame.Load<int>("[6]spd"); 

            if(SaveGame.Exists("[6]heal"))
                unit.heal = SaveGame.Load<int>("[6]heal"); 

            if(SaveGame.Exists("[6]lvl"))
                unit.lvl = SaveGame.Load<int>("[6]lvl");

            if(SaveGame.Exists("[6]xp"))
                unit.xp = SaveGame.Load<int>("[6]xp");

            if(SaveGame.Exists("[6]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[6]maxxp");

            if(SaveGame.Exists("[6]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[6]unlocked");
        }

        if(player.currentFighter == "Mew")
        {
            if(SaveGame.Exists("[7]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[7]maxhp");

            if(SaveGame.Exists("[7]curhp"))
                unit.currentHP = SaveGame.Load<int>("[7]curhp");

            if(SaveGame.Exists("[7]atk"))
                unit.attack = SaveGame.Load<int>("[7]atk");
            
            if(SaveGame.Exists("[7]spd"))
                unit.speed = SaveGame.Load<int>("[7]spd"); 

            if(SaveGame.Exists("[7]heal"))
                unit.heal = SaveGame.Load<int>("[7]heal"); 

            if(SaveGame.Exists("[7]lvl"))
                unit.lvl = SaveGame.Load<int>("[7]lvl");

            if(SaveGame.Exists("[7]xp"))
                unit.xp = SaveGame.Load<int>("[7]xp");

            if(SaveGame.Exists("[7]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[7]maxxp");

            if(SaveGame.Exists("[7]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[7]unlocked");
        }

        if(player.currentFighter == "Blue Eyes White Dragon")
        {
            if(SaveGame.Exists("[8]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[8]maxhp");

            if(SaveGame.Exists("[8]curhp"))
                unit.currentHP = SaveGame.Load<int>("[8]curhp");

            if(SaveGame.Exists("[8]atk"))
                unit.attack = SaveGame.Load<int>("[8]atk");
            
            if(SaveGame.Exists("[8]spd"))
                unit.speed = SaveGame.Load<int>("[8]spd"); 

            if(SaveGame.Exists("[8]heal"))
                unit.heal = SaveGame.Load<int>("[8]heal"); 

            if(SaveGame.Exists("[8]lvl"))
                unit.lvl = SaveGame.Load<int>("[8]lvl");

            if(SaveGame.Exists("[8]xp"))
                unit.xp = SaveGame.Load<int>("[8]xp");

            if(SaveGame.Exists("[8]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[8]maxxp");

            if(SaveGame.Exists("[8]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[8]unlocked");
        }

        if(player.currentFighter == "Dracula")
        {
            if(SaveGame.Exists("[9]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[9]maxhp");

            if(SaveGame.Exists("[9]curhp"))
                unit.currentHP = SaveGame.Load<int>("[9]curhp");

            if(SaveGame.Exists("[9]atk"))
                unit.attack = SaveGame.Load<int>("[9]atk");
            
            if(SaveGame.Exists("[9]spd"))
                unit.speed = SaveGame.Load<int>("[9]spd"); 

            if(SaveGame.Exists("[9]heal"))
                unit.heal = SaveGame.Load<int>("[9]heal"); 

            if(SaveGame.Exists("[9]lvl"))
                unit.lvl = SaveGame.Load<int>("[9]lvl");

            if(SaveGame.Exists("[9]xp"))
                unit.xp = SaveGame.Load<int>("[9]xp");

            if(SaveGame.Exists("[9]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[9]maxxp");

            if(SaveGame.Exists("[9]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[9]unlocked");
        }

        if(player.currentFighter == "Bowser")
        {
            if(SaveGame.Exists("[10]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[10]maxhp");

            if(SaveGame.Exists("[10]curhp"))
                unit.currentHP = SaveGame.Load<int>("[10]curhp");

            if(SaveGame.Exists("[10]atk"))
                unit.attack = SaveGame.Load<int>("[10]atk");
            
            if(SaveGame.Exists("[10]spd"))
                unit.speed = SaveGame.Load<int>("[10]spd"); 

            if(SaveGame.Exists("[10]heal"))
                unit.heal = SaveGame.Load<int>("[10]heal"); 

            if(SaveGame.Exists("[10]lvl"))
                unit.lvl = SaveGame.Load<int>("[10]lvl");

            if(SaveGame.Exists("[10]xp"))
                unit.xp = SaveGame.Load<int>("[10]xp");

            if(SaveGame.Exists("[10]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[10]maxxp");

            if(SaveGame.Exists("[10]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[10]unlocked");
        }

        if(player.currentFighter == "Ganondorf")
        {
            if(SaveGame.Exists("[11]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[11]maxhp");

            if(SaveGame.Exists("[11]curhp"))
                unit.currentHP = SaveGame.Load<int>("[11]curhp");

            if(SaveGame.Exists("[11]atk"))
                unit.attack = SaveGame.Load<int>("[11]atk");
            
            if(SaveGame.Exists("[11]spd"))
                unit.speed = SaveGame.Load<int>("[11]spd"); 

            if(SaveGame.Exists("[11]heal"))
                unit.heal = SaveGame.Load<int>("[11]heal"); 

            if(SaveGame.Exists("[11]lvl"))
                unit.lvl = SaveGame.Load<int>("[11]lvl");

            if(SaveGame.Exists("[11]xp"))
                unit.xp = SaveGame.Load<int>("[11]xp");

            if(SaveGame.Exists("[11]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[11]maxxp");

            if(SaveGame.Exists("[11]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[11]unlocked");
        }

        if(player.currentFighter == "Mewtwo")
        {
            if(SaveGame.Exists("[12]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[12]maxhp");

            if(SaveGame.Exists("[12]curhp"))
                unit.currentHP = SaveGame.Load<int>("[12]curhp");

            if(SaveGame.Exists("[12]atk"))
                unit.attack = SaveGame.Load<int>("[12]atk");
            
            if(SaveGame.Exists("[12]spd"))
                unit.speed = SaveGame.Load<int>("[12]spd"); 

            if(SaveGame.Exists("[12]heal"))
                unit.heal = SaveGame.Load<int>("[12]heal"); 

            if(SaveGame.Exists("[12]lvl"))
                unit.lvl = SaveGame.Load<int>("[12]lvl");

            if(SaveGame.Exists("[12]xp"))
                unit.xp = SaveGame.Load<int>("[12]xp");

            if(SaveGame.Exists("[12]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[12]maxxp");

            if(SaveGame.Exists("[12]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[12]unlocked");
        }

        if(player.currentFighter == "Princess Zelda")
        {
            if(SaveGame.Exists("[13]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[13]maxhp");

            if(SaveGame.Exists("[13]curhp"))
                unit.currentHP = SaveGame.Load<int>("[13]curhp");

            if(SaveGame.Exists("[13]atk"))
                unit.attack = SaveGame.Load<int>("[13]atk");
            
            if(SaveGame.Exists("[13]spd"))
                unit.speed = SaveGame.Load<int>("[13]spd"); 

            if(SaveGame.Exists("[13]heal"))
                unit.heal = SaveGame.Load<int>("[13]heal"); 

            if(SaveGame.Exists("[13]lvl"))
                unit.lvl = SaveGame.Load<int>("[13]lvl");

            if(SaveGame.Exists("[13]xp"))
                unit.xp = SaveGame.Load<int>("[13]xp");

            if(SaveGame.Exists("[13]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[13]maxxp");

            if(SaveGame.Exists("[13]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[13]unlocked");
        }
        
        if(player.currentFighter == "Blood Slime")
        {
            if(SaveGame.Exists("[14]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[14]maxhp");

            if(SaveGame.Exists("[14]curhp"))
                unit.currentHP = SaveGame.Load<int>("[14]curhp");

            if(SaveGame.Exists("[14]atk"))
                unit.attack = SaveGame.Load<int>("[14]atk");
            
            if(SaveGame.Exists("[14]spd"))
                unit.speed = SaveGame.Load<int>("[14]spd"); 

            if(SaveGame.Exists("[14]heal"))
                unit.heal = SaveGame.Load<int>("[14]heal"); 

            if(SaveGame.Exists("[14]lvl"))
                unit.lvl = SaveGame.Load<int>("[14]lvl");

            if(SaveGame.Exists("[14]xp"))
                unit.xp = SaveGame.Load<int>("[14]xp");

            if(SaveGame.Exists("[14]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[14]maxxp");

            if(SaveGame.Exists("[14]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[14]unlocked");
        }
                
        if(player.currentFighter == "Elven Naturess")
        {
            if(SaveGame.Exists("[15]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[15]maxhp");

            if(SaveGame.Exists("[15]curhp"))
                unit.currentHP = SaveGame.Load<int>("[15]curhp");

            if(SaveGame.Exists("[15]atk"))
                unit.attack = SaveGame.Load<int>("[15]atk");
            
            if(SaveGame.Exists("[15]spd"))
                unit.speed = SaveGame.Load<int>("[15]spd"); 

            if(SaveGame.Exists("[15]heal"))
                unit.heal = SaveGame.Load<int>("[15]heal"); 

            if(SaveGame.Exists("[15]lvl"))
                unit.lvl = SaveGame.Load<int>("[15]lvl");

            if(SaveGame.Exists("[15]xp"))
                unit.xp = SaveGame.Load<int>("[15]xp");

            if(SaveGame.Exists("[15]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[15]maxxp");

            if(SaveGame.Exists("[15]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[15]unlocked");
        }
                       
        if(player.currentFighter == "Earth Gem Golem")
        {
            if(SaveGame.Exists("[16]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[16]maxhp");

            if(SaveGame.Exists("[16]curhp"))
                unit.currentHP = SaveGame.Load<int>("[16]curhp");

            if(SaveGame.Exists("[16]atk"))
                unit.attack = SaveGame.Load<int>("[16]atk");
            
            if(SaveGame.Exists("[16]spd"))
                unit.speed = SaveGame.Load<int>("[16]spd"); 

            if(SaveGame.Exists("[16]heal"))
                unit.heal = SaveGame.Load<int>("[16]heal"); 

            if(SaveGame.Exists("[16]lvl"))
                unit.lvl = SaveGame.Load<int>("[16]lvl");

            if(SaveGame.Exists("[16]xp"))
                unit.xp = SaveGame.Load<int>("[16]xp");

            if(SaveGame.Exists("[16]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[16]maxxp");

            if(SaveGame.Exists("[16]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[16]unlocked");
        }
                                       
        if(player.currentFighter == "Colossal Rex")
        {
            if(SaveGame.Exists("[17]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[17]maxhp");

            if(SaveGame.Exists("[17]curhp"))
                unit.currentHP = SaveGame.Load<int>("[17]curhp");

            if(SaveGame.Exists("[17]atk"))
                unit.attack = SaveGame.Load<int>("[17]atk");
            
            if(SaveGame.Exists("[17]spd"))
                unit.speed = SaveGame.Load<int>("[17]spd"); 

            if(SaveGame.Exists("[17]heal"))
                unit.heal = SaveGame.Load<int>("[17]heal"); 

            if(SaveGame.Exists("[17]lvl"))
                unit.lvl = SaveGame.Load<int>("[17]lvl");

            if(SaveGame.Exists("[17]xp"))
                unit.xp = SaveGame.Load<int>("[17]xp");

            if(SaveGame.Exists("[17]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[17]maxxp");

            if(SaveGame.Exists("[17]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[17]unlocked");
        }
    
        if(player.currentFighter == "Undeen")
        {
            if(SaveGame.Exists("[18]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[18]maxhp");

            if(SaveGame.Exists("[18]curhp"))
                unit.currentHP = SaveGame.Load<int>("[18]curhp");

            if(SaveGame.Exists("[18]atk"))
                unit.attack = SaveGame.Load<int>("[18]atk");
            
            if(SaveGame.Exists("[18]spd"))
                unit.speed = SaveGame.Load<int>("[18]spd"); 

            if(SaveGame.Exists("[18]heal"))
                unit.heal = SaveGame.Load<int>("[18]heal"); 

            if(SaveGame.Exists("[18]lvl"))
                unit.lvl = SaveGame.Load<int>("[18]lvl");

            if(SaveGame.Exists("[18]xp"))
                unit.xp = SaveGame.Load<int>("[18]xp");

            if(SaveGame.Exists("[18]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[18]maxxp");

            if(SaveGame.Exists("[18]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[18]unlocked");
        }
                
        if(player.currentFighter == "Ignis")
        {
            if(SaveGame.Exists("[19]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[19]maxhp");

            if(SaveGame.Exists("[19]curhp"))
                unit.currentHP = SaveGame.Load<int>("[19]curhp");

            if(SaveGame.Exists("[19]atk"))
                unit.attack = SaveGame.Load<int>("[19]atk");
            
            if(SaveGame.Exists("[19]spd"))
                unit.speed = SaveGame.Load<int>("[19]spd"); 

            if(SaveGame.Exists("[19]heal"))
                unit.heal = SaveGame.Load<int>("[19]heal"); 

            if(SaveGame.Exists("[19]lvl"))
                unit.lvl = SaveGame.Load<int>("[19]lvl");

            if(SaveGame.Exists("[19]xp"))
                unit.xp = SaveGame.Load<int>("[19]xp");

            if(SaveGame.Exists("[19]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[19]maxxp");

            if(SaveGame.Exists("[19]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[19]unlocked");
        }
                                
        if(player.currentFighter == "Scorpion")
        {
            if(SaveGame.Exists("[20]maxhp"))
                unit.maxHP = SaveGame.Load<int>("[20]maxhp");

            if(SaveGame.Exists("[20]curhp"))
                unit.currentHP = SaveGame.Load<int>("[20]curhp");

            if(SaveGame.Exists("[20]atk"))
                unit.attack = SaveGame.Load<int>("[20]atk");
            
            if(SaveGame.Exists("[20]spd"))
                unit.speed = SaveGame.Load<int>("[20]spd"); 

            if(SaveGame.Exists("[20]heal"))
                unit.heal = SaveGame.Load<int>("[20]heal"); 

            if(SaveGame.Exists("[20]lvl"))
                unit.lvl = SaveGame.Load<int>("[20]lvl");

            if(SaveGame.Exists("[20]xp"))
                unit.xp = SaveGame.Load<int>("[20]xp");

            if(SaveGame.Exists("[20]maxxp"))
                unit.maxXP = SaveGame.Load<int>("[20]maxxp");

            if(SaveGame.Exists("[20]unlocked"))
                unit.unlocked = SaveGame.Load<bool>("[20]unlocked");
        }

        #endregion
        
        if(SaveGame.Exists("clickcoins"))
            clickCoins = SaveGame.Load<int>("clickcoins");
        
        if(SaveGame.Exists("clickvalue"))
            clickValue = SaveGame.Load<int>("clickvalue");

        if(SaveGame.Exists("selfclickvalue"))
            selfClickValue = SaveGame.Load<int>("selfclickvalue");

        if(SaveGame.Exists("selfclick"))
            selfClick = SaveGame.Load<int>("selfclick");

        if(SaveGame.Exists("selfclickupgrade"))
            selfClickUpgrade = SaveGame.Load<int>("selfclickupgrade");
        
        if(SaveGame.Exists("selfclickupgradeprice"))
            selfClickUpgradePrice = SaveGame.Load<int>("selfclickupgradeprice");

        if(SaveGame.Exists("powerclickupgrade"))
            powerClickUpgrade = SaveGame.Load<int>("powerclickupgrade");

        if(SaveGame.Exists("powerclickupgradeprice"))
            powerClickUpgradePrice = SaveGame.Load<int>("powerclickupgradeprice");

        if(SaveGame.Exists("powerclickupgradeprice"))
            player.bossGems = SaveGame.Load<int>("bossgems");
                    
        if(SaveGame.Exists("gamespeed"))
            player.gameSpeed = SaveGame.Load<int>("gamespeed");

        if(SaveGame.Exists("gamespeedupgrade"))
            gameSpeedUpgrade = SaveGame.Load<int>("gamespeedupgrade");

        if(SaveGame.Exists("gamespeedupgradeprice"))
            gameSpeedUpgradePrice = SaveGame.Load<int>("gamespeedupgradeprice");

        if(SaveGame.Exists("offlineupgrade"))
            offlineUpgrade = SaveGame.Load<int>("offlineupgrade"); 
        
        if(SaveGame.Exists("offlineupgradeprice"))
            offlineUpgradePrice = SaveGame.Load<int>("offlineupgradeprice"); 

        
        #region FIGHTERINFO

        if(player.currentFighter == "Egyptian Warrior")
        {
            if(SaveGame.Exists("[0]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[0]atkpowerupgrade");
            
            if(SaveGame.Exists("[0]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[0]atkpowerupgradeprice");

            if(SaveGame.Exists("[0]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[0]maxhpupgrade");

            if(SaveGame.Exists("[0]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[0]maxhpupgradeprice");
        }

        if(player.currentFighter == "Egyptian Mage")
        {
            if(SaveGame.Exists("[1]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[1]atkpowerupgrade");

            if(SaveGame.Exists("[1]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[1]atkpowerupgradeprice");

            if(SaveGame.Exists("[1]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[1]maxhpupgrade");

            if(SaveGame.Exists("[1]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[1]maxhpupgradeprice");
        }
        
        if(player.currentFighter == "Egyptian Sphinx")
        {
            if(SaveGame.Exists("[2]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[2]atkpowerupgrade"); 

            if(SaveGame.Exists("[2]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[2]atkpowerupgradeprice");

            if(SaveGame.Exists("[2]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[2]maxhpupgrade");

            if(SaveGame.Exists("[2]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[2]maxhpupgradeprice");
        }

        if(player.currentFighter == "Mario")
        {
            if(SaveGame.Exists("[3]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[3]atkpowerupgrade");

            if(SaveGame.Exists("[3]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[3]atkpowerupgradeprice");

            if(SaveGame.Exists("[3]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[3]maxhpupgrade");

            if(SaveGame.Exists("[3]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[3]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Charizard")
        {
            if(SaveGame.Exists("[4]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[4]atkpowerupgrade");

            if(SaveGame.Exists("[4]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[4]atkpowerupgradeprice");

            if(SaveGame.Exists("[4]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[4]maxhpupgrade");

            if(SaveGame.Exists("[4]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[4]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Luigi")
        {
            if(SaveGame.Exists("[5]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[5]atkpowerupgrade");

            if(SaveGame.Exists("[5]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[5]atkpowerupgradeprice");

            if(SaveGame.Exists("[5]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[5]maxhpupgrade");

            if(SaveGame.Exists("[5]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[5]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Link")
        {
            if(SaveGame.Exists("[6]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[6]atkpowerupgrade");

            if(SaveGame.Exists("[6]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[6]atkpowerupgradeprice");

            if(SaveGame.Exists("[6]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[6]maxhpupgrade");

            if(SaveGame.Exists("[6]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[6]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Mew")
        {
            if(SaveGame.Exists("[7]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[7]atkpowerupgrade");

            if(SaveGame.Exists("[7]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[7]atkpowerupgradeprice");

            if(SaveGame.Exists("[7]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[7]maxhpupgrade");

            if(SaveGame.Exists("[7]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[7]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Blue Eyes White Dragon")
        {
            if(SaveGame.Exists("[8]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[8]atkpowerupgrade");

            if(SaveGame.Exists("[8]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[8]atkpowerupgradeprice");

            if(SaveGame.Exists("[8]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[8]maxhpupgrade");

            if(SaveGame.Exists("[8]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[8]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Dracula")
        {
            if(SaveGame.Exists("[9]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[9]atkpowerupgrade");

            if(SaveGame.Exists("[9]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[9]atkpowerupgradeprice");

            if(SaveGame.Exists("[9]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[9]maxhpupgrade");

            if(SaveGame.Exists("[9]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[9]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Bowser")
        {
            if(SaveGame.Exists("[10]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[10]atkpowerupgrade");

            if(SaveGame.Exists("[10]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[10]atkpowerupgradeprice");

            if(SaveGame.Exists("[10]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[10]maxhpupgrade");

            if(SaveGame.Exists("[10]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[10]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Ganondorf")
        {
            if(SaveGame.Exists("[11]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[11]atkpowerupgrade");

            if(SaveGame.Exists("[11]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[11]atkpowerupgradeprice");

            if(SaveGame.Exists("[11]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[11]maxhpupgrade");

            if(SaveGame.Exists("[11]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[11]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Mewtwo")
        {
            if(SaveGame.Exists("[12]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[12]atkpowerupgrade");

            if(SaveGame.Exists("[12]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[12]atkpowerupgradeprice");

            if(SaveGame.Exists("[12]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[12]maxhpupgrade");

            if(SaveGame.Exists("[12]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[12]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Princess Zelda")
        {
            if(SaveGame.Exists("[13]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[13]atkpowerupgrade");

            if(SaveGame.Exists("[13]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[13]atkpowerupgradeprice");

            if(SaveGame.Exists("[13]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[13]maxhpupgrade");

            if(SaveGame.Exists("[13]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[13]maxhpupgradeprice"); 
        }

        
        if(player.currentFighter == "Blood Slime")
        {
            if(SaveGame.Exists("[14]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[14]atkpowerupgrade");

            if(SaveGame.Exists("[14]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[14]atkpowerupgradeprice");

            if(SaveGame.Exists("[14]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[14]maxhpupgrade");

            if(SaveGame.Exists("[14]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[14]maxhpupgradeprice"); 
        }

                
        if(player.currentFighter == "Elven Naturess")
        {
            if(SaveGame.Exists("[15]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[15]atkpowerupgrade");

            if(SaveGame.Exists("[15]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[15]atkpowerupgradeprice");

            if(SaveGame.Exists("[15]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[15]maxhpupgrade");

            if(SaveGame.Exists("[15]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[15]maxhpupgradeprice"); 
        }
                  
        if(player.currentFighter == "Earth Gem Golem")
        {
            if(SaveGame.Exists("[16]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[16]atkpowerupgrade");

            if(SaveGame.Exists("[16]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[16]atkpowerupgradeprice");

            if(SaveGame.Exists("[16]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[16]maxhpupgrade");

            if(SaveGame.Exists("[16]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[16]maxhpupgradeprice"); 
        }
                           
        if(player.currentFighter == "Colossal Rex")
        {
            if(SaveGame.Exists("[17]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[17]atkpowerupgrade");

            if(SaveGame.Exists("[17]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[17]atkpowerupgradeprice");

            if(SaveGame.Exists("[17]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[17]maxhpupgrade");

            if(SaveGame.Exists("[17]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[17]maxhpupgradeprice"); 
        }

        if(player.currentFighter == "Undeen")
        {
            if(SaveGame.Exists("[18]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[18]atkpowerupgrade");

            if(SaveGame.Exists("[18]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[18]atkpowerupgradeprice");

            if(SaveGame.Exists("[18]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[18]maxhpupgrade");

            if(SaveGame.Exists("[18]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[18]maxhpupgradeprice"); 
        }
        
        if(player.currentFighter == "Ignis")
        {
            if(SaveGame.Exists("[19]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[19]atkpowerupgrade");

            if(SaveGame.Exists("[19]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[19]atkpowerupgradeprice");

            if(SaveGame.Exists("[19]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[19]maxhpupgrade");

            if(SaveGame.Exists("[19]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[19]maxhpupgradeprice"); 
        }
                
        if(player.currentFighter == "Scorpion")
        {
            if(SaveGame.Exists("[20]atkpowerupgrade"))
                atkPowerUpgrade = SaveGame.Load<int>("[20]atkpowerupgrade");

            if(SaveGame.Exists("[20]atkpowerupgradeprice"))
                atkPowerUpgradePrice = SaveGame.Load<int>("[20]atkpowerupgradeprice");

            if(SaveGame.Exists("[20]maxhpupgrade"))
                maxHPUpgrade = SaveGame.Load<int>("[20]maxhpupgrade");

            if(SaveGame.Exists("[20]maxhpupgradeprice"))
                maxHPUpgradePrice = SaveGame.Load<int>("[20]maxhpupgradeprice"); 
        }

        #endregion
    }

    public void SaveFighter()
    {
        SaveGame.Save<int>("clickcoins", clickCoins);
        SaveGame.Save<int>("clickvalue", clickValue);
        SaveGame.Save<int>("selfclickvalue", selfClickValue);
        SaveGame.Save<int>("selfclick", selfClick);
        SaveGame.Save<int>("selfclickupgrade", selfClickUpgrade);
        SaveGame.Save<int>("selfclickupgradeprice", selfClickUpgradePrice);
        SaveGame.Save<int>("powerclickupgrade", powerClickUpgrade);
        SaveGame.Save<int>("powerclickupgradeprice", powerClickUpgradePrice);
        SaveGame.Save<int>("bossgems", player.bossGems);
        SaveGame.Save<int>("gamespeed", player.gameSpeed);
        SaveGame.Save<int>("gamespeedupgrade", gameSpeedUpgrade); 
        SaveGame.Save<int>("gamespeedupgradeprice", gameSpeedUpgradePrice); 
        SaveGame.Save<int>("offlineupgrade", offlineUpgrade); 
        SaveGame.Save<int>("offlineupgradeprice", offlineUpgradePrice); 

        if(player.currentFighter == "Egyptian Warrior")
        {
            SaveGame.Save<int>("[0]atkpowerupgrade", atkPowerUpgrade);
            SaveGame.Save<int>("[0]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[0]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[0]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[0]atk", unit.attack);
            SaveGame.Save<int>("[0]maxhp", unit.maxHP);
        }
        
        if(player.currentFighter == "Egyptian Mage")
        {
            SaveGame.Save<int>("[1]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[1]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[1]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[1]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[1]atk", unit.attack);
            SaveGame.Save<int>("[1]maxhp", unit.maxHP);
        }

        
        if(player.currentFighter == "Egyptian Sphinx")
        {
            SaveGame.Save<int>("[2]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[2]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[2]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[2]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[2]atk", unit.attack);
            SaveGame.Save<int>("[2]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Mario")
        {
            SaveGame.Save<int>("[3]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[3]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[3]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[3]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[3]atk", unit.attack);
            SaveGame.Save<int>("[3]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Charizard")
        {
            SaveGame.Save<int>("[4]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[4]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[4]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[4]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[4]atk", unit.attack);
            SaveGame.Save<int>("[4]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Luigi")
        {
            SaveGame.Save<int>("[5]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[5]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[5]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[5]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[5]atk", unit.attack);
            SaveGame.Save<int>("[5]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Link")
        {
            SaveGame.Save<int>("[6]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[6]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[6]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[6]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[6]atk", unit.attack);
            SaveGame.Save<int>("[6]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Mew")
        {
            SaveGame.Save<int>("[7]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[7]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[7]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[7]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[7]atk", unit.attack);
            SaveGame.Save<int>("[7]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Blue Eyes White Dragon")
        {
            SaveGame.Save<int>("[8]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[8]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[8]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[8]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[8]atk", unit.attack);
            SaveGame.Save<int>("[8]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Dracula")
        {
            SaveGame.Save<int>("[9]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[9]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[9]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[9]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[9]atk", unit.attack);
            SaveGame.Save<int>("[9]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Bowser")
        {
            SaveGame.Save<int>("[10]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[10]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[10]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[10]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[10]atk", unit.attack);
            SaveGame.Save<int>("[10]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Ganondorf")
        {
            SaveGame.Save<int>("[11]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[11]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[11]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[11]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[11]atk", unit.attack);
            SaveGame.Save<int>("[11]maxhp", unit.maxHP);
        }

        if(player.currentFighter == "Mewtwo")
        {
            SaveGame.Save<int>("[12]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[12]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[12]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[12]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[12]atk", unit.attack);
            SaveGame.Save<int>("[12]maxhp", unit.maxHP);
        }
        
        if(player.currentFighter == "Princess Zelda")
        {
            SaveGame.Save<int>("[13]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[13]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[13]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[13]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[13]atk", unit.attack);
            SaveGame.Save<int>("[13]maxhp", unit.maxHP);
        }
                
        if(player.currentFighter == "Blood Slime")
        {
            SaveGame.Save<int>("[14]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[14]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[14]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[14]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[14]atk", unit.attack);
            SaveGame.Save<int>("[14]maxhp", unit.maxHP);
        }
                        
        if(player.currentFighter == "Elven Naturess")
        {
            SaveGame.Save<int>("[15]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[15]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[15]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[15]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[15]atk", unit.attack);
            SaveGame.Save<int>("[15]maxhp", unit.maxHP);
        }
                                
        if(player.currentFighter == "Earth Gem Golem")
        {
            SaveGame.Save<int>("[16]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[16]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[16]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[16]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[16]atk", unit.attack);
            SaveGame.Save<int>("[16]maxhp", unit.maxHP);
        }
                                        
        if(player.currentFighter == "Colossal Rex")
        {
            SaveGame.Save<int>("[17]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[17]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[17]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[17]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[17]atk", unit.attack);
            SaveGame.Save<int>("[17]maxhp", unit.maxHP);
        }
                                                
        if(player.currentFighter == "Undeen")
        {
            SaveGame.Save<int>("[18]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[18]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[18]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[18]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[18]atk", unit.attack);
            SaveGame.Save<int>("[18]maxhp", unit.maxHP);
        }
                                                        
        if(player.currentFighter == "Ignis")
        {
            SaveGame.Save<int>("[19]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[19]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[19]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[19]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[19]atk", unit.attack);
            SaveGame.Save<int>("[19]maxhp", unit.maxHP);
        }
                                                               
        if(player.currentFighter == "Scorpion")
        {
            SaveGame.Save<int>("[20]atkpowerupgrade", atkPowerUpgrade); 
            SaveGame.Save<int>("[20]atkpowerupgradeprice", atkPowerUpgradePrice);
            SaveGame.Save<int>("[20]maxhpupgrade", maxHPUpgrade); 
            SaveGame.Save<int>("[20]maxhpupgradeprice", maxHPUpgradePrice);
            SaveGame.Save<int>("[20]atk", unit.attack);
            SaveGame.Save<int>("[20]maxhp", unit.maxHP);
        }


    }

    void SelfClick()
    {
        if(selfClickValue > 0)
        {
            int rand = UnityEngine.Random.Range(0,3);
            Instantiate(clickTxt, spotsTrans[rand]);
            clickTxt.text = "+" + selfClickValue; 
            clickCoins = clickCoins + selfClickValue;
        }
        else
        {
            return; 
        }
    }

    public void ClickUpgrade()
    {
        if(selfClickUpgrade == 1 && !upgrading)
        {
            upgrading = true; 
            selfClickUpgradePrice = 250; 

            if(clickCoins >= selfClickUpgradePrice)
            {
                selfClickValue = 1; 
                clickCoins = clickCoins - selfClickUpgradePrice; 
                selfClickUpgradePrice = selfClickUpgradePrice * 2; 
                selfClickUpgrade = 2;
                AssignUpgradeUI();   
            }
            StartCoroutine(WaitUpgrades());
        }
        
        if(selfClickUpgrade == 2 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= selfClickUpgradePrice)
            {
                selfClickValue = 2; 
                clickCoins = clickCoins - selfClickUpgradePrice; 
                selfClickUpgradePrice = selfClickUpgradePrice * 2; 
                selfClickUpgrade = 3;
                AssignUpgradeUI();  
            }
            StartCoroutine(WaitUpgrades());
        }
        
        if(selfClickUpgrade == 3 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= selfClickUpgradePrice)
            {
                selfClickValue = 3; 
                clickCoins = clickCoins - selfClickUpgradePrice; 
                selfClickUpgradePrice = selfClickUpgradePrice * 2; 
                selfClickUpgrade = 4;
                AssignUpgradeUI(); 
            }
            StartCoroutine(WaitUpgrades());
        }
                
        if(selfClickUpgrade == 4 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= selfClickUpgradePrice)
            {
                selfClickValue = 4; 
                clickCoins = clickCoins - selfClickUpgradePrice; 
                selfClickUpgradePrice = selfClickUpgradePrice * 4; 
                selfClickUpgrade = 5;
                AssignUpgradeUI(); 
            }
            StartCoroutine(WaitUpgrades());
        }
                        
        if(selfClickUpgrade == 5 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= selfClickUpgradePrice)
            {
                selfClickValue = 8; 
                clickCoins = clickCoins - selfClickUpgradePrice; 
                selfClickUpgradePrice = selfClickUpgradePrice * 5; 
                selfClickUpgrade = 6;
                AssignUpgradeUI(); 
            }
            StartCoroutine(WaitUpgrades());
        }
                                
        if(selfClickUpgrade == 6 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= selfClickUpgradePrice)
            {
                selfClickValue = 16; 
                clickCoins = clickCoins - selfClickUpgradePrice; 
                selfClickUpgradePrice = selfClickUpgradePrice * 6; 
                selfClickUpgrade = 7;
                AssignUpgradeUI(); 
            }
            StartCoroutine(WaitUpgrades());
        }
    }

    public void PowerClickUpgrade()
    { 

        if(powerClickUpgrade == 1 && !upgrading)
        {
            upgrading = true; 
            if(clickCoins >= powerClickUpgradePrice)
            {
                clickValue = clickValue * 2;  
                clickCoins = clickCoins - powerClickUpgradePrice; 
                powerClickUpgradePrice = powerClickUpgradePrice * 3; 
                powerClickUpgrade = 2; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }

        if(powerClickUpgrade == 2 && !upgrading)
        {
            upgrading = true; 
            if(clickCoins >= powerClickUpgradePrice)
            {
                clickValue = clickValue * 2;  
                clickCoins = clickCoins - powerClickUpgradePrice; 
                powerClickUpgradePrice = powerClickUpgradePrice * 3; 
                powerClickUpgrade = 3; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }

        if(powerClickUpgrade == 3 && !upgrading)
        {
            upgrading = true; 
            if(clickCoins >= powerClickUpgradePrice)
            {
                clickValue = clickValue * 2;   
                clickCoins = clickCoins - powerClickUpgradePrice; 
                powerClickUpgradePrice = powerClickUpgradePrice * 3; 
                powerClickUpgrade = 4; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
        
        if(powerClickUpgrade == 4 && !upgrading)
        {
            upgrading = true; 
            if(clickCoins >= powerClickUpgradePrice)
            {
                clickValue = clickValue * 2;   
                clickCoins = clickCoins - powerClickUpgradePrice; 
                powerClickUpgradePrice = powerClickUpgradePrice * 4; 
                powerClickUpgrade = 5; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
                
        if(powerClickUpgrade == 5 && !upgrading)
        {
            upgrading = true; 
            if(clickCoins >= powerClickUpgradePrice)
            {
                clickValue = clickValue * 2;   
                clickCoins = clickCoins - powerClickUpgradePrice; 
                powerClickUpgradePrice = powerClickUpgradePrice * 5; 
                powerClickUpgrade = 6; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
                
        if(powerClickUpgrade == 6 && !upgrading)
        {
            upgrading = true; 
            if(clickCoins >= powerClickUpgradePrice)
            {
                clickValue = clickValue * 2;   
                clickCoins = clickCoins - powerClickUpgradePrice; 
                powerClickUpgradePrice = powerClickUpgradePrice * 6; 
                powerClickUpgrade = 7; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
    }

    public void BuyOfflineUpgrade()
    {
       if(offlineUpgrade == 1 && !upgrading)
        {
           upgrading = true;

           if(clickCoins >= offlineUpgradePrice)
           {
               clickCoins = clickCoins - offlineUpgradePrice;
               offlineUpgradePrice = offlineUpgradePrice * 2; 
               offlineUpgrade = 2; 
               AssignUpgradeUI();
           }
           StartCoroutine(WaitUpgrades());
        }
        
        if(offlineUpgrade == 2 && !upgrading)
        {
           upgrading = true;

           if(clickCoins >= offlineUpgradePrice)
           {
               clickCoins = clickCoins - offlineUpgradePrice;
               offlineUpgradePrice = offlineUpgradePrice * 2; 
               offlineUpgrade = 3; 
               AssignUpgradeUI();
           }
           StartCoroutine(WaitUpgrades());
        }
                
        if(offlineUpgrade == 3 && !upgrading)
        {
           upgrading = true;

           if(clickCoins >= offlineUpgradePrice)
           {
               clickCoins = clickCoins - offlineUpgradePrice;
               offlineUpgradePrice = offlineUpgradePrice * 2; 
               offlineUpgrade = 4; 
               AssignUpgradeUI();
           }
           StartCoroutine(WaitUpgrades());
        }
    }

    public void BuyGameSpeedUpgrade()
    {
        if(gameSpeedUpgrade == 1 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= gameSpeedUpgradePrice)
            {
                player.gameSpeed = 2; 
                clickCoins = clickCoins - gameSpeedUpgradePrice; 
                gameSpeedUpgradePrice = gameSpeedUpgradePrice * 5; 
                gameSpeedUpgrade = 2; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
        
        if(gameSpeedUpgrade == 2 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= gameSpeedUpgradePrice)
            {
                player.gameSpeed = 3; 
                clickCoins = clickCoins - gameSpeedUpgradePrice; 
                gameSpeedUpgradePrice = gameSpeedUpgradePrice * 5; 
                gameSpeedUpgrade = 3; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }

        if(gameSpeedUpgrade == 3 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= gameSpeedUpgradePrice)
            {
                player.gameSpeed = 4; 
                clickCoins = clickCoins - gameSpeedUpgradePrice; 
                gameSpeedUpgradePrice = gameSpeedUpgradePrice * 5; 
                gameSpeedUpgrade = 4; 
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
    }

    IEnumerator WaitUpgrades()
    {
        yield return new WaitForSeconds(0.5f);
        upgrading = false; 
    }

    public void BuyAttackUpgrade()
    {
        if(atkPowerUpgrade == 1 && !upgrading)
        {
            upgrading = true;

            if(clickCoins >= atkPowerUpgradePrice)
            { 
                unit.attack = unit.attack + 5;
                clickCoins = clickCoins - atkPowerUpgradePrice; 
                atkPowerUpgradePrice = atkPowerUpgradePrice * 2; 
                atkPowerUpgrade = 2;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        } 
        
        if(atkPowerUpgrade == 2 && !upgrading)
        {
            upgrading = true;

            if(clickCoins >= atkPowerUpgradePrice)
            { 
                unit.attack = unit.attack + 10;
                clickCoins = clickCoins - atkPowerUpgradePrice; 
                atkPowerUpgradePrice = atkPowerUpgradePrice * 6; 
                atkPowerUpgrade = 3;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
                
        if(atkPowerUpgrade == 3 && !upgrading)
        {
            upgrading = true;

            if(clickCoins >= atkPowerUpgradePrice)
            { 
                unit.attack = unit.attack + 20;
                clickCoins = clickCoins - atkPowerUpgradePrice; 
                atkPowerUpgradePrice = atkPowerUpgradePrice * 4; 
                atkPowerUpgrade = 4;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
                        
        if(atkPowerUpgrade == 4 && !upgrading)
        {
            upgrading = true;

            if(clickCoins >= atkPowerUpgradePrice)
            { 
                unit.attack = unit.attack + 25;
                clickCoins = clickCoins - atkPowerUpgradePrice; 
                atkPowerUpgradePrice = atkPowerUpgradePrice * 8; 
                atkPowerUpgrade = 5;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }

    }

    public void BuyMaxHPUpgrade()
    {
        if(maxHPUpgrade == 1 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= maxHPUpgradePrice)
            {
                unit.maxHP = unit.maxHP + 5; 
                clickCoins = clickCoins - maxHPUpgradePrice;
                maxHPUpgradePrice = maxHPUpgradePrice * 2; 
                maxHPUpgrade = 2;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
        
        if(maxHPUpgrade == 2 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= maxHPUpgradePrice)
            {
                unit.maxHP = unit.maxHP + 10; 
                clickCoins = clickCoins - maxHPUpgradePrice;
                maxHPUpgradePrice = maxHPUpgradePrice * 6; 
                maxHPUpgrade = 3;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
                
        if(maxHPUpgrade == 3 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= maxHPUpgradePrice)
            {
                unit.maxHP = unit.maxHP + 20; 
                clickCoins = clickCoins - maxHPUpgradePrice;
                maxHPUpgradePrice = maxHPUpgradePrice * 4; 
                maxHPUpgrade = 4;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }
                
        if(maxHPUpgrade == 4 && !upgrading)
        {
            upgrading = true; 

            if(clickCoins >= maxHPUpgradePrice)
            {
                unit.maxHP = unit.maxHP + 25; 
                clickCoins = clickCoins - maxHPUpgradePrice;
                maxHPUpgradePrice = maxHPUpgradePrice * 8; 
                maxHPUpgrade = 5;
                AssignUpgradeUI();
            }
            StartCoroutine(WaitUpgrades());
        }

    }

    void AssignUpgradeUI()
    {
        #region SELF CLICK UPGRADE
        if(clickCoins < selfClickUpgradePrice)
        {
            clickerUpgradePriceTxt.color = Color.red; 
        }
        else
        {
            clickerUpgradePriceTxt.color = Color.white; 
        }
        
        if(selfClickUpgrade == 1)
        {
            clickerUpgradeInfoTxt.text = "PURCHASE: Auto-clicks ONCE per second";
            clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
            clickerUpgradePriceTxt.text = "" + selfClickUpgradePrice;
        }

        if(selfClickUpgrade == 2)
        {
            clickerUpgradeInfoTxt.text = "PURCHASE: Auto-clicks TWICE per second";
            clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
            clickerUpgradePriceTxt.text = "" + selfClickUpgradePrice;
        }

        if(selfClickUpgrade == 3)
        {
                clickerUpgradeInfoTxt.text = "PURCHASE: Auto-clicks THREE per second";
                clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
                clickerUpgradePriceTxt.text = "" + selfClickUpgradePrice;
        }

        if(selfClickUpgrade == 4)
        {
                clickerUpgradeInfoTxt.text = "PURCHASE: Auto-clicks FOUR per second";
                clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
                clickerUpgradePriceTxt.text = "" + selfClickUpgradePrice;
        }
        
        if(selfClickUpgrade == 5)
        {
                clickerUpgradeInfoTxt.text = "PURCHASE: Auto-clicks EIGHT per second";
                clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
                clickerUpgradePriceTxt.text = "" + selfClickUpgradePrice;
        }
                
        if(selfClickUpgrade == 6)
        {
                clickerUpgradeInfoTxt.text = "PURCHASE: Auto-clicks SIXTEEN per second";
                clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
                clickerUpgradePriceTxt.text = "" + selfClickUpgradePrice;
        }
                        
        if(selfClickUpgrade == 7)
        {
                clickerUpgradeInfoTxt.text = "[COMPLETE] Auto-clicks SIXTEEN times per second";
                clickerUpgradeLvlTxt.text = "Level " + selfClickUpgrade;
                clickerUpgradePriceTxt.text = "N/A";
        }
        #endregion 

        #region POWER CLICK UPGRADE
        
        if(clickCoins < powerClickUpgradePrice)
        {
            powerClickPriceTxt.color = Color.red; 
        }
        else
        {
            powerClickPriceTxt.color = Color.white; 
        }

        if(powerClickUpgrade == 1)
        {
            powerClickInfoTxt.text = "PURCHASE: User clicks return 2 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "" + powerClickUpgradePrice; 
        }
        
        if(powerClickUpgrade == 2)
        {
            powerClickInfoTxt.text = "PURCHASE: User clicks return 4 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "" + powerClickUpgradePrice; 
        }

        if(powerClickUpgrade == 3)
        {
            powerClickInfoTxt.text = "PURCHASE: User clicks return 8 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "" + powerClickUpgradePrice; 
        }

        if(powerClickUpgrade == 4)
        {
            powerClickInfoTxt.text = "PURCHASE: User clicks return 16 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "" + powerClickUpgradePrice; 
        }
        
        if(powerClickUpgrade == 5)
        {
            powerClickInfoTxt.text = "PURCHASE: User clicks return 32 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "" + powerClickUpgradePrice; 
        }
        
        if(powerClickUpgrade == 6)
        {
            powerClickInfoTxt.text = "PURCHASE: User clicks return 64 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "" + powerClickUpgradePrice; 
        }
        
        if(powerClickUpgrade == 7)
        {
            powerClickInfoTxt.text = "[COMPLETE] User clicks return 64 coins"; 
            powerClickLvlTxt.text = "Level " + powerClickUpgrade;
            powerClickPriceTxt.text = "N/A"; 
        }
        

        #endregion

        #region GAME SPEED UPGRADE
                
        if(clickCoins < gameSpeedUpgradePrice)
        {
            gameSpeedPriceTxt.color = Color.red; 
        }
        else
        {
            gameSpeedPriceTxt.color = Color.white; 
        }

        if(gameSpeedUpgrade == 1)
        {
            gameSpeedInfoTxt.text = "PURCHASE: (FIGHTER BATTLES ONLY) Increase game speed to x2";
            gameSpeedLvlTxt.text = "Level " + gameSpeedUpgrade;
            gameSpeedPriceTxt.text = "" + gameSpeedUpgradePrice; 
        }
        if(gameSpeedUpgrade == 2)
        {
            gameSpeedInfoTxt.text = "PURCHASE: (FIGHTER BATTLES ONLY) Increase game speed to x3";
            gameSpeedLvlTxt.text = "Level " + gameSpeedUpgrade;
            gameSpeedPriceTxt.text = "" + gameSpeedUpgradePrice; 
        }
        if(gameSpeedUpgrade == 3)
        {
            gameSpeedInfoTxt.text = "PURCHASE: (FIGHTER BATTLES ONLY) Increase game speed to x4";
            gameSpeedLvlTxt.text = "Level " + gameSpeedUpgrade;
            gameSpeedPriceTxt.text = "" + gameSpeedUpgradePrice; 
        }
        if(gameSpeedUpgrade == 4)
        {
            gameSpeedInfoTxt.text = "(FIGHTER BATTLES ONLY) [COMPLETE] - X4 GAME SPEED";
            gameSpeedLvlTxt.text = "Level " + gameSpeedUpgrade;
            gameSpeedPriceTxt.text = "N/A"; 
        }

        #endregion 

        #region ATTACK POWER UPGRADE 
                
        if(clickCoins < atkPowerUpgradePrice)
        {
            atkPowerPriceTxt.color = Color.red; 
        }
        else
        {
            atkPowerPriceTxt.color = Color.white; 
        }

        if(atkPowerUpgrade == 1)
        {
            atkPowerInfoTxt.text = "PURCHASE: Increases your CURRENT fighters attack by 5";
            atkPowerLvlTxt.text = "Level " + atkPowerUpgrade; 
            atkPowerPriceTxt.text = "" + atkPowerUpgradePrice; 
        }

        if(atkPowerUpgrade == 2)
        {
            atkPowerInfoTxt.text = "PURCHASE: Increases your CURRENT fighters attack by 10";
            atkPowerLvlTxt.text = "Level " + atkPowerUpgrade; 
            atkPowerPriceTxt.text = "" + atkPowerUpgradePrice; 
        }
        
        if(atkPowerUpgrade == 3)
        {
            atkPowerInfoTxt.text = "PURCHASE: Increases your CURRENT fighters attack by 20";
            atkPowerLvlTxt.text = "Level " + atkPowerUpgrade; 
            atkPowerPriceTxt.text = "" + atkPowerUpgradePrice; 
        }
                
        if(atkPowerUpgrade == 4)
        {
            atkPowerInfoTxt.text = "PURCHASE: Increases your CURRENT fighters attack by 25";
            atkPowerLvlTxt.text = "Level " + atkPowerUpgrade; 
            atkPowerPriceTxt.text = "" + atkPowerUpgradePrice; 
        }
                
        if(atkPowerUpgrade == 5)
        {
            atkPowerInfoTxt.text = "[COMPLETE] CURRENT fighters attack total increase by 60";
            atkPowerLvlTxt.text = "Level " + atkPowerUpgrade; 
            atkPowerPriceTxt.text = "N/A"; 
        }
        
        #endregion

        #region MAX HP UPGRADE 
              
        if(clickCoins < maxHPUpgradePrice)
        {
            maxHPPriceTxt.color = Color.red; 
        }
        else
        {
            maxHPPriceTxt.color = Color.white; 
        }

        if(maxHPUpgrade == 1)
        {
            maxHPInfoTxt.text = "PURCHASE: Increases your CURRENT fighters MAX HP by 5"; 
            maxHPLvlTxt.text = "Level " + maxHPUpgrade;
            maxHPPriceTxt.text = "" + maxHPUpgradePrice;
        }

        if(maxHPUpgrade == 2)
        {
            maxHPInfoTxt.text = "PURCHASE: Increases your CURRENT fighters MAX HP by 10";
            maxHPLvlTxt.text = "Level " + maxHPUpgrade; 
            maxHPPriceTxt.text = "" + maxHPUpgradePrice; 
        }
        
        if(maxHPUpgrade == 3)
        {
            maxHPInfoTxt.text = "PURCHASE: Increases your CURRENT fighters MAX HP by 20";
            maxHPLvlTxt.text = "Level " + maxHPUpgrade; 
            maxHPPriceTxt.text = "" + maxHPUpgradePrice; 
        }
        
        if(maxHPUpgrade == 4)
        {
            maxHPInfoTxt.text = "PURCHASE: Increases your CURRENT fighters MAX HP by 25";
            maxHPLvlTxt.text = "Level " + maxHPUpgrade; 
            maxHPPriceTxt.text = "" + maxHPUpgradePrice; 
        }
        
        if(maxHPUpgrade == 5)
        {
            maxHPInfoTxt.text = "[COMPLETE] CURRENT fighters MAX HP total increase by 60";
            maxHPLvlTxt.text = "Level " + maxHPUpgrade; 
            maxHPPriceTxt.text = "N/A"; 
        }

        #endregion

        #region OFFLINE UPGRADE
            
        if(clickCoins < offlineUpgradePrice)
        {
            offlinePriceTxt.color = Color.red; 
        }
        else
        {
            offlinePriceTxt.color = Color.white; 
        }

        if(offlineUpgrade == 1)
        {
            offlineInfoTxt.text = "PURCHASE: Increase offline coins produced by x1.5";
            offlineLvlTxt.text = "Level " + offlineUpgrade; 
            offlinePriceTxt.text = "" + offlineUpgradePrice; 
        }
        
        if(offlineUpgrade == 2)
        {
            offlineInfoTxt.text = "PURCHASE: Increase offline coins produced by x2";
            offlineLvlTxt.text = "Level " + offlineUpgrade; 
            offlinePriceTxt.text = "" + offlineUpgradePrice; 
        }
        
        if(offlineUpgrade == 3)
        {
            offlineInfoTxt.text = "PURCHASE: Increase offline coins produced by x3";
            offlineLvlTxt.text = "Level " + offlineUpgrade; 
            offlinePriceTxt.text = "" + offlineUpgradePrice; 
        }
                
        if(offlineUpgrade == 4)
        {
            offlineInfoTxt.text = "[COMPLETE] Offline coin production increased by x3";
            offlineLvlTxt.text = "Level " + offlineUpgrade; 
            offlinePriceTxt.text = "N//A";
        }

        #endregion 
    
    }

}
