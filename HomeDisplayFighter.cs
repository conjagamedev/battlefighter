using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using BayatGames.SaveGameFree; 
using UnityEngine.Advertisements; 

public class HomeDisplayFighter : MonoBehaviour
{
    public GameObject[] fighters; 
    public Sprite[] fighterSprites; 

    [Space(20)]

    public Image currFighterImage;
    public Text currentFighterTxt; 
    public Text hpAtkTxt; 
    public Text SpdHealTxt; 
    public Slider xpSlider;
    public Text xpTxt; 
    
    Player player; 
    Unit unit; 
    public bool gameSaved; 

    public GameObject battleLvlObj; 
    public Text battleLvlTxt; 
    public int battleLvl = 1; 

    public GameObject themeMusic; 

    string gameId = "3922605"; //google play 
    bool testMode = true; 
    string placementId = "bannerVideo"; 
    bool showAd;

    public Text msgboxText; 
    public Animation msgboxAnim; 

    public Text clickCoinsTxt; 


    public void Start()
    {


        if(SaveGame.Exists("clickcoins"))
            clickCoinsTxt.text = "" + SaveGame.Load<int>("clickcoins"); 
        else
            clickCoinsTxt.text = "0"; 

        if(SaveGame.Exists("offlinetime"))
        {
            if(SaveGame.Load<string>("offlinetime") == "")
                SaveGame.Save<string>("offlinetime", System.DateTime.Now.ToBinary().ToString()); 
        }
        
        if(!SaveGame.Exists("offlineprogresscheck"))
        {
            msgboxAnim.Play("msgBoxAnim"); 
            msgboxText.text = "Welcome to FIGHTER BATTLE! If you wish to read a tutorial, click on the settings button!";  
            SaveGame.Save<bool>("offlineprogresscheck", true);
        }

        player = GetComponent<Player>();
        if(SaveGame.Exists("currentfighter"))
        {
            player.currentFighter = SaveGame.Load<string>("currentfighter");
        }
        
        if(player.currentFighter == "")
        {
            player.currentFighter = "Egyptian Mage";
        }
        ApplyFighterDisplay();
        LoadFighter(); 
        
        if(unit.lvl != 50)
        {
            battleLvlObj.active = false;
        }

        if(SaveGame.Exists("battlelvl"))
        {
            battleLvl = SaveGame.Load<int>("battlelvl");
        }

        if(SaveGame.Exists("showad"))
            showAd = SaveGame.Load<bool>("showad"); 
            
        Advertisement.Initialize(gameId, testMode);

        if(SaveGame.Exists("fightcount"))
        {
            if(SaveGame.Load<int>("fightcount") == 6)
                showAd = true; 
        }

        if(Advertisement.IsReady() && showAd)
        {
            Advertisement.Show();
            SaveGame.Save("fightcount", 1);
            showAd = false;
            SaveGame.Save<bool>("showad", false); 
        }
        else
        {
            print("no ad");
        }

        Advertisement.Initialize(gameId, testMode); 
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_LEFT);
        //Advertisement.Banner.Show(placementId);

        #region messagebox unlocks
        if(SaveGame.Exists("newunlock"))
        {

            if(SaveGame.Load<string>("newunlock") == "Sphinx")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked EGYPTIAN SPHINX. Check your fighter collection!";
            }
                        
            if(SaveGame.Load<string>("newunlock") == "Mario")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked MARIO. Check your fighter collection!";
            }
                        
            if(SaveGame.Load<string>("newunlock") == "Charizard")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked CHARIZARD. Check your fighter collection!";
            }
                        
            if(SaveGame.Load<string>("newunlock") == "Luigi")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked LUIGI. Check your fighter collection!";
            }
                                    
            if(SaveGame.Load<string>("newunlock") == "Link")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked LINK. Check your fighter collection!";
            }
                                    
            if(SaveGame.Load<string>("newunlock") == "Mew")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked MEW. Check your fighter collection!";
            }
            
            if(SaveGame.Load<string>("newunlock") == "Dracula")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked DRACULA. Check your fighter collection!";
            }
                        
            if(SaveGame.Load<string>("newunlock") == "Bowser")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked BOWSER. Check your fighter collection!";
            }
                                    
            if(SaveGame.Load<string>("newunlock") == "Ganondorf")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked GANONDORF. Check your fighter collection! Zelda is also available to purchase in the SHOP.";
            }
                                    
            if(SaveGame.Load<string>("newunlock") == "Mewtwo")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked MEWTWO. Check your fighter collection!";
            }
                                                
            if(SaveGame.Load<string>("newunlock") == "Blood Slime")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked BLOOD SLIME. Purchase in the fighter shop!";
            }
                                                            
            if(SaveGame.Load<string>("newunlock") == "Elven Naturess")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked ELVEN NATURESS. Purchase in the fighter shop!";
            }
                                                                        
            if(SaveGame.Load<string>("newunlock") == "Earth Gem Golem")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked EARTH GEM GOLEM. Purchase in the fighter shop!";
            }
                                                                                    
            if(SaveGame.Load<string>("newunlock") == "Colossal Rex")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked COLOSSAL REX. Purchase in the fighter shop!";
            }
                                                                                                
            if(SaveGame.Load<string>("newunlock") == "Undeen")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked UNDEEN. Purchase in the fighter shop!";
            }
                                                                                                
            if(SaveGame.Load<string>("newunlock") == "Ignis")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked IGNIS. Purchase in the fighter shop!";
            }
                                                                                                
            if(SaveGame.Load<string>("newunlock") == "Scorpion")
            {
                msgboxAnim.Play("msgBoxAnim"); 
                msgboxText.text = "You have unlocked SCORPION. Purchase in the fighter shop!";
            }    
        }
        #endregion

        currentFighterTxt.text = "Lv." + unit.lvl + " - " + unit.unitName;
        hpAtkTxt.text = "HP: " + unit.maxHP + "     Atk: " + unit.attack;
        SpdHealTxt.text = "Spd: " + unit.speed + "     Heal: " + unit.heal;  

        if(GameObject.FindGameObjectWithTag("themeMusic") == null)
        {
            if(SaveGame.Exists("ismuted"))
            {
                if(SaveGame.Load<bool>("ismuted") == false) 
                {
                    Instantiate(themeMusic); 
                    DontDestroyOnLoad(GameObject.FindGameObjectWithTag("themeMusic")); 
                }
            }
            else
            {
                Instantiate(themeMusic); 
                DontDestroyOnLoad(GameObject.FindGameObjectWithTag("themeMusic"));   
            }
        }
        else
        {
            return; 
        }

    }

    void Update()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("homeScene"))
        {
            ApplyFighterDisplay();
            Time.timeScale = 1;
            HandleBattleTxt();
        }
    }

    public void PopUpMenuOKButton()
    {
        msgboxAnim.Play("msgBoxAwayAnim"); 
        SaveGame.Save<string>("newunlock", "");
    }
    
    public void ApplyFighterDisplay()
    {
        if(player.currentFighter == "Egyptian Mage")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[1];

            unit = fighters[1].GetComponent<Unit>();
        }
        else if(player.currentFighter == "Egyptian Warrior")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[0];

            unit = fighters[0].GetComponent<Unit>();
        }
        else if(player.currentFighter == "Egyptian Sphinx")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[2];

            unit = fighters[2].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Mario")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[3];

            unit = fighters[3].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Charizard")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[4];

            unit = fighters[4].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Luigi")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[5];

            unit = fighters[5].GetComponent<Unit>();
        }

        
        else if(player.currentFighter == "Link")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[6];

            unit = fighters[6].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Mew")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[7];

            unit = fighters[7].GetComponent<Unit>();
        }

        
        else if(player.currentFighter == "Blue Eyes White Dragon")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[8];

            unit = fighters[8].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Dracula")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[9];

            unit = fighters[9].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Bowser")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[10];

            unit = fighters[10].GetComponent<Unit>();
        }

        else if(player.currentFighter == "Ganondorf")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[11];

            unit = fighters[11].GetComponent<Unit>();
        }
        
        else if(player.currentFighter == "Mewtwo")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[12];

            unit = fighters[12].GetComponent<Unit>();
        }
                
        else if(player.currentFighter == "Princess Zelda")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[13];

            unit = fighters[13].GetComponent<Unit>();
        }
                        
        else if(player.currentFighter == "Blood Slime")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[14];

            unit = fighters[14].GetComponent<Unit>();
        }
                                
        else if(player.currentFighter == "Elven Naturess")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[15];

            unit = fighters[15].GetComponent<Unit>();
        }
                                        
        else if(player.currentFighter == "Earth Gem Golem")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[16];

            unit = fighters[16].GetComponent<Unit>();
        }
                                                
        else if(player.currentFighter == "Colossal Rex")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[17];

            unit = fighters[17].GetComponent<Unit>();
        }
                                                        
        else if(player.currentFighter == "Undeen")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[18];

            unit = fighters[18].GetComponent<Unit>();
        }
                                                                
        else if(player.currentFighter == "Ignis")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[19];

            unit = fighters[19].GetComponent<Unit>();
        }
                                                                        
        else if(player.currentFighter == "Scorpion")
        {
            if(currFighterImage.sprite != null)
                currFighterImage.sprite = fighterSprites[20];

            unit = fighters[20].GetComponent<Unit>();
        }

        currentFighterTxt.text = "Lv." + unit.lvl + " - " + unit.unitName;
        hpAtkTxt.text = "HP: " + unit.maxHP + "     Atk: " + unit.attack;
        SpdHealTxt.text = "Spd: " + unit.speed + "     Heal: " + unit.heal;
        xpSlider.maxValue = unit.maxXP; 
        xpSlider.minValue = 0; 
        xpSlider.value = unit.xp;
        xpTxt.text = unit.xp + "/" + unit.maxXP;
    }

    public void LoadFighter()
    {
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
    }

    public void ClearGameData()
    {
        SaveGame.Clear();
        player.currentFighter = "Egyptian Mage";
    }

    //DEBUG !! 

    public void LevelUpBtn()
    {
        unit.LevelUp(unit.maxXP - unit.xp);
        SaveFighter();
        ApplyFighterDisplay();

    }

    public void BattleLvlBtn()
    {
        battleLvl++;

        if(battleLvl == 6)
        {
            battleLvl = 1;
        } 

        SaveGame.Save<int>("battlelvl", battleLvl);
    }

    void HandleBattleTxt()
    {
        if(battleLvl == 1)
        {
            battleLvlTxt.text = "LV1-10";
        }
        
        if(battleLvl == 2)
        {
            battleLvlTxt.text = "LV11-20";
        }
                
        if(battleLvl == 3)
        {
            battleLvlTxt.text = "LV21-30";
        }
                
        if(battleLvl == 4)
        {
            battleLvlTxt.text = "LV31-40";
        }
                
        if(battleLvl == 5)
        {
            battleLvlTxt.text = "LV41-50";
        }


    }

    public void SaveFighter()
    {
        if(player.currentFighter == "Egyptian Warrior")
        { 
            SaveGame.Save<int>("[0]maxhp", unit.maxHP);
            SaveGame.Save<int>("[0]curhp", unit.currentHP);
            SaveGame.Save<int>("[0]atk", unit.attack);
            SaveGame.Save<int>("[0]spd", unit.speed);
            SaveGame.Save<int>("[0]heal", unit.heal);
            SaveGame.Save<int>("[0]lvl", unit.lvl);
            SaveGame.Save<int>("[0]xp", unit.xp);
            SaveGame.Save<int>("[0]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[0]unlocked", unit.unlocked);
        }
        if(player.currentFighter == "Egyptian Mage")
        {
            SaveGame.Save<int>("[1]maxhp", unit.maxHP);
            SaveGame.Save<int>("[1]curhp", unit.currentHP);
            SaveGame.Save<int>("[1]atk", unit.attack);
            SaveGame.Save<int>("[1]spd", unit.speed);
            SaveGame.Save<int>("[1]heal", unit.heal);
            SaveGame.Save<int>("[1]lvl", unit.lvl);
            SaveGame.Save<int>("[1]xp", unit.xp);
            SaveGame.Save<int>("[1]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[1]unlocked", unit.unlocked);
        }
        if(player.currentFighter == "Egyptian Sphinx")
        {
            SaveGame.Save<int>("[2]maxhp", unit.maxHP);
            SaveGame.Save<int>("[2]curhp", unit.currentHP);
            SaveGame.Save<int>("[2]atk", unit.attack);
            SaveGame.Save<int>("[2]spd", unit.speed);
            SaveGame.Save<int>("[2]heal", unit.heal);
            SaveGame.Save<int>("[2]lvl", unit.lvl);
            SaveGame.Save<int>("[2]xp", unit.xp);
            SaveGame.Save<int>("[2]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[2]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Mario")
        {
            SaveGame.Save<int>("[3]maxhp", unit.maxHP);
            SaveGame.Save<int>("[3]curhp", unit.currentHP);
            SaveGame.Save<int>("[3]atk", unit.attack);
            SaveGame.Save<int>("[3]spd", unit.speed);
            SaveGame.Save<int>("[3]heal", unit.heal);
            SaveGame.Save<int>("[3]lvl", unit.lvl);
            SaveGame.Save<int>("[3]xp", unit.xp);
            SaveGame.Save<int>("[3]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[3]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Charizard")
        {
            SaveGame.Save<int>("[4]maxhp", unit.maxHP);
            SaveGame.Save<int>("[4]curhp", unit.currentHP);
            SaveGame.Save<int>("[4]atk", unit.attack);
            SaveGame.Save<int>("[4]spd", unit.speed);
            SaveGame.Save<int>("[4]heal", unit.heal);
            SaveGame.Save<int>("[4]lvl", unit.lvl);
            SaveGame.Save<int>("[4]xp", unit.xp);
            SaveGame.Save<int>("[4]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[4]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Luigi")
        {
            SaveGame.Save<int>("[5]maxhp", unit.maxHP);
            SaveGame.Save<int>("[5]curhp", unit.currentHP);
            SaveGame.Save<int>("[5]atk", unit.attack);
            SaveGame.Save<int>("[5]spd", unit.speed);
            SaveGame.Save<int>("[5]heal", unit.heal);
            SaveGame.Save<int>("[5]lvl", unit.lvl);
            SaveGame.Save<int>("[5]xp", unit.xp);
            SaveGame.Save<int>("[5]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[5]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Link")
        {
            SaveGame.Save<int>("[6]maxhp", unit.maxHP);
            SaveGame.Save<int>("[6]curhp", unit.currentHP);
            SaveGame.Save<int>("[6]atk", unit.attack);
            SaveGame.Save<int>("[6]spd", unit.speed);
            SaveGame.Save<int>("[6]heal", unit.heal);
            SaveGame.Save<int>("[6]lvl", unit.lvl);
            SaveGame.Save<int>("[6]xp", unit.xp);
            SaveGame.Save<int>("[6]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[6]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Mew")
        {
            SaveGame.Save<int>("[7]maxhp", unit.maxHP);
            SaveGame.Save<int>("[7]curhp", unit.currentHP);
            SaveGame.Save<int>("[7]atk", unit.attack);
            SaveGame.Save<int>("[7]spd", unit.speed);
            SaveGame.Save<int>("[7]heal", unit.heal);
            SaveGame.Save<int>("[7]lvl", unit.lvl);
            SaveGame.Save<int>("[7]xp", unit.xp);
            SaveGame.Save<int>("[7]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[7]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Blue Eyes White Dragon")
        {
            SaveGame.Save<int>("[8]maxhp", unit.maxHP);
            SaveGame.Save<int>("[8]curhp", unit.currentHP);
            SaveGame.Save<int>("[8]atk", unit.attack);
            SaveGame.Save<int>("[8]spd", unit.speed);
            SaveGame.Save<int>("[8]heal", unit.heal);
            SaveGame.Save<int>("[8]lvl", unit.lvl);
            SaveGame.Save<int>("[8]xp", unit.xp);
            SaveGame.Save<int>("[8]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[8]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Dracula")
        {
            SaveGame.Save<int>("[9]maxhp", unit.maxHP);
            SaveGame.Save<int>("[9]curhp", unit.currentHP);
            SaveGame.Save<int>("[9]atk", unit.attack);
            SaveGame.Save<int>("[9]spd", unit.speed);
            SaveGame.Save<int>("[9]heal", unit.heal);
            SaveGame.Save<int>("[9]lvl", unit.lvl);
            SaveGame.Save<int>("[9]xp", unit.xp);
            SaveGame.Save<int>("[9]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[9]unlocked", unit.unlocked);
        }

        
        if(player.currentFighter == "Bowser")
        {
            SaveGame.Save<int>("[10]maxhp", unit.maxHP);
            SaveGame.Save<int>("[10]curhp", unit.currentHP);
            SaveGame.Save<int>("[10]atk", unit.attack);
            SaveGame.Save<int>("[10]spd", unit.speed);
            SaveGame.Save<int>("[10]heal", unit.heal);
            SaveGame.Save<int>("[10]lvl", unit.lvl);
            SaveGame.Save<int>("[10]xp", unit.xp);
            SaveGame.Save<int>("[10]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[10]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Ganondorf")
        {
            SaveGame.Save<int>("[11]maxhp", unit.maxHP);
            SaveGame.Save<int>("[11]curhp", unit.currentHP);
            SaveGame.Save<int>("[11]atk", unit.attack);
            SaveGame.Save<int>("[11]spd", unit.speed);
            SaveGame.Save<int>("[11]heal", unit.heal);
            SaveGame.Save<int>("[11]lvl", unit.lvl);
            SaveGame.Save<int>("[11]xp", unit.xp);
            SaveGame.Save<int>("[11]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[11]unlocked", unit.unlocked);
        }
        
        if(player.currentFighter == "Mewtwo")
        {
            SaveGame.Save<int>("[12]maxhp", unit.maxHP);
            SaveGame.Save<int>("[12]curhp", unit.currentHP);
            SaveGame.Save<int>("[12]atk", unit.attack);
            SaveGame.Save<int>("[12]spd", unit.speed);
            SaveGame.Save<int>("[12]heal", unit.heal);
            SaveGame.Save<int>("[12]lvl", unit.lvl);
            SaveGame.Save<int>("[12]xp", unit.xp);
            SaveGame.Save<int>("[12]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[12]unlocked", unit.unlocked);
        }

        if(player.currentFighter == "Mewtwo")
        {
            SaveGame.Save<int>("[13]maxhp", unit.maxHP);
            SaveGame.Save<int>("[13]curhp", unit.currentHP);
            SaveGame.Save<int>("[13]atk", unit.attack);
            SaveGame.Save<int>("[13]spd", unit.speed);
            SaveGame.Save<int>("[13]heal", unit.heal);
            SaveGame.Save<int>("[13]lvl", unit.lvl);
            SaveGame.Save<int>("[13]xp", unit.xp);
            SaveGame.Save<int>("[13]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[13]unlocked", unit.unlocked);
        }
        
        if(player.currentFighter == "Blood Slime")
        {
            SaveGame.Save<int>("[14]maxhp", unit.maxHP);
            SaveGame.Save<int>("[14]curhp", unit.currentHP);
            SaveGame.Save<int>("[14]atk", unit.attack);
            SaveGame.Save<int>("[14]spd", unit.speed);
            SaveGame.Save<int>("[14]heal", unit.heal);
            SaveGame.Save<int>("[14]lvl", unit.lvl);
            SaveGame.Save<int>("[14]xp", unit.xp);
            SaveGame.Save<int>("[14]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[14]unlocked", unit.unlocked);
        }
               
        if(player.currentFighter == "Elven Naturess")
        {
            SaveGame.Save<int>("[15]maxhp", unit.maxHP);
            SaveGame.Save<int>("[15]curhp", unit.currentHP);
            SaveGame.Save<int>("[15]atk", unit.attack);
            SaveGame.Save<int>("[15]spd", unit.speed);
            SaveGame.Save<int>("[15]heal", unit.heal);
            SaveGame.Save<int>("[15]lvl", unit.lvl);
            SaveGame.Save<int>("[15]xp", unit.xp);
            SaveGame.Save<int>("[15]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[15]unlocked", unit.unlocked);
        }
                       
        if(player.currentFighter == "Earth Gem Golem")
        {
            SaveGame.Save<int>("[16]maxhp", unit.maxHP);
            SaveGame.Save<int>("[16]curhp", unit.currentHP);
            SaveGame.Save<int>("[16]atk", unit.attack);
            SaveGame.Save<int>("[16]spd", unit.speed);
            SaveGame.Save<int>("[16]heal", unit.heal);
            SaveGame.Save<int>("[16]lvl", unit.lvl);
            SaveGame.Save<int>("[16]xp", unit.xp);
            SaveGame.Save<int>("[16]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[16]unlocked", unit.unlocked);
        }
                               
        if(player.currentFighter == "Colossal Rex")
        {
            SaveGame.Save<int>("[17]maxhp", unit.maxHP);
            SaveGame.Save<int>("[17]curhp", unit.currentHP);
            SaveGame.Save<int>("[17]atk", unit.attack);
            SaveGame.Save<int>("[17]spd", unit.speed);
            SaveGame.Save<int>("[17]heal", unit.heal);
            SaveGame.Save<int>("[17]lvl", unit.lvl);
            SaveGame.Save<int>("[17]xp", unit.xp);
            SaveGame.Save<int>("[17]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[17]unlocked", unit.unlocked);
        }
                                       
        if(player.currentFighter == "Undeen")
        {
            SaveGame.Save<int>("[18]maxhp", unit.maxHP);
            SaveGame.Save<int>("[18]curhp", unit.currentHP);
            SaveGame.Save<int>("[18]atk", unit.attack);
            SaveGame.Save<int>("[18]spd", unit.speed);
            SaveGame.Save<int>("[18]heal", unit.heal);
            SaveGame.Save<int>("[18]lvl", unit.lvl);
            SaveGame.Save<int>("[18]xp", unit.xp);
            SaveGame.Save<int>("[18]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[18]unlocked", unit.unlocked);
        }
                                               
        if(player.currentFighter == "Ignis")
        {
            SaveGame.Save<int>("[19]maxhp", unit.maxHP);
            SaveGame.Save<int>("[19]curhp", unit.currentHP);
            SaveGame.Save<int>("[19]atk", unit.attack);
            SaveGame.Save<int>("[19]spd", unit.speed);
            SaveGame.Save<int>("[19]heal", unit.heal);
            SaveGame.Save<int>("[19]lvl", unit.lvl);
            SaveGame.Save<int>("[19]xp", unit.xp);
            SaveGame.Save<int>("[19]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[19]unlocked", unit.unlocked);
        }
                                                       
        if(player.currentFighter == "Scorpion")
        {
            SaveGame.Save<int>("[20]maxhp", unit.maxHP);
            SaveGame.Save<int>("[20]curhp", unit.currentHP);
            SaveGame.Save<int>("[20]atk", unit.attack);
            SaveGame.Save<int>("[20]spd", unit.speed);
            SaveGame.Save<int>("[20]heal", unit.heal);
            SaveGame.Save<int>("[20]lvl", unit.lvl);
            SaveGame.Save<int>("[20]xp", unit.xp);
            SaveGame.Save<int>("[20]maxxp", unit.maxXP);
            SaveGame.Save<bool>("[20]unlocked", unit.unlocked);
        }
    }
}

