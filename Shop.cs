using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BayatGames.SaveGameFree; 

public class Shop : MonoBehaviour
{
    Player player; 

    public Text[] priceText;

    public Image[] fighterImage; 

    public Text playerCoinTxt; 

    void Start()
    {
        if(GameObject.Find("HomeSystem") != null)
            player = GameObject.Find("HomeSystem").GetComponent<Player>();
        else
            return; 
        
        if(SaveGame.Exists("clickcoins"))
            player.clickCoins = SaveGame.Load<int>("clickcoins"); 
        
        if(SaveGame.Exists("[13]unlocked"))
        {
            priceText[0].text = "UNLOCKED"; 
        }
        else
        {
            priceText[0].text = "350,000"; 
            
            if(player.clickCoins < 350000)
            {
                priceText[0].color = Color.red;
            }
            else
            {
                priceText[0].color = Color.white; 
            }
        }
        

        if(SaveGame.Exists("[14]unlocked"))
        {
            priceText[1].text = "UNLOCKED"; 
        }
        else
        {
            priceText[1].text = "150,000"; 
                        
            if(player.clickCoins < 150000)
            {
                priceText[1].color = Color.red;
            }
            else
            {
                priceText[1].color = Color.white; 
            }
        }
        
        if(SaveGame.Exists("[15]unlocked"))
        {
            priceText[2].text = "UNLOCKED"; 
        }
        else
        {
            priceText[2].text = "125,000"; 
                        
            if(player.clickCoins < 125000)
            {
                priceText[2].color = Color.red;
            }
            else
            {
                priceText[2].color = Color.white; 
            }
        }
        
        if(SaveGame.Exists("[16]unlocked"))
        {
            priceText[3].text = "UNLOCKED"; 
        }
        else
        {
            priceText[3].text = "250,000"; 
                        
            if(player.clickCoins < 250000)
            {
                priceText[3].color = Color.red;
            }
            else
            {
                priceText[3].color = Color.white; 
            }; 
        }
        
        if(SaveGame.Exists("[17]unlocked"))
        {
            priceText[4].text = "UNLOCKED"; 
        }
        else
        {
            priceText[4].text = "275,000"; 
                        
            if(player.clickCoins < 275000)
            {
                priceText[4].color = Color.red;
            }
            else
            {
                priceText[4].color = Color.white; 
            }
        }
                
        if(SaveGame.Exists("[18]unlocked"))
        {
            priceText[5].text = "UNLOCKED"; 
        }
        else
        {
            priceText[5].text = "225,000";
                        
            if(player.clickCoins < 225000)
            {
                priceText[5].color = Color.red;
            }
            else
            {
                priceText[5].color = Color.white; 
            } 
        }
                        
        if(SaveGame.Exists("[19]unlocked"))
        {
            priceText[6].text = "UNLOCKED"; 
        }
        else
        {
            priceText[6].text = "350,000";
                        
            if(player.clickCoins < 350000)
            {
                priceText[6].color = Color.red;
            }
            else
            {
                priceText[6].color = Color.white; 
            } 
        }
                        
        if(SaveGame.Exists("[20]unlocked"))
        {
            priceText[7].text = "UNLOCKED"; 
        }
        else
        {
            priceText[7].text = "500,000"; 
                        
            if(player.clickCoins < 500000)
            {
                priceText[7].color = Color.red;
            }
            else
            {
                priceText[7].color = Color.white; 
            }
        }

        HideShowFighters();
            
    }

    void HideShowFighters()
    {
        if(SaveGame.Load<bool>("[13]shop") == true)
        {
            GameObject zeldaName = GameObject.Find("zeldaName");
            zeldaName.active = true; 
            fighterImage[0].color = new Color(230,230,230);
        }
        else
        {
            GameObject zeldaName = GameObject.Find("zeldaName");
            zeldaName.active = false; 
            fighterImage[0].color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[14]shop") == true)
        {
            GameObject slimeName = GameObject.Find("slimeName");
            slimeName.active = true; 
            fighterImage[1].color = new Color(230,230,230);
        }
        else
        {
            GameObject slimeName = GameObject.Find("slimeName");
            slimeName.active = false; 
            fighterImage[1].color = new Color(0,0,0);
        }
        
        if(SaveGame.Load<bool>("[15]shop") == true)
        {
            GameObject elvenName = GameObject.Find("elvenName");
            elvenName.active = true; 
            fighterImage[2].color = new Color(230,230,230);
        }
        else
        {
            GameObject elvenName = GameObject.Find("elvenName");
            elvenName.active = false; 
            fighterImage[2].color = new Color(0,0,0);
        }
                
        if(SaveGame.Load<bool>("[16]shop") == true)
        {
            GameObject golemName = GameObject.Find("golemName");
            golemName.active = true; 
            fighterImage[3].color = new Color(230,230,230);
        }
        else
        {
            GameObject golemName = GameObject.Find("golemName");
            golemName.active = false; 
            fighterImage[3].color = new Color(0,0,0);
        }

                
        if(SaveGame.Load<bool>("[17]shop") == true)
        {
            GameObject rexName = GameObject.Find("rexName");
            rexName.active = true; 
            fighterImage[4].color = new Color(230,230,230);
        }
        else
        {
            GameObject rexName = GameObject.Find("rexName");
            rexName.active = false; 
            fighterImage[4].color = new Color(0,0,0);
        }
                        
        if(SaveGame.Load<bool>("[18]shop") == true)
        {
            GameObject undeenName = GameObject.Find("undeenName");
            undeenName.active = true; 
            fighterImage[5].color = new Color(230,230,230);
        }
        else
        {
            GameObject undeenName = GameObject.Find("undeenName");
            undeenName.active = false; 
            fighterImage[5].color = new Color(0,0,0);
        }
                                
        if(SaveGame.Load<bool>("[19]shop") == true)
        {
            GameObject ignisName = GameObject.Find("ignisName");
            ignisName.active = true; 
            fighterImage[6].color = new Color(230,230,230);
        }
        else
        {
            GameObject ignisName = GameObject.Find("ignisName");
            ignisName.active = false; 
            fighterImage[6].color = new Color(0,0,0);
        }
                                        
        if(SaveGame.Load<bool>("[20]shop") == true)
        {
            GameObject scorpionName = GameObject.Find("scorpionName");
            scorpionName.active = true; 
            fighterImage[7].color = new Color(230,230,230);
        }
        else
        {
            GameObject scorpionName = GameObject.Find("scorpionName");
            scorpionName.active = false; 
            fighterImage[7].color = new Color(0,0,0);
        }




    }
    void Update()
    {
        playerCoinTxt.text = "" + player.clickCoins; 
        SaveGame.Save<int>("clickcoins", player.clickCoins); 


    }

    public void BuyZelda()
    {
        if(SaveGame.Exists("[13]shop"))
        {
            if(SaveGame.Load<bool>("[13]shop") == true)
            {
                int buyPrice = 350000; 
                
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[13]unlocked") == false)
                {
                    SaveGame.Save<bool>("[13]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[0].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }

    public void BuyBloodSlime()
    {
        if(SaveGame.Exists("[14]shop"))
        {
            if(SaveGame.Load<bool>("[14]shop") == true)
            {
                int buyPrice = 150000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[14]unlocked") == false)
                {
                    SaveGame.Save<bool>("[14]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[1].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }
    
    public void BuyNaturess()
    {
        if(SaveGame.Exists("[15]shop"))
        {
            if(SaveGame.Load<bool>("[15]shop") == true)
            {
                int buyPrice = 125000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[15]unlocked") == false)
                {
                    SaveGame.Save<bool>("[15]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[2].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }

    public void BuyGemGolem()
    {
        if(SaveGame.Exists("[16]shop"))
        {
            if(SaveGame.Load<bool>("[16]shop") == true)
            {
                int buyPrice = 250000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[16]unlocked") == false)
                {
                    SaveGame.Save<bool>("[16]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[3].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }
    
    public void BuyRex()
    {
        if(SaveGame.Exists("[17]shop"))
        {
            if(SaveGame.Load<bool>("[17]shop") == true)
            {
                int buyPrice = 275000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[17]unlocked") == false)
                {
                    SaveGame.Save<bool>("[17]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[4].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }
    
    public void BuyUndeen()
    {
        if(SaveGame.Exists("[18]shop"))
        {
            if(SaveGame.Load<bool>("[18]shop") == true)
            {
                int buyPrice = 225000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[18]unlocked") == false)
                {
                    SaveGame.Save<bool>("[18]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[5].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }
        
    public void BuyIgnis()
    {
        if(SaveGame.Exists("[19]shop"))
        {
            if(SaveGame.Load<bool>("[19]shop") == true)
            {
                int buyPrice = 350000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[19]unlocked") == false)
                {
                    SaveGame.Save<bool>("[19]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[6].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }
            
    public void BuyScorpion()
    {
        if(SaveGame.Exists("[20]shop"))
        {
            if(SaveGame.Load<bool>("[20]shop") == true)
            {
                int buyPrice = 500000; 
                if(player.clickCoins >= buyPrice && SaveGame.Load<bool>("[20]unlocked") == false)
                {
                    SaveGame.Save<bool>("[20]unlocked", true);
                    player.clickCoins = player.clickCoins - buyPrice; 
                    priceText[7].text = "UNLOCKED";
                    SaveGame.Save<int>("clickcoins", player.clickCoins); 
                }
            }
        }
        else
        {
            print("unlock this fighter first");
        } 
    }
    


}
