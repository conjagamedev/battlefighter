using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using BayatGames.SaveGameFree;

public class Collections : MonoBehaviour
{
    Player player; 
    public Text[] activeFightersTxt;  
    public GameObject[] fighters; 
    public GameObject[] fightersOnScreen; 
    Unit unit; 
    bool unlockedEgypstianSphinx; 

    void Start()
    {
        player = GameObject.Find("HomeSystem").GetComponent<Player>();

        if(SaveGame.Load<bool>("[2]unlocked") == false)
        {
            GameObject sphinxName = GameObject.Find("sphinxName");
            sphinxName.active = false; 
            Image image = fightersOnScreen[2].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[2]unlocked") == true)
        {
            GameObject sphinxName = GameObject.Find("sphinxName");
            sphinxName.active = true;         
        }


        if(SaveGame.Load<bool>("[3]unlocked") == false )
        {
            GameObject marioName = GameObject.Find("marioName");
            marioName.active = false; 
            Image image = fightersOnScreen[3].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[3]unlocked") == true)
        {
            GameObject marioName = GameObject.Find("marioName");
            marioName.active = true;  
     
        }

        if(SaveGame.Load<bool>("[4]unlocked") == false)
        {
            GameObject charizardName = GameObject.Find("charizardName");
            charizardName.active = false; 
            Image image = fightersOnScreen[4].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[4]unlocked") == true) 
        {
            GameObject charizardName = GameObject.Find("charizardName");
            charizardName.active = true;        
        }

        if(SaveGame.Load<bool>("[5]unlocked") == false)
        {
            GameObject luigiName = GameObject.Find("luigiName");
            luigiName.active = false; 
            Image image = fightersOnScreen[5].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[5]unlocked") == true) 
        {
            GameObject luigiName = GameObject.Find("luigiName");
            luigiName.active = true;        
        }

        if(SaveGame.Load<bool>("[6]unlocked") == false)
        {
            GameObject linkName = GameObject.Find("linkName");
            linkName.active = false; 
            Image image = fightersOnScreen[6].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[6]unlocked") == true) 
        {
            GameObject linkName = GameObject.Find("linkName");
            linkName.active = true;         
        }

        if(SaveGame.Load<bool>("[7]unlocked") == false)
        {
            GameObject mewName = GameObject.Find("mewName");
            mewName.active = false; 
            Image image = fightersOnScreen[7].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[7]unlocked") == true) 
        {
            GameObject mewName = GameObject.Find("mewName");
            mewName.active = true;  

        }

        if(SaveGame.Load<bool>("[8]unlocked") == false)
        {
            GameObject blueEyesName = GameObject.Find("blueEyesName");
            blueEyesName.active = false; 
            Image image = fightersOnScreen[8].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[8]unlocked") == true) 
        {
            GameObject blueEyesName = GameObject.Find("blueEyesName");
            blueEyesName.active = true; 
        
        }

        if(SaveGame.Load<bool>("[9]unlocked") == false)
        {
            GameObject draculaName = GameObject.Find("draculaName");
            draculaName.active = false; 
            Image image = fightersOnScreen[9].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[9]unlocked") == true) 
        {
            GameObject draculaName = GameObject.Find("draculaName");
            draculaName.active = true;       
        }

        if(SaveGame.Load<bool>("[10]unlocked") == false)
        {
            GameObject bowserName = GameObject.Find("bowserName");
            bowserName.active = false; 
            Image image = fightersOnScreen[10].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[10]unlocked") == true) 
        {
            GameObject bowserName = GameObject.Find("bowserName");
            bowserName.active = true;  
        }

        if(SaveGame.Load<bool>("[11]unlocked") == false)
        {
            GameObject ganondorfName = GameObject.Find("ganondorfName");
            ganondorfName.active = false; 
            Image image = fightersOnScreen[11].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[11]unlocked") == true) 
        {
            GameObject ganondorfName = GameObject.Find("ganondorfName");
            ganondorfName.active = true;           
        }

        if(SaveGame.Load<bool>("[12]unlocked") == false)
        {
            GameObject mewtwoName = GameObject.Find("mewtwoName");
            mewtwoName.active = false; 
            Image image = fightersOnScreen[12].GetComponent<Image>();
            image.color = new Color(0,0,0); 
        }
        
        if(SaveGame.Load<bool>("[12]unlocked") == true) 
        {
            GameObject mewtwoName = GameObject.Find("mewtwoName");
            mewtwoName.active = true;       
        }
        
        if(SaveGame.Load<bool>("[13]unlocked") == false)
        {
            GameObject zeldaName = GameObject.Find("zeldaName");
            zeldaName.active = false; 
            Image image = fightersOnScreen[13].GetComponent<Image>();
            image.color = new Color(0,0,0); 
        }
        
        if(SaveGame.Load<bool>("[13]unlocked") == true) 
        {
            GameObject zeldaName = GameObject.Find("zeldaName");
            zeldaName.active = true;        
        }

              
        if(SaveGame.Load<bool>("[14]unlocked") == false)
        {
            GameObject slimeName = GameObject.Find("slimeName");
            slimeName.active = false; 
            Image image = fightersOnScreen[14].GetComponent<Image>();
            image.color = new Color(0,0,0); 
        }
        
        if(SaveGame.Load<bool>("[14]unlocked") == true) 
        {
            GameObject slimeName = GameObject.Find("slimeName");
            slimeName.active = true;           
        }
                        
        if(SaveGame.Load<bool>("[15]unlocked") == false)
        {
            GameObject elvenName = GameObject.Find("elvenName");
            elvenName.active = false; 
            Image image = fightersOnScreen[15].GetComponent<Image>();
            image.color = new Color(0,0,0); 
        }
        
        if(SaveGame.Load<bool>("[15]unlocked") == true) 
        {
            GameObject elvenName = GameObject.Find("elvenName");
            elvenName.active = true;         
        }
                                
        if(SaveGame.Load<bool>("[16]unlocked") == false)
        {
            GameObject golemName = GameObject.Find("golemName");
            golemName.active = false; 
            Image image = fightersOnScreen[16].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[16]unlocked") == true) 
        {
            GameObject golemName = GameObject.Find("golemName");
            golemName.active = true;         
        }
        
        if(SaveGame.Load<bool>("[17]unlocked") == false)
        {
            GameObject rexName = GameObject.Find("rexName");
            rexName.active = false; 
            Image image = fightersOnScreen[17].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[17]unlocked") == true) 
        {
            GameObject rexName = GameObject.Find("rexName");
            rexName.active = true;          
        }

        if(SaveGame.Load<bool>("[18]unlocked") == false)
        {
            GameObject undeenName = GameObject.Find("undeenName");
            undeenName.active = false; 
            Image image = fightersOnScreen[18].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[18]unlocked") == true) 
        {
            GameObject undeenName = GameObject.Find("undeenName");
            undeenName.active = true;          
        }
        
        if(SaveGame.Load<bool>("[19]unlocked") == false)
        {
            GameObject ignisName = GameObject.Find("ignisName");
            ignisName.active = false; 
            Image image = fightersOnScreen[19].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[19]unlocked") == true) 
        {
            GameObject ignisName = GameObject.Find("ignisName");
            ignisName.active = true;            
        }
               
        if(SaveGame.Load<bool>("[20]unlocked") == false)
        {
            GameObject scorpionName = GameObject.Find("scorpionName");
            scorpionName.active = false; 
            Image image = fightersOnScreen[20].GetComponent<Image>();
            image.color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[20]unlocked") == true) 
        {
            GameObject scorpionName = GameObject.Find("scorpionName");
            scorpionName.active = true;       
        }
        

        
    }

    void Update()
    {
        SetActiveFighter();
    }

    void SetActiveFighter()
    {
        for(int i = 0; i<activeFightersTxt.Length; i++)
        {
            activeFightersTxt[i].text = "";
        }

        if(player.currentFighter == "Egyptian Mage")
        {
            unit = fighters[0].GetComponent<Unit>();
            activeFightersTxt[0].text = "Active Fighter";
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            
        }

        if(player.currentFighter == "Egyptian Warrior")
        {
            unit = fighters[1].GetComponent<Unit>();
            activeFightersTxt[1].text = "Active Fighter";
            SaveGame.Save<string>("currentfighter", player.currentFighter);
        }

        if(player.currentFighter == "Egyptian Sphinx")
        {
            unit = fighters[2].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[2].text = "Active Fighter";
        }

        if(player.currentFighter == "Mario")
        {
            unit = fighters[3].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[3].text = "Active Fighter";
        }

        if(player.currentFighter == "Charizard")
        {
            unit = fighters[4].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[4].text = "Active Fighter";
        }

        if(player.currentFighter == "Luigi")
        {
            unit = fighters[5].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[5].text = "Active Fighter";
        }

        if(player.currentFighter == "Link")
        {
            unit = fighters[6].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[6].text = "Active Fighter";
        }

        if(player.currentFighter == "Mew")
        {
            unit = fighters[7].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[7].text = "Active Fighter";
        }

        if(player.currentFighter == "Blue Eyes White Dragon")
        {
            unit = fighters[8].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[8].text = "Active Fighter";
        }

        if(player.currentFighter == "Dracula")
        {
            unit = fighters[9].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[9].text = "Active Fighter";
        }

        if(player.currentFighter == "Bowser")
        {
            unit = fighters[10].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[10].text = "Active Fighter";
        }

        if(player.currentFighter == "Ganondorf")
        {
            unit = fighters[11].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[11].text = "Active Fighter";
        }
        
        if(player.currentFighter == "Mewtwo")
        {
            unit = fighters[12].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[12].text = "Active Fighter";
        }
                
        if(player.currentFighter == "Princess Zelda")
        {
            unit = fighters[13].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[13].text = "Active Fighter";
        }
                        
        if(player.currentFighter == "Blood Slime")
        {
            unit = fighters[14].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[14].text = "Active Fighter";
        }
                                
        if(player.currentFighter == "Elven Naturess")
        {
            unit = fighters[15].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[15].text = "Active Fighter";
        }
                                        
        if(player.currentFighter == "Earth Gem Golem")
        {
            unit = fighters[16].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[16].text = "Active Fighter";
        }
                                                
        if(player.currentFighter == "Colossal Rex")
        {
            unit = fighters[17].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[17].text = "Active Fighter";
        }
                                                        
        if(player.currentFighter == "Undeen")
        {
            unit = fighters[18].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[18].text = "Active Fighter";
        }
                                                                
        if(player.currentFighter == "Ignis")
        {
            unit = fighters[19].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[19].text = "Active Fighter";
        }
                                                                        
        if(player.currentFighter == "Scorpion")
        {
            unit = fighters[20].GetComponent<Unit>();
            SaveGame.Save<string>("currentfighter", player.currentFighter);
            activeFightersTxt[20].text = "Active Fighter";
        }
        
    }

    public void SetEgyptMage()
    {
        unit = fighters[0].GetComponent<Unit>();
        
        if(unit.unlocked)
            player.currentFighter = "Egyptian Mage";
    }

    public void SetEgyptWarrior()
    {
        unit = fighters[1].GetComponent<Unit>();
        
        if(unit.unlocked)
            player.currentFighter = "Egyptian Warrior";
    }

    public void SetEgyptSphinx()
    {
        unit = fighters[2].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[2]unlocked") == true)
            player.currentFighter = "Egyptian Sphinx";
    }

    public void SetMario()
    {
        unit = fighters[3].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[3]unlocked") == true)
            player.currentFighter = "Mario";
    }

    public void SetCharizard()
    {
        unit = fighters[4].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[4]unlocked") == true)
            player.currentFighter = "Charizard";
    }

    public void SetLuigi()
    {
        unit = fighters[5].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[5]unlocked") == true)
            player.currentFighter = "Luigi";
    }

    public void SetLink()
    {
        unit = fighters[6].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[6]unlocked") == true)
            player.currentFighter = "Link";
    }

    public void SetMew()
    {
        unit = fighters[7].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[7]unlocked") == true)
            player.currentFighter = "Mew";
    }

    public void SetBlueEyes()
    {
        unit = fighters[8].GetComponent<Unit>();

        if(SaveGame.Load<bool>("[8]unlocked") == true)
            player.currentFighter = "Blue Eyes White Dragon";
    }

    public void SetDracula()
    {
        unit = fighters[9].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[9]unlocked") == true)
            player.currentFighter = "Dracula";
    }

    public void SetBowser()
    {
        unit = fighters[10].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[10]unlocked") == true)
            player.currentFighter = "Bowser";
    }

    public void SetGanondorf()
    {
        unit = fighters[11].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[11]unlocked") == true)
            player.currentFighter = "Ganondorf";
    }
    
    public void SetMewtwo()
    {
        unit = fighters[12].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[12]unlocked") == true)
            player.currentFighter = "Mewtwo";
    }
    
    public void SetZelda()
    {
        unit = fighters[13].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[13]unlocked") == true)
            player.currentFighter = "Princess Zelda";
    }
        
    public void SetBloodSlime()
    {
        unit = fighters[14].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[14]unlocked") == true)
            player.currentFighter = "Blood Slime";
    }
            
    public void SetElvenNaturess()
    {
        unit = fighters[15].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[15]unlocked") == true)
            player.currentFighter = "Elven Naturess";
    }
                
    public void SetEarthGemGolem()
    {
        unit = fighters[16].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[16]unlocked") == true)
            player.currentFighter = "Earth Gem Golem";
    }
                    
    public void SetColossalRex()
    {
        unit = fighters[17].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[17]unlocked") == true)
            player.currentFighter = "Colossal Rex";
    }
                        
    public void SetUndeen()
    {
        unit = fighters[18].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[18]unlocked") == true)
            player.currentFighter = "Undeen";
    }
                            
    public void SetIgnis()
    {
        unit = fighters[19].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[19]unlocked") == true)
            player.currentFighter = "Ignis";
    }
                                
    public void SetScorpion()
    {
        unit = fighters[20].GetComponent<Unit>();
        
        if(SaveGame.Load<bool>("[20]unlocked") == true)
            player.currentFighter = "Scorpion";
    }

    
}
