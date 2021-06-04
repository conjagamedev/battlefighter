using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using UnityEditor; 
using UnityEngine.Advertisements;

using BayatGames.SaveGameFree;

public enum BattleState {START, playerTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public GameObject playerSpot;  
    public GameObject enemySpot;

    public GameObject[] fighters; 
    public GameObject[] enemies; 
    [Space(20)]

    public GameObject playerFighters; 
    public GameObject currFighter; 
    public Unit currFighterUnit; 

    Transform playerSpotTrans;
    Transform enemySpotTrans; 

    Player playerScript; 
    Enemy enemyScript;  

    Unit playerUnit; 
    Unit enemyUnit; 

    public BattleState state; 

    public Text dialogText; 

    GameObject playerGO; 
    GameObject enemyGO; 

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD; 

    public Button atkBtn; 
    public Button healBtn; 

    public Text dmgText;
    public Slider gameSpeed; 

    public GameObject dmgSpot; 
    GameObject victoryObject; 
    Animation victoryAnim; 
    Text victoryText; 

    int earnXPAmount; 

    HomeDisplayFighter homeDisplayFighter;

    bool earnedFighter; 
    bool fighterInShop; 

    int fightCount; 

    bool isMuted; 

    public AudioSource atkAudio; 
 

    void Start()
    {
        playerSpotTrans = playerSpot.GetComponent<Transform>();
        enemySpotTrans = enemySpot.GetComponent<Transform>();
        playerScript = GameObject.Find("HomeSystem").GetComponent<Player>();
        enemyScript = GetComponent<Enemy>();
        victoryObject = GameObject.Find("victoryOverlayImg");
        victoryAnim = victoryObject.GetComponent<Animation>(); 
        victoryText = GameObject.Find("victoryTxt").GetComponent<Text>();
        victoryObject.active = false;
        homeDisplayFighter = GameObject.Find("HomeSystem").GetComponent<HomeDisplayFighter>();

        if(SaveGame.Exists("fightcount"))   
            fightCount = SaveGame.Load<int>("fightcount"); 
                    
        if(SaveGame.Exists("ismuted"))
            isMuted = SaveGame.Load<bool>("ismuted");



        state = BattleState.START;

        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        dialogText.text = "...";
        atkBtn.enabled = false; 
        healBtn.enabled = false; 
        AssignplayerFighter();
        LoadFighter();
        currFighterUnit.currentHP = currFighterUnit.maxHP;
        FetchEnemyFighter();
        AssignEnemyFighter();
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        playerGO.active = false;
        enemyGO.active = false; 
        Advertisement.Banner.Hide();
        gameSpeed.value = Time.timeScale;
        yield return new WaitForSeconds(1.5f);
        playerGO.active = true;
        enemyGO.active = true; 
        dialogText.text = "A " +enemyUnit.unitName + " approaches..."; 
        yield return new WaitForSeconds(2f);
        
        if(playerUnit.speed > enemyUnit.speed)
        {
            state = BattleState.playerTURN; 
            playerTurn();
        }
        
        if(playerUnit.speed < enemyUnit.speed)
        {
            state = BattleState.ENEMYTURN; 
            StartCoroutine(EnemyTurn());
        }

        if(playerUnit.speed == enemyUnit.speed)
        {
            int rand = Random.Range(0,2);
            if(rand == 0)
            {
                state = BattleState.playerTURN;
                playerTurn();
            }
            if(rand == 1)
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
    }

    void playerTurn()
    {
        dialogText.text = "Your turn, make your move!";
        atkBtn.enabled = true; 
        healBtn.enabled = true; 
    }

    public void OnAttackButton()
    {
        if(state != BattleState.playerTURN)
            return;
        
        StartCoroutine(playerAttack());
        atkBtn.enabled = false; 
        healBtn.enabled = false; 
    }
    
    public void OnHealButton()
    {
        if(state != BattleState.playerTURN)
            return;
        
        StartCoroutine(playerHeal());
        atkBtn.enabled = false;
        healBtn.enabled = false; 
    }

    IEnumerator playerAttack()
    {
        playerUnit.animator.SetBool("isAttack", true);
        
        if(!isMuted)
            atkAudio.Play(); 
        
        yield return new WaitForSeconds(0.5f);
        dmgText.text = "-" + playerUnit.attack;
        Instantiate(dmgText, dmgSpot.transform);
        yield return new WaitForSeconds(0.2f);
        playerUnit.animator.SetBool("isAttack", false);
        
        bool isDead = enemyUnit.TakeDamage(playerUnit.attack);
        enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit);
        
        dialogText.text = "Successful attack!";

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator playerHeal()
    {
        dialogText.text = playerUnit.unitName + " heals for " + playerUnit.heal + " HP";
        yield return new WaitForSeconds(0.5f);
        dmgText.text = "+" + playerUnit.heal; 
        Instantiate(dmgText, dmgSpot.transform);
        yield return new WaitForSeconds(0.2f);
        playerUnit.Heal();
        playerHUD.SetHP(playerUnit.currentHP, playerUnit);
        yield return new WaitForSeconds(2f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        
        if(enemyUnit.currentHP < enemyUnit.maxHP / 2)
        {
            int action = Random.Range(0,2);

            if(action == 0)
            {
                dialogText.text = enemyUnit.unitName + " attacks!";
                yield return new WaitForSeconds(0.5f);
                enemyUnit.animator.SetBool("isAttack", true);
                                
                if(!isMuted)
                    atkAudio.Play(); 
                     
                yield return new WaitForSeconds(0.5f);
                dmgText.text = "-" + enemyUnit.attack;
                Instantiate(dmgText, dmgSpot.transform);
                yield return new WaitForSeconds(0.5f);
                dialogText.text = "Enemy landed attack succesfully!";
                enemyUnit.animator.SetBool("isAttack", false); 
                bool isDead = playerUnit.TakeDamage(enemyUnit.attack);
                playerHUD.SetHP(playerUnit.currentHP, playerUnit);
                yield return new WaitForSeconds(1f);

                 if(isDead)
                {
                    state = BattleState.LOST;
                    StartCoroutine(EndBattle());
                }
                else 
                {
                    state = BattleState.playerTURN;
                    playerTurn();
                }
            }
            if(action == 1)
            {
                dialogText.text = enemyUnit.unitName + " heals for " + enemyUnit.heal + " HP!";
                yield return new WaitForSeconds(0.5f);
                dmgText.text = "+" + enemyUnit.heal; 
                Instantiate(dmgText, dmgSpot.transform);
                yield return new WaitForSeconds(0.2f);
                enemyUnit.Heal();
                enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit);
                yield return new WaitForSeconds(1f);
                state = BattleState.playerTURN;
                playerTurn();


            }
        }
        else
        {
            dialogText.text = enemyUnit.unitName + " attacks!";
            yield return new WaitForSeconds(0.5f);
            enemyUnit.animator.SetBool("isAttack", true);
                    
            if(!isMuted)
                atkAudio.Play(); 

            yield return new WaitForSeconds(0.5f);
            dmgText.text = "-" + enemyUnit.attack;
            Instantiate(dmgText, dmgSpot.transform);
            yield return new WaitForSeconds(0.5f);
            dialogText.text = "Enemy landed attack succesfully!";
            enemyUnit.animator.SetBool("isAttack", false); 
            bool isDead = playerUnit.TakeDamage(enemyUnit.attack);
            playerHUD.SetHP(playerUnit.currentHP, playerUnit);
            yield return new WaitForSeconds(1f);

            if(isDead)
            {
                state = BattleState.LOST;
                StartCoroutine(EndBattle());
            }
            else 
            {
                state = BattleState.playerTURN;
                playerTurn();
            }
        }
    }

    IEnumerator EndBattle()
    {
        if(state == BattleState.WON)
        {
            EarnedNewplayer();
            NewShopFighter();
            currFighterUnit.LevelUp(earnXPAmount);
            dialogText.text = "You gained " + earnXPAmount + " XP!";
            victoryObject.active = true;

            if(fightCount < 7)
            {
                fightCount = fightCount + 1; 
                SaveGame.Save<int>("fightcount", fightCount); 
            }
            else
            {
                fightCount = 1; 
                SaveGame.Save<int>("fightcount", fightCount); 
            }

            if(!earnedFighter && !fighterInShop)
                victoryText.text = "VICTORY";                
            else if(!earnedFighter && fighterInShop && enemyScript.currentEnemy != "Ganondorf")
                victoryText.text = enemyScript.currentEnemy + " NOW IN SHOP"; 
            else if(!earnedFighter && fighterInShop && enemyScript.currentEnemy == "Ganondorf")
                victoryText.text = "ZELDA NOW IN SHOP"; 
            else
                victoryText.text = enemyScript.currentEnemy + " UNLOCKED";

            victoryAnim.Play(); 
            SaveFighter();
            yield return new WaitForSeconds(2f);
            Destroy(GameObject.Find("HomeSystem"));
            SceneManager.LoadScene("homeScene", LoadSceneMode.Single);
        }
        else if (state == BattleState.LOST)
        {
            dialogText.text = "You flee from the fighter!";
            victoryObject.active = true;
            victoryText.text = "DEFEATED"; 
            victoryAnim.Play();
            yield return new WaitForSeconds(2f);
            Destroy(GameObject.Find("HomeSystem"));
            SceneManager.LoadScene("homeScene", LoadSceneMode.Single);
        }
    }

    void AssignplayerFighter()
    {
        if(playerScript.currentFighter == "Egyptian Warrior")
        { 
            //playerGO = Instantiate(fighters[0], playerSpotTrans);
            playerGO = fighters[0];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[0];
            currFighterUnit = fighters[0].GetComponent<Unit>();
        }
        if(playerScript.currentFighter == "Egyptian Mage")
        {
           // playerGO = Instantiate(fighters[1], playerSpotTrans);
            playerGO = fighters[1];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[1];
            currFighterUnit = fighters[1].GetComponent<Unit>();
        }
        if(playerScript.currentFighter == "Egyptian Sphinx")
        {
            playerGO = fighters[2];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[2];
            currFighterUnit = fighters[2].GetComponent<Unit>();
        }

        if(playerScript.currentFighter == "Mario")
        {
            playerGO = fighters[3];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[3];
            currFighterUnit = fighters[3].GetComponent<Unit>();
        }

        
        if(playerScript.currentFighter == "Charizard")
        {
            playerGO = fighters[4];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[4];
            currFighterUnit = fighters[4].GetComponent<Unit>();
        }

        if(playerScript.currentFighter == "Luigi")
        {
            playerGO = fighters[5];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[5];
            currFighterUnit = fighters[5].GetComponent<Unit>();
        }

        if(playerScript.currentFighter == "Link")
        {
            playerGO = fighters[6];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[6];
            currFighterUnit = fighters[6].GetComponent<Unit>();
        }

        if(playerScript.currentFighter == "Mew")
        {
            playerGO = fighters[7];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[7];
            currFighterUnit = fighters[7].GetComponent<Unit>(); 
        }
        
        if(playerScript.currentFighter == "Blue Eyes White Dragon")
        {
            playerGO = fighters[8];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[8];
            currFighterUnit = fighters[8].GetComponent<Unit>(); 
        }

        if(playerScript.currentFighter == "Dracula")
        {
            playerGO = fighters[9];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[9];
            currFighterUnit = fighters[9].GetComponent<Unit>(); 
        }

        if(playerScript.currentFighter == "Bowser")
        {
            playerGO = fighters[10];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[10];
            currFighterUnit = fighters[10].GetComponent<Unit>(); 
        }

        if(playerScript.currentFighter == "Ganondorf")
        {
            playerGO = fighters[11];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[11];
            currFighterUnit = fighters[11].GetComponent<Unit>(); 
        }

        if(playerScript.currentFighter == "Mewtwo")
        {
            playerGO = fighters[12];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[12];
            currFighterUnit = fighters[12].GetComponent<Unit>(); 
        }

        if(playerScript.currentFighter == "Princess Zelda")
        {
            playerGO = fighters[13];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[13];
            currFighterUnit = fighters[13].GetComponent<Unit>(); 
        }
        
        
        if(playerScript.currentFighter == "Blood Slime")
        {
            playerGO = fighters[14];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[14];
            currFighterUnit = fighters[14].GetComponent<Unit>(); 
        }
       
        if(playerScript.currentFighter == "Elven Naturess")
        {
            playerGO = fighters[15];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[15];
            currFighterUnit = fighters[15].GetComponent<Unit>(); 
        }
              
        if(playerScript.currentFighter == "Earth Gem Golem")
        {
            playerGO = fighters[16];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[16];
            currFighterUnit = fighters[16].GetComponent<Unit>(); 
        }
                     
        if(playerScript.currentFighter == "Colossal Rex")
        {
            playerGO = fighters[17];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[17];
            currFighterUnit = fighters[17].GetComponent<Unit>(); 
        }
                             
        if(playerScript.currentFighter == "Undeen")
        {
            playerGO = fighters[18];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[18];
            currFighterUnit = fighters[18].GetComponent<Unit>(); 
        }
                                     
        if(playerScript.currentFighter == "Ignis")
        {
            playerGO = fighters[19];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[19];
            currFighterUnit = fighters[19].GetComponent<Unit>(); 
        }
                                             
        if(playerScript.currentFighter == "Scorpion")
        {
            playerGO = fighters[20];
            playerGO.transform.parent = playerSpotTrans;
            playerGO.transform.localPosition = Vector3.zero; 
            currFighter = fighters[20];
            currFighterUnit = fighters[20].GetComponent<Unit>(); 
        }


        playerUnit = playerGO.GetComponent<Unit>();
    }

    void FetchEnemyFighter()
    {
        if(SaveGame.Exists("battlelvl"))
        {
            if(playerUnit.lvl == 50)
            {
                int random; 

                if(SaveGame.Load<int>("battlelvl") == 1) //lvl1-10
                {
                    random = Random.Range(1,15);

                    if(random == 1)
                        enemyScript.currentEnemy =  "Birdy Brown";
                        
                    if(random == 2)
                        enemyScript.currentEnemy =  "Birdy Pink";
                        
                    if(random == 3)
                        enemyScript.currentEnemy =  "Melee Birdy"; 
                
                    if(random == 4)
                        enemyScript.currentEnemy =  "Purp";
                        
                    if(random == 5)
                        enemyScript.currentEnemy =  "Pryno";
                    
                    if(random == 6)
                        enemyScript.currentEnemy =  "Prynid";
                        
                    if(random == 7)
                        enemyScript.currentEnemy =  "Gold Knight";
                    
                    if(random == 8)
                        enemyScript.currentEnemy = "Red Knight";

                    if(random <= 9)
                        enemyScript.currentEnemy = "Master Pink Bird";

                    if(random == 10)
                        enemyScript.currentEnemy = "Horned Bunny";
                    
                    if(random == 11)
                        enemyScript.currentEnemy = "Horned Lizard";
                    
                    if(random == 12)
                        enemyScript.currentEnemy = "Mario";
                                            
                    if(random == 13)
                        enemyScript.currentEnemy = "Egyptian Mummy";
                    
                    if(random == 14)
                        enemyScript.currentEnemy = "Egyptian Sphinx";
                }
            
                if(SaveGame.Load<int>("battlelvl") == 2) //lvl11-20
                {
                    random = Random.Range(1,22);

                    if(random == 1)
                        enemyScript.currentEnemy = "Luigi"; 
                                            
                    if(random == 2)
                        enemyScript.currentEnemy = "Elven Spellcaster"; 

                    if(random == 3)
                        enemyScript.currentEnemy = "Elven Archer";
                    
                    if(random == 4)
                        enemyScript.currentEnemy = "Elven Assassin";
                    
                    if(random == 5)
                        enemyScript.currentEnemy = "Elven Warrior";

                    if(random == 6)
                        enemyScript.currentEnemy = "Elven Rogue";

                    if(random == 7)
                        enemyScript.currentEnemy = "Egyptian Archer";

                    if(random == 8)
                        enemyScript.currentEnemy = "Egyptian Crocodile";

                    if(random == 9)
                        enemyScript.currentEnemy = "Egyptian Cobra";

                    if(random == 10)
                        enemyScript.currentEnemy = "Elven Naturess";
                    
                    if(random == 11)
                        enemyScript.currentEnemy = "Psyrena";
     
                    if(random == 12)
                    {
                        enemyScript.currentEnemy = "Blood Mage";
                    }
                    if(random == 13)
                    {
                        enemyScript.currentEnemy = "Blood Wolf";
                    }
                    if(random == 14)
                    {
                        enemyScript.currentEnemy = "Blood Slime";
                    }
                    if(random == 15)
                    {
                        enemyScript.currentEnemy = "Goomba";
                    }
                    if(random == 16)
                    {
                        enemyScript.currentEnemy = "Dark Devil Imp";
                    }
                    if(random == 17)
                    {
                        enemyScript.currentEnemy = "Dark Banshee";
                    }
                    if(random == 18)
                    {
                        enemyScript.currentEnemy = "Dark Angel";
                    }
                    if(random == 19)
                    {
                        enemyScript.currentEnemy = "Dark Wisp"; 
                    }
                    if(random == 20)
                    {
                        enemyScript.currentEnemy = "Gibdo";
                    }
                    if(random == 21)
                    {
                        enemyScript.currentEnemy = "Charizard";
                    }
                }

                if(SaveGame.Load<int>("battlelvl") == 3)//lvl21-30
                {
                    random = Random.Range(1,20); 

                    if(random == 1)
                        enemyScript.currentEnemy = "Link"; 
                    
                    if(random == 2)
                        enemyScript.currentEnemy = "Mew"; 
                        
                    if(random == 3)
                    {
                        enemyScript.currentEnemy = "Knuckles";
                    }

                    if(random == 4)
                    {
                        enemyScript.currentEnemy = "Wart";
                    }

                    if(random == 5)
                    {
                        enemyScript.currentEnemy = "Skulltula";
                    }

                    if(random == 6)
                    {
                        enemyScript.currentEnemy = "Earth Bull";
                    }

                    if(random == 7)
                    {
                        enemyScript.currentEnemy = "Earth Lion";
                    }

                    if(random == 8)
                    {
                        enemyScript.currentEnemy = "Earth Turtle";
                    }
                    if(random == 9)
                    {
                        enemyScript.currentEnemy = "Earth Mandrake";
                    }
                    if(random == 10)
                    {
                        enemyScript.currentEnemy = "Earth Snake";
                    }
                    if(random == 11)
                    {
                        enemyScript.currentEnemy = "Earth Wisp";
                    }
                    if(random == 12)
                    {
                        enemyScript.currentEnemy = "Earth Gem Golem"; 
                    }
                    
                    if(random == 13)
                        enemyScript.currentEnemy = "Colossal Hydra";
                    
                    if(random == 14)
                        enemyScript.currentEnemy = "Colossal Cobra";
                    
                    if(random == 15)
                        enemyScript.currentEnemy = "Colossal Rex";
                    
                    if(random == 16)
                        enemyScript.currentEnemy = "The Creature"; 

                    if(random == 17)
                        enemyScript.currentEnemy = "Raiden";


                    if(random == 18)
                        enemyScript.currentEnemy = "Red Wyvern";
                    

                    if(random == 19)
                        enemyScript.currentEnemy = "Oriental Dragon";

                }

                if(SaveGame.Load<int>("battlelvl") == 4)//lvl31-40
                {
                    random = Random.Range(1,20);

                    if(random == 1)
                        enemyScript.currentEnemy = "Blue Eyes White Dragon";  

                    if(random == 2) 
                        enemyScript.currentEnemy = "White Dragon King";    

                    if(random == 3)
                        enemyScript.currentEnemy = "Dragon Wyrm"; 

                    if(random == 4)
                        enemyScript.currentEnemy = "Dracula";    
                        
                    if(random == 5)
                        enemyScript.currentEnemy = "Jonathan Morris";

                    if(random == 6)
                        enemyScript.currentEnemy = "Snake";

                    if(random == 7)
                        enemyScript.currentEnemy = "Excalibur Sonic";

                    if(random == 8)
                        enemyScript.currentEnemy = "God Jupiter";

                    if(random == 9)
                        enemyScript.currentEnemy = "Goddess Aphrodite";
                    
                    if(random == 10)
                        enemyScript.currentEnemy = "God Poseidon";

                    if(random == 11)
                        enemyScript.currentEnemy = "God Hades";

                    if(random == 12)
                        enemyScript.currentEnemy = "God Dagon";

                    if(random == 13)
                        enemyScript.currentEnemy = "Goddess Airi";  
                        
                    if(random == 14)
                        enemyScript.currentEnemy = "Gohma";  
                                                
                    if(random == 15)
                        enemyScript.currentEnemy = "Lugia";   
                                                                        
                    if(random == 16)
                        enemyScript.currentEnemy = "Seto Kaiba"; 
                                                
                    if(random == 17)
                        enemyScript.currentEnemy = "Undeen";  
                                                
                    if(random == 18)
                        enemyScript.currentEnemy = "Volvagia";   
                                                                        
                    if(random == 19)
                        enemyScript.currentEnemy = "Zant";        
                }

                if(SaveGame.Load<int>("battlelvl") == 5)//lv41-50
                {
                    random = Random.Range(1,29);

                    if(random == 1)
                        enemyScript.currentEnemy = "Scorpion"; 

                    if(random == 2)
                        enemyScript.currentEnemy = "Bowser Jr";

                    if(random == 3)
                        enemyScript.currentEnemy = "Zeograth"; 

                    if(random == 4)
                        enemyScript.currentEnemy = "The Horde";

                    if(random == 5)
                        enemyScript.currentEnemy = "Spirit Fighter";

                    if(random == 6)
                        enemyScript.currentEnemy = "Urmica"; 

                    if(random == 7)
                        enemyScript.currentEnemy = "Mecha Drake";
                    
                    if(random == 8)
                        enemyScript.currentEnemy = "Feral Kitsune";
                        
                    if(random == 9)
                        enemyScript.currentEnemy = "Ancient Priestess";

                    if(random == 10)
                        enemyScript.currentEnemy = "Majora"; 

                    if(random == 11)
                        enemyScript.currentEnemy = "Rayquaza"; 

                    if(random == 12)
                        enemyScript.currentEnemy = "Death"; 
                    
                    if(random == 13)
                        enemyScript.currentEnemy = "The Werewolf";
                    
                    if(random == 14)
                        enemyScript.currentEnemy = "Tiamat"; 
                    
                    if(random == 15)
                        enemyScript.currentEnemy = "Sabre Gargoyle"; 
                    
                    if(random == 16)
                        enemyScript.currentEnemy = "Son of Valhalla";
                    
                    if(random == 17)
                        enemyScript.currentEnemy = "Xoer";

                    if(random == 18)
                        enemyScript.currentEnemy = "Sorceress Duessa";
                    
                    if(random == 19)
                        enemyScript.currentEnemy = "Abomination";
                    
                    if(random == 20)
                        enemyScript.currentEnemy = "Rukkha";

                    if(random == 21)
                        enemyScript.currentEnemy = "Lich King"; 
                    
                    if(random == 22)
                        enemyScript.currentEnemy = "Phantom Ganon"; 
                    
                    if(random == 23)
                        enemyScript.currentEnemy = "Sans"; 
                    
                    if(random == 24)
                        enemyScript.currentEnemy = "Toriel"; 
                    
                    if(random == 25)
                        enemyScript.currentEnemy = "Goro"; 
                    
                    if(random == 26)
                        enemyScript.currentEnemy = "Frisk"; 
                    
                    if(random == 27)
                        enemyScript.currentEnemy = "Nuckaleeve"; 
                    
                    if(random == 28)
                        enemyScript.currentEnemy = "Ignis"; 

                }
            }
            else
            {
                #region LV1-10
                int rand; 
                

                if(playerUnit.lvl == 1)
                {
                    enemyScript.currentEnemy = "Birdy Brown";
                }
                
                if(playerUnit.lvl == 2)
                {
                    rand = Random.Range(0,2);
                    if(rand == 0)
                        enemyScript.currentEnemy = "Birdy Pink";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Melee Birdy";
                }
                
                if(playerUnit.lvl == 3)
                {
                    rand = Random.Range(0,2);
                    if(rand == 0)
                        enemyScript.currentEnemy = "Melee Birdy";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Purp";
                }
                
                if(playerUnit.lvl == 4)
                {
                    rand = Random.Range(0,2);
                    if(rand == 0)
                        enemyScript.currentEnemy = "Pryno";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Prynid";
                }
                
                if(playerUnit.lvl == 5)
                {
                    rand = Random.Range(0,2);
                    if(rand == 0)
                        enemyScript.currentEnemy = "Prynid";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Pryno";
                }
                
                if(playerUnit.lvl == 6)
                {
                    rand = Random.Range(0,2);
                    if(rand == 0)
                        enemyScript.currentEnemy = "Gold Knight";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Red Knight";
                }  

                if(playerUnit.lvl == 7)
                {
                    rand = Random.Range(0,5);
                    if(rand <= 3)
                        enemyScript.currentEnemy = "Master Pink Bird";
                    if(rand == 4)
                        enemyScript.currentEnemy = "Horned Bunny";
                }

                if(playerUnit.lvl == 8)
                {
                    rand = Random.Range(0,2);
                    if(rand == 0)
                        enemyScript.currentEnemy = "Horned Bunny";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Horned Lizard";
                }
                if(playerUnit.lvl == 9)
                {
                    rand = Random.Range(0,2);
                    if((SaveGame.Load<bool>("[2]unlocked") == true) && SaveGame.Load<bool>("[3]unlocked") == false)
                    {
                        enemyScript.currentEnemy = "Mario";
                    }
                    else
                    {
                        if(rand == 0)
                            enemyScript.currentEnemy = "Egyptian Mummy";
                        if(rand == 1)
                            enemyScript.currentEnemy = "Horned Lizard";
                    }
                }

                if(playerUnit.lvl == 10)
                {
                    
                    if((SaveGame.Load<bool>("[2]unlocked") == false))
                    {
                        enemyScript.currentEnemy = "Egyptian Sphinx";
                    }
                    else
                    {
                        enemyScript.currentEnemy = "Egyptian Mummy";
                    }
                    

                }

                #endregion

                #region LV11-20
                if(playerUnit.lvl >= 11 && playerUnit.lvl <= 15)
                {
                    rand = Random.Range(0,10);
                    
                    if(SaveGame.Load<bool>("[4]unlocked") == true && SaveGame.Load<bool>("[5]unlocked") == false && rand == 0)
                    {
                        enemyScript.currentEnemy = "Luigi";
                    }
                    if(SaveGame.Load<bool>("[4]unlocked") == true && SaveGame.Load<bool>("[5]unlocked") == true && rand == 0)
                    {
                        rand = Random.Range(1,10);
                    }
                    else 
                    {
                        rand = Random.Range(1,10);
                    }
                    
                    if(rand == 1)
                        enemyScript.currentEnemy = "Elven Spellcaster"; 

                    if(rand == 2)
                        enemyScript.currentEnemy = "Elven Archer";
                    
                    if(rand == 3)
                        enemyScript.currentEnemy = "Elven Assassin";
                    
                    if(rand == 4)
                        enemyScript.currentEnemy = "Elven Warrior";

                    if(rand == 5)
                        enemyScript.currentEnemy = "Elven Rogue";

                    if(rand == 6)
                        enemyScript.currentEnemy = "Egyptian Archer";

                    if(rand == 7)
                        enemyScript.currentEnemy = "Egyptian Crocodile";

                    if(rand == 8)
                        enemyScript.currentEnemy = "Egyptian Cobra";

                    if(rand == 9)
                        enemyScript.currentEnemy = "Elven Naturess";
                }

                if(playerUnit.lvl == 15) 
                {
                    if(SaveGame.Load<bool>("[4]unlocked") == true && SaveGame.Load<bool>("[5]unlocked") == false)
                    {
                        enemyScript.currentEnemy = "Luigi";
                    }
                    else
                    {
                        rand = Random.Range(0,10);
                    
                        if(rand == 0)
                            enemyScript.currentEnemy = "Psyrena";
                    
                        if(rand == 1)
                            enemyScript.currentEnemy = "Elven Spellcaster"; 

                        if(rand == 2)
                            enemyScript.currentEnemy = "Elven Archer";
                    
                        if(rand == 3)
                            enemyScript.currentEnemy = "Elven Assassin";
                    
                        if(rand == 4)
                            enemyScript.currentEnemy = "Elven Warrior";

                        if(rand == 5)
                            enemyScript.currentEnemy = "Elven Rogue";

                        if(rand == 6)
                            enemyScript.currentEnemy = "Egyptian Archer";

                        if(rand == 7)
                            enemyScript.currentEnemy = "Egyptian Crocodile";

                        if(rand == 8)
                            enemyScript.currentEnemy = "Egyptian Cobra";

                        if(rand == 9)
                            enemyScript.currentEnemy = "Elven Naturess";
                    }
                }


                if(playerUnit.lvl >= 16 && playerUnit.lvl <= 20)
                {
                    rand = Random.Range(0,10);
                    
                    if(SaveGame.Load<bool>("[4]unlocked") == false && rand == 0)
                    {
                        enemyScript.currentEnemy = "Charizard";
                    }
                    else if(SaveGame.Load<bool>("[4]unlocked") == true && rand == 0)
                    {
                        rand = Random.Range(1,10);
                    }
                    else
                    {
                        rand = Random.Range(1,10);               
                    }

                    if(rand == 1)
                    {
                        enemyScript.currentEnemy = "Blood Mage";
                    }
                    if(rand == 2)
                    {
                        enemyScript.currentEnemy = "Blood Wolf";
                    }
                    if(rand == 3)
                    {
                        enemyScript.currentEnemy = "Blood Slime";
                    }
                    if(rand == 4)
                    {
                        enemyScript.currentEnemy = "Goomba";
                    }
                    if(rand == 5)
                    {
                        enemyScript.currentEnemy = "Dark Devil Imp";
                    }
                    if(rand == 6)
                    {
                        enemyScript.currentEnemy = "Dark Banshee";
                    }
                    if(rand == 7)
                    {
                        enemyScript.currentEnemy = "Dark Angel";
                    }
                    if(rand == 8)
                    {
                        enemyScript.currentEnemy = "Dark Wisp"; 
                    }
                    if(rand == 9)
                    {
                        enemyScript.currentEnemy = "Gibdo";
                    }
                }

                #endregion

                #region LV21-32
                if(playerUnit.lvl >= 21 && playerUnit.lvl <= 26) // LEVEL 21-26 
                {
                    rand = Random.Range(0,12); 
                    
                    if(rand == 0)
                    {
                        if(SaveGame.Load<bool>("[6]unlocked") == false)
                        {
                            enemyScript.currentEnemy = "Link"; 
                        }
                        else
                        {
                            rand = Random.Range(2,12); 
                        }

                    }

                    if(rand == 1)
                    {
                        if(SaveGame.Load<bool>("[7]unlocked") == false)
                        {
                            enemyScript.currentEnemy = "Mew"; 
                        }
                        else
                        {
                            rand = Random.Range(2,12);
                        }
                    }

                    if(rand == 2)
                    {
                        enemyScript.currentEnemy = "Knuckles";
                    }

                    if(rand == 3)
                    {
                        enemyScript.currentEnemy = "Wart";
                    }

                    if(rand == 4)
                    {
                        enemyScript.currentEnemy = "Skulltula";
                    }

                    if(rand == 5)
                    {
                        enemyScript.currentEnemy = "Earth Bull";
                    }

                    if(rand == 6)
                    {
                        enemyScript.currentEnemy = "Earth Lion";
                    }

                    if(rand == 7)
                    {
                        enemyScript.currentEnemy = "Earth Turtle";
                    }
                    if(rand == 8)
                    {
                        enemyScript.currentEnemy = "Earth Mandrake";
                    }
                    if(rand == 9)
                    {
                        enemyScript.currentEnemy = "Earth Snake";
                    }
                    if(rand == 10)
                    {
                        enemyScript.currentEnemy = "Earth Wisp";
                    }
                    if(rand == 11)
                    {
                        enemyScript.currentEnemy = "Earth Gem Golem"; 
                    }
                }
                
                if(playerUnit.lvl >= 27 && playerUnit.lvl <= 32)
                {
                    rand = Random.Range(0,10);

                    if(SaveGame.Load<bool>("[8]unlocked") == false && rand == 0)
                    {
                        enemyScript.currentEnemy = "Blue Eyes White Dragon";
                    }
                    else if(SaveGame.Load<bool>("[8]unlocked") == true && rand == 0)
                    {
                        rand = Random.Range(1,10);
                    }

                    if(rand == 1)
                        enemyScript.currentEnemy = "Colossal Hydra";
                    
                    if(rand == 2)
                        enemyScript.currentEnemy = "Colossal Cobra";
                    
                    if(rand == 3)
                        enemyScript.currentEnemy = "Colossal Rex";
                    
                    if(rand == 4)
                        enemyScript.currentEnemy = "The Creature"; 

                    if(rand == 5)
                        enemyScript.currentEnemy = "Raiden";

                    if(rand == 6)
                        enemyScript.currentEnemy = "Dragon Wyrm";

                    if(rand == 7)
                        enemyScript.currentEnemy = "Red Wyvern";
                    
                    if(rand == 8)
                        enemyScript.currentEnemy = "White Dragon King";

                    if(rand == 9)
                        enemyScript.currentEnemy = "Oriental Dragon";
                }

                #endregion

                #region LV33-44
                if(playerUnit.lvl >= 33 && playerUnit.lvl <= 38)
                {
                    rand = Random.Range(0,10);

                    if(SaveGame.Load<bool>("[9]unlocked") == false && rand == 0)
                    {
                        enemyScript.currentEnemy = "Dracula";
                    }
                    else if(SaveGame.Load<bool>("[9]unlocked") == true && rand == 0)
                    {
                        rand = Random.Range(1,10);
                    }

                    if(rand == 1)
                        enemyScript.currentEnemy = "Jonathan Morris";

                    if(rand == 2)
                        enemyScript.currentEnemy = "Snake";

                    if(rand == 3)
                        enemyScript.currentEnemy = "Excalibur Sonic";

                    if(rand == 4)
                        enemyScript.currentEnemy = "God Jupiter";

                    if(rand == 5)
                        enemyScript.currentEnemy = "Goddess Aphrodite";
                    
                    if(rand == 6)
                        enemyScript.currentEnemy = "God Poseidon";

                    if(rand == 7)
                        enemyScript.currentEnemy = "God Hades";

                    if(rand == 8)
                        enemyScript.currentEnemy = "God Dagon";

                    if(rand == 9)
                        enemyScript.currentEnemy = "Goddess Airi";                
                }

                if(playerUnit.lvl >= 39 && playerUnit.lvl <= 44)
                {
                    rand = Random.Range(0,16); 

                    if(SaveGame.Load<bool>("[10]unlocked") == false && rand == 0)
                    {
                        enemyScript.currentEnemy = "Bowser";
                    }
                    else
                    {
                        rand = Random.Range(1,16);
                    }

                    if(rand == 1)
                        enemyScript.currentEnemy = "Seto Kaiba";
                    
                    if(rand == 2)
                        enemyScript.currentEnemy = "Volvagia";

                    if(rand == 3)
                        enemyScript.currentEnemy = "Gohma"; 

                    if(rand == 4)
                        enemyScript.currentEnemy = "Lugia";

                    if(rand == 5)
                        enemyScript.currentEnemy = "Scorpion"; 

                    if(rand == 6)
                        enemyScript.currentEnemy = "Bowser Jr";

                    if(rand == 7)
                        enemyScript.currentEnemy = "Zeograth"; 

                    if(rand == 8)
                        enemyScript.currentEnemy = "The Horde";

                    if(rand == 9)
                        enemyScript.currentEnemy = "Spirit Fighter";

                    if(rand == 10)
                        enemyScript.currentEnemy = "Urmica"; 

                    if(rand == 11)
                        enemyScript.currentEnemy = "Sea Leviathan"; 
                    
                    if(rand == 12)
                        enemyScript.currentEnemy = "Undeen"; 

                    if(rand == 13)
                        enemyScript.currentEnemy = "Mecha Drake";
                    
                    if(rand == 14)
                        enemyScript.currentEnemy = "Feral Kitsune";

                    if(rand == 15)
                        enemyScript.currentEnemy = "Zant";
                }

                #endregion

                #region LV45-50 
                {
                    if(playerUnit.lvl >= 45 && playerUnit.lvl <= 50)
                    {
                        rand = Random.Range(0,21);

                        if(SaveGame.Load<bool>("[11]unlocked") == false && rand == 0)
                        {
                            enemyScript.currentEnemy = "Ganondorf"; 
                        }
                        else if(SaveGame.Load<bool>("[11]unlocked") == true && SaveGame.Load<bool>("[12]unlocked") == false && rand == 0)
                        {
                            enemyScript.currentEnemy = "Mewtwo"; 
                        }
                        else
                        {
                            rand = Random.Range(1,21);
                        }

                        if(rand == 1)
                            enemyScript.currentEnemy = "Ancient Priestess";

                        if(rand == 2)
                            enemyScript.currentEnemy = "Majora"; 

                        if(rand == 3)
                            enemyScript.currentEnemy = "Rayquaza"; 

                        if(rand == 4)
                            enemyScript.currentEnemy = "Death"; 
                        
                        if(rand == 5)
                            enemyScript.currentEnemy = "The Werewolf";
                        
                        if(rand == 6)
                            enemyScript.currentEnemy = "Tiamat"; 
                        
                        if(rand == 7)
                            enemyScript.currentEnemy = "Sabre Gargoyle"; 
                        
                        if(rand == 8)
                            enemyScript.currentEnemy = "Son of Valhalla";
                        
                        if(rand == 9)
                            enemyScript.currentEnemy = "Xoer";

                        if(rand == 10)
                            enemyScript.currentEnemy = "Sorceress Duessa";
                        
                        if(rand == 11)
                            enemyScript.currentEnemy = "Abomination";
                        
                        if(rand == 12)
                            enemyScript.currentEnemy = "Rukkha";

                        if(rand == 13)
                            enemyScript.currentEnemy = "Lich King"; 
                        
                        if(rand == 14)
                            enemyScript.currentEnemy = "Phantom Ganon"; 
                        
                        if(rand == 15)
                            enemyScript.currentEnemy = "Sans"; 
                        
                        if(rand == 16)
                            enemyScript.currentEnemy = "Toriel"; 
                        
                        if(rand == 17)
                            enemyScript.currentEnemy = "Goro"; 
                        
                        if(rand == 18)
                            enemyScript.currentEnemy = "Frisk"; 
                        
                        if(rand == 19)
                            enemyScript.currentEnemy = "Nuckaleeve"; 
                        
                        if(rand == 20)
                            enemyScript.currentEnemy = "Ignis"; 
                    }
                } 
                #endregion
            }
        }
        else
        {
            #region LV1-10
            int rand; 
            

            if(playerUnit.lvl == 1)
            {
                enemyScript.currentEnemy = "Birdy Brown";
            }
            
            if(playerUnit.lvl == 2)
            {
                rand = Random.Range(0,2);
                if(rand == 0)
                    enemyScript.currentEnemy = "Birdy Pink";
                if(rand == 1)
                    enemyScript.currentEnemy = "Melee Birdy";
            }
            
            if(playerUnit.lvl == 3)
            {
                rand = Random.Range(0,2);
                if(rand == 0)
                    enemyScript.currentEnemy = "Melee Birdy";
                if(rand == 1)
                    enemyScript.currentEnemy = "Purp";
            }
            
            if(playerUnit.lvl == 4)
            {
                rand = Random.Range(0,2);
                if(rand == 0)
                    enemyScript.currentEnemy = "Pryno";
                if(rand == 1)
                    enemyScript.currentEnemy = "Prynid";
            }
            
            if(playerUnit.lvl == 5)
            {
                rand = Random.Range(0,2);
                if(rand == 0)
                    enemyScript.currentEnemy = "Prynid";
                if(rand == 1)
                    enemyScript.currentEnemy = "Pryno";
            }
            
            if(playerUnit.lvl == 6)
            {
                rand = Random.Range(0,2);
                if(rand == 0)
                    enemyScript.currentEnemy = "Gold Knight";
                if(rand == 1)
                    enemyScript.currentEnemy = "Red Knight";
            }  

            if(playerUnit.lvl == 7)
            {
                rand = Random.Range(0,5);
                if(rand <= 3)
                    enemyScript.currentEnemy = "Master Pink Bird";
                if(rand == 4)
                    enemyScript.currentEnemy = "Horned Bunny";
            }

            if(playerUnit.lvl == 8)
            {
                rand = Random.Range(0,2);
                if(rand == 0)
                    enemyScript.currentEnemy = "Horned Bunny";
                if(rand == 1)
                    enemyScript.currentEnemy = "Horned Lizard";
            }
            if(playerUnit.lvl == 9)
            {
                rand = Random.Range(0,2);
                if((SaveGame.Load<bool>("[2]unlocked") == true) && SaveGame.Load<bool>("[3]unlocked") == false)
                {
                    enemyScript.currentEnemy = "Mario";
                }
                else
                {
                    if(rand == 0)
                        enemyScript.currentEnemy = "Egyptian Mummy";
                    if(rand == 1)
                        enemyScript.currentEnemy = "Horned Lizard";
                }
            }

            if(playerUnit.lvl == 10)
            {
                
                if((SaveGame.Load<bool>("[2]unlocked") == false))
                {
                    enemyScript.currentEnemy = "Egyptian Sphinx";
                }
                else
                {
                    enemyScript.currentEnemy = "Egyptian Mummy";
                }
                

            }

            #endregion

            #region LV11-20
            if(playerUnit.lvl >= 11 && playerUnit.lvl <= 15)
            {
                rand = Random.Range(0,10);
                
                if(SaveGame.Load<bool>("[4]unlocked") == true && SaveGame.Load<bool>("[5]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Luigi";
                }
                if(SaveGame.Load<bool>("[4]unlocked") == true && SaveGame.Load<bool>("[5]unlocked") == true && rand == 0)
                {
                    rand = Random.Range(1,10);
                }
                else 
                {
                    rand = Random.Range(1,10);
                }
                
                if(rand == 1)
                    enemyScript.currentEnemy = "Elven Spellcaster"; 

                if(rand == 2)
                    enemyScript.currentEnemy = "Elven Archer";
                
                if(rand == 3)
                    enemyScript.currentEnemy = "Elven Assassin";
                
                if(rand == 4)
                    enemyScript.currentEnemy = "Elven Warrior";

                if(rand == 5)
                    enemyScript.currentEnemy = "Elven Rogue";

                if(rand == 6)
                    enemyScript.currentEnemy = "Egyptian Archer";

                if(rand == 7)
                    enemyScript.currentEnemy = "Egyptian Crocodile";

                if(rand == 8)
                    enemyScript.currentEnemy = "Egyptian Cobra";

                if(rand == 9)
                    enemyScript.currentEnemy = "Elven Naturess";
            }

            if(playerUnit.lvl == 15) 
            {
                if(SaveGame.Load<bool>("[4]unlocked") == true && SaveGame.Load<bool>("[5]unlocked") == false)
                {
                    enemyScript.currentEnemy = "Luigi";
                }
                else
                {
                    rand = Random.Range(0,10);
                
                    if(rand == 0)
                        enemyScript.currentEnemy = "Psyrena";
                
                    if(rand == 1)
                        enemyScript.currentEnemy = "Elven Spellcaster"; 

                    if(rand == 2)
                        enemyScript.currentEnemy = "Elven Archer";
                
                    if(rand == 3)
                        enemyScript.currentEnemy = "Elven Assassin";
                
                    if(rand == 4)
                        enemyScript.currentEnemy = "Elven Warrior";

                    if(rand == 5)
                        enemyScript.currentEnemy = "Elven Rogue";

                    if(rand == 6)
                        enemyScript.currentEnemy = "Egyptian Archer";

                    if(rand == 7)
                        enemyScript.currentEnemy = "Egyptian Crocodile";

                    if(rand == 8)
                        enemyScript.currentEnemy = "Egyptian Cobra";

                    if(rand == 9)
                        enemyScript.currentEnemy = "Elven Naturess";
                }
            }


            if(playerUnit.lvl >= 16 && playerUnit.lvl <= 20)
            {
                rand = Random.Range(0,10);
                
                if(SaveGame.Load<bool>("[4]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Charizard";
                }
                else if(SaveGame.Load<bool>("[4]unlocked") == true && rand == 0)
                {
                    rand = Random.Range(1,10);
                }
                else
                {
                    rand = Random.Range(1,10);               
                }

                if(rand == 1)
                {
                    enemyScript.currentEnemy = "Blood Mage";
                }
                if(rand == 2)
                {
                    enemyScript.currentEnemy = "Blood Wolf";
                }
                if(rand == 3)
                {
                    enemyScript.currentEnemy = "Blood Slime";
                }
                if(rand == 4)
                {
                    enemyScript.currentEnemy = "Goomba";
                }
                if(rand == 5)
                {
                    enemyScript.currentEnemy = "Dark Devil Imp";
                }
                if(rand == 6)
                {
                    enemyScript.currentEnemy = "Dark Banshee";
                }
                if(rand == 7)
                {
                    enemyScript.currentEnemy = "Dark Angel";
                }
                if(rand == 8)
                {
                    enemyScript.currentEnemy = "Dark Wisp"; 
                }
                if(rand == 9)
                {
                    enemyScript.currentEnemy = "Gibdo";
                }
            }

            #endregion

            #region LV21-32
            if(playerUnit.lvl >= 21 && playerUnit.lvl <= 26) // LEVEL 21-26 
            {
                rand = Random.Range(0,12); 

                if(rand == 0)
                {
                    if(SaveGame.Load<bool>("[6]unlocked") == false)
                    {
                        enemyScript.currentEnemy = "Link"; 
                    }
                    else
                    {
                        rand = Random.Range(2,12); 
                    }

                }

                if(rand == 1)
                {
                    if(SaveGame.Load<bool>("[7]unlocked") == false)
                    {
                        enemyScript.currentEnemy = "Mew"; 
                    }
                    else
                    {
                        rand = Random.Range(2,12);
                    }
                }

                if(rand == 2)
                {
                    enemyScript.currentEnemy = "Knuckles";
                }

                if(rand == 3)
                {
                    enemyScript.currentEnemy = "Wart";
                }

                if(rand == 4)
                {
                    enemyScript.currentEnemy = "Skulltula";
                }

                if(rand == 5)
                {
                    enemyScript.currentEnemy = "Earth Bull";
                }

                if(rand == 6)
                {
                    enemyScript.currentEnemy = "Earth Lion";
                }

                if(rand == 7)
                {
                    enemyScript.currentEnemy = "Earth Turtle";
                }
                if(rand == 8)
                {
                    enemyScript.currentEnemy = "Earth Mandrake";
                }
                if(rand == 9)
                {
                    enemyScript.currentEnemy = "Earth Snake";
                }
                if(rand == 10)
                {
                    enemyScript.currentEnemy = "Earth Wisp";
                }
                if(rand == 11)
                {
                    enemyScript.currentEnemy = "Earth Gem Golem"; 
                }
            }
            
            if(playerUnit.lvl >= 27 && playerUnit.lvl <= 32)
            {
                rand = Random.Range(0,10);

                if(SaveGame.Load<bool>("[8]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Blue Eyes White Dragon";
                }
                else if(SaveGame.Load<bool>("[8]unlocked") == true && rand == 0)
                {
                    rand = Random.Range(1,10);
                }

                if(rand == 1)
                    enemyScript.currentEnemy = "Colossal Hydra";
                
                if(rand == 2)
                    enemyScript.currentEnemy = "Colossal Cobra";
                
                if(rand == 3)
                    enemyScript.currentEnemy = "Colossal Rex";
                
                if(rand == 4)
                    enemyScript.currentEnemy = "The Creature"; 

                if(rand == 5)
                    enemyScript.currentEnemy = "Raiden";

                if(rand == 6)
                    enemyScript.currentEnemy = "Dragon Wyrm";

                if(rand == 7)
                    enemyScript.currentEnemy = "Red Wyvern";
                
                if(rand == 8)
                    enemyScript.currentEnemy = "White Dragon King";

                if(rand == 9)
                    enemyScript.currentEnemy = "Oriental Dragon";
            }

            #endregion

            #region LV33-44
            if(playerUnit.lvl >= 33 && playerUnit.lvl <= 38)
            {
                rand = Random.Range(0,10);

                if(SaveGame.Load<bool>("[9]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Dracula";
                }
                else if(SaveGame.Load<bool>("[9]unlocked") == true && rand == 0)
                {
                    rand = Random.Range(1,10);
                }

                if(rand == 1)
                    enemyScript.currentEnemy = "Jonathan Morris";

                if(rand == 2)
                    enemyScript.currentEnemy = "Snake";

                if(rand == 3)
                    enemyScript.currentEnemy = "Excalibur Sonic";

                if(rand == 4)
                    enemyScript.currentEnemy = "God Jupiter";

                if(rand == 5)
                    enemyScript.currentEnemy = "Goddess Aphrodite";
                
                if(rand == 6)
                    enemyScript.currentEnemy = "God Poseidon";

                if(rand == 7)
                    enemyScript.currentEnemy = "God Hades";

                if(rand == 8)
                    enemyScript.currentEnemy = "God Dagon";

                if(rand == 9)
                    enemyScript.currentEnemy = "Goddess Airi";                
            }

            if(playerUnit.lvl >= 39 && playerUnit.lvl <= 44)
            {
                rand = Random.Range(0,16); 

                if(SaveGame.Load<bool>("[10]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Bowser";
                }
                else
                {
                    rand = Random.Range(1,16);
                }

                if(rand == 1)
                    enemyScript.currentEnemy = "Seto Kaiba";
                
                if(rand == 2)
                    enemyScript.currentEnemy = "Volvagia";

                if(rand == 3)
                    enemyScript.currentEnemy = "Gohma"; 

                if(rand == 4)
                    enemyScript.currentEnemy = "Lugia";

                if(rand == 5)
                    enemyScript.currentEnemy = "Scorpion"; 

                if(rand == 6)
                    enemyScript.currentEnemy = "Bowser Jr";

                if(rand == 7)
                    enemyScript.currentEnemy = "Zeograth"; 

                if(rand == 8)
                    enemyScript.currentEnemy = "The Horde";

                if(rand == 9)
                    enemyScript.currentEnemy = "Spirit Fighter";

                if(rand == 10)
                    enemyScript.currentEnemy = "Urmica"; 

                if(rand == 11)
                    enemyScript.currentEnemy = "Sea Leviathan"; 
                
                if(rand == 12)
                    enemyScript.currentEnemy = "Undeen"; 

                if(rand == 13)
                    enemyScript.currentEnemy = "Mecha Drake";
                
                if(rand == 14)
                    enemyScript.currentEnemy = "Feral Kitsune";

                if(rand == 15)
                    enemyScript.currentEnemy = "Zant";
            }

            #endregion

            #region LV45-50
            if(playerUnit.lvl >= 45 && playerUnit.lvl <= 50)
            {
                rand = Random.Range(0,21);

                if(SaveGame.Load<bool>("[11]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Ganondorf"; 
                }
                else if(SaveGame.Load<bool>("[11]unlocked") == true && SaveGame.Load<bool>("[12]unlocked") == false && rand == 0)
                {
                    enemyScript.currentEnemy = "Mewtwo"; 
                }
                else
                {
                    rand = Random.Range(1,21);
                }

                if(rand == 1)
                    enemyScript.currentEnemy = "Ancient Priestess";

                if(rand == 2)
                    enemyScript.currentEnemy = "Majora"; 

                if(rand == 3)
                    enemyScript.currentEnemy = "Rayquaza"; 

                if(rand == 4)
                    enemyScript.currentEnemy = "Death"; 
                
                if(rand == 5)
                    enemyScript.currentEnemy = "The Werewolf";
                
                if(rand == 6)
                    enemyScript.currentEnemy = "Tiamat"; 
                
                if(rand == 7)
                    enemyScript.currentEnemy = "Sabre Gargoyle"; 
                
                if(rand == 8)
                    enemyScript.currentEnemy = "Son of Valhalla";
                
                if(rand == 9)
                    enemyScript.currentEnemy = "Xoer";

                if(rand == 10)
                    enemyScript.currentEnemy = "Sorceress Duessa";
                
                if(rand == 11)
                    enemyScript.currentEnemy = "Abomination";
                
                if(rand == 12)
                    enemyScript.currentEnemy = "Rukkha";

                if(rand == 13)
                    enemyScript.currentEnemy = "Lich King"; 
                
                if(rand == 14)
                    enemyScript.currentEnemy = "Phantom Ganon"; 
                
                if(rand == 15)
                    enemyScript.currentEnemy = "Sans"; 
                
                if(rand == 16)
                    enemyScript.currentEnemy = "Toriel"; 
                
                if(rand == 17)
                    enemyScript.currentEnemy = "Goro"; 
                
                if(rand == 18)
                    enemyScript.currentEnemy = "Frisk"; 
                
                if(rand == 19)
                    enemyScript.currentEnemy = "Nuckaleeve"; 
                
                if(rand == 20)
                    enemyScript.currentEnemy = "Ignis"; 
        }

        #endregion
        }
        //detects whether player is lvl 50 and if not lvl 50, follow function to grab player level and throw out enemies dependent on current level (1-10 if player lvl 1-10 etc.)
        //if not and the player is lvl 50, it detects whether the button on homepage has been clicked before, if not - it carries player to normal 44-50, however possible for 
        //player to begin clicking button on home button to swap the levels they want to fight at with their level 50 to potentially create a "farming" method to the game of 
        //finding fighters at certain levels that never appeared whilst previously powering up a fighter!
    }

    void AssignEnemyFighter()
    {
        #region LVL1-10
        if(enemyScript.currentEnemy == "Birdy Brown")
        {
            enemyGO = Instantiate(enemies[0], enemySpotTrans);
            earnXPAmount = Random.Range(50,70);
        }

        if(enemyScript.currentEnemy == "Birdy Pink")
        {
            enemyGO = Instantiate(enemies[1], enemySpotTrans);
            earnXPAmount = Random.Range(70,100);
        }
        
        if(enemyScript.currentEnemy == "Melee Birdy")
        {
            enemyGO = Instantiate(enemies[2], enemySpotTrans);
            earnXPAmount = Random.Range(100,150);
        }

        if(enemyScript.currentEnemy == "Purp")
        {
            enemyGO = Instantiate(enemies[3], enemySpotTrans);
            earnXPAmount = Random.Range(100,150);
        }

        if(enemyScript.currentEnemy == "Pryno")
        {
            enemyGO = Instantiate(enemies[4], enemySpotTrans);
            earnXPAmount = Random.Range(100,150);
        }
        
        if(enemyScript.currentEnemy == "Prynid")
        {
            enemyGO = Instantiate(enemies[5], enemySpotTrans);
            earnXPAmount = Random.Range(160,200);
        }

        if(enemyScript.currentEnemy == "Gold Knight")
        {
            enemyGO = Instantiate(enemies[6], enemySpotTrans);
            earnXPAmount = Random.Range(160,220);
        }
        
        if(enemyScript.currentEnemy == "Red Knight")
        {
            enemyGO = Instantiate(enemies[7], enemySpotTrans);
            earnXPAmount = Random.Range(160,220);
        }

        if(enemyScript.currentEnemy == "Master Pink Bird")
        {
            enemyGO = Instantiate(enemies[8], enemySpotTrans);
            earnXPAmount = Random.Range(180,200);
        }

        if(enemyScript.currentEnemy == "Horned Bunny")
        {
            enemyGO = Instantiate(enemies[9], enemySpotTrans);
            earnXPAmount = Random.Range(220,240);
        }

        if(enemyScript.currentEnemy == "Horned Lizard")
        {
            enemyGO = Instantiate(enemies[10], enemySpotTrans);
            earnXPAmount = Random.Range(200,250);
        }

        if(enemyScript.currentEnemy == "Egyptian Mummy")
        {
            enemyGO = Instantiate(enemies[11], enemySpotTrans);
            earnXPAmount = Random.Range(200,220);
        }

        if(enemyScript.currentEnemy == "Egyptian Sphinx")
        {
            enemyGO = Instantiate(enemies[12], enemySpotTrans);
            earnXPAmount = Random.Range(400,450);
        }

        if(enemyScript.currentEnemy == "Mario")
        {
            enemyGO = Instantiate(enemies[13], enemySpotTrans);
            earnXPAmount = Random.Range(500,550);
        }
        #endregion

        #region LVL11-16
        if(enemyScript.currentEnemy == "Psyrena")
        {
            enemyGO = Instantiate(enemies[14], enemySpotTrans);
            earnXPAmount = Random.Range(300,400);
        }

        if(enemyScript.currentEnemy == "Elven Spellcaster")
        {
            enemyGO = Instantiate(enemies[15], enemySpotTrans);
            earnXPAmount = Random.Range(250,350);
        }

        if(enemyScript.currentEnemy == "Elven Archer")
        {
            enemyGO = Instantiate(enemies[16], enemySpotTrans);
            earnXPAmount = Random.Range(250,350);
        }

        if(enemyScript.currentEnemy == "Elven Assassin")
        {
            enemyGO = Instantiate(enemies[17], enemySpotTrans);
            earnXPAmount = Random.Range(250, 350); 
        }

        if(enemyScript.currentEnemy == "Elven Warrior")
        {
            enemyGO = Instantiate(enemies[18], enemySpotTrans);
            earnXPAmount = Random.Range(250, 350);
        }

        if(enemyScript.currentEnemy == "Elven Naturess")
        {
            enemyGO = Instantiate(enemies[19], enemySpotTrans);
            earnXPAmount = Random.Range(400,600);
        }

        if(enemyScript.currentEnemy == "Elven Rogue")
        {
            enemyGO = Instantiate(enemies[20], enemySpotTrans);
            earnXPAmount = Random.Range(250,350);
        }

        if(enemyScript.currentEnemy == "Egyptian Archer")
        {
            enemyGO = Instantiate(enemies[21], enemySpotTrans);
            earnXPAmount = Random.Range(250,350);
        }

        if(enemyScript.currentEnemy == "Egyptian Crocodile")
        {
            enemyGO = Instantiate(enemies[22], enemySpotTrans);
            earnXPAmount = Random.Range(250,350);
        }

        if(enemyScript.currentEnemy == "Egyptian Cobra")
        {
            enemyGO = Instantiate(enemies[23], enemySpotTrans); 
            earnXPAmount = Random.Range(250,350);
        }

        if(enemyScript.currentEnemy == "Charizard")
        {
            enemyGO = Instantiate(enemies[24], enemySpotTrans);
            earnXPAmount = Random.Range(900,1100);
        }
        if(enemyScript.currentEnemy == "Luigi")
        {
            enemyGO = Instantiate(enemies[25], enemySpotTrans);
            earnXPAmount = Random.Range(600,700);
        }
        #endregion 

        #region LVL17-20
        if(enemyScript.currentEnemy == "Goomba")
        {
            enemyGO = Instantiate(enemies[26], enemySpotTrans);
            earnXPAmount = Random.Range(350,400); 
        }
        if(enemyScript.currentEnemy == "Gibdo")
        {
            enemyGO = Instantiate(enemies[27], enemySpotTrans);
            earnXPAmount = Random.Range(350,400);
        }
        if(enemyScript.currentEnemy == "Blood Mage")
        {
            enemyGO = Instantiate(enemies[28], enemySpotTrans);
            earnXPAmount = Random.Range(350,400);
        }

        if(enemyScript.currentEnemy == "Blood Wolf")
        {
            enemyGO = Instantiate(enemies[29], enemySpotTrans);
            earnXPAmount = Random.Range(350,450);
        }
        if(enemyScript.currentEnemy == "Blood Slime")
        {
            enemyGO = Instantiate(enemies[30], enemySpotTrans);
            earnXPAmount = Random.Range(350,450);
        }
        if(enemyScript.currentEnemy == "Dark Devil Imp")
        {
            enemyGO = Instantiate(enemies[31], enemySpotTrans);
            earnXPAmount = Random.Range(450,500);
        }
        if(enemyScript.currentEnemy == "Dark Banshee")
        {
            enemyGO = Instantiate(enemies[32], enemySpotTrans);
            earnXPAmount = Random.Range(450,500);
        }
        if(enemyScript.currentEnemy == "Dark Angel")
        {
            enemyGO = Instantiate(enemies[33], enemySpotTrans);
            earnXPAmount = Random.Range(450,500);
        }
        if(enemyScript.currentEnemy == "Dark Ogre")
        {
            enemyGO = Instantiate(enemies[34],enemySpotTrans); 
            earnXPAmount = Random.Range(450,500);
        }
        if(enemyScript.currentEnemy == "Dark Wisp")
        {
            enemyGO = Instantiate(enemies[35],enemySpotTrans);
            earnXPAmount = Random.Range(350,450);
        }
        #endregion

        #region LVL21-26
        if(enemyScript.currentEnemy == "Link")
        {
            enemyGO = Instantiate(enemies[36], enemySpotTrans);
            earnXPAmount = Random.Range(1000,1300);
        }
        if(enemyScript.currentEnemy == "Knuckles")
        {
            enemyGO = Instantiate(enemies[37], enemySpotTrans); 
            earnXPAmount = Random.Range(700,800);
        }
        if(enemyScript.currentEnemy == "Wart")
        {
            enemyGO = Instantiate(enemies[38], enemySpotTrans);
            earnXPAmount = Random.Range(1400,1600);
        }
        if(enemyScript.currentEnemy == "Skulltula")
        {
            enemyGO = Instantiate(enemies[39], enemySpotTrans); 
            earnXPAmount = Random.Range(1300,1500); 
        }
        if(enemyScript.currentEnemy == "Earth Bull")
        {
            enemyGO = Instantiate(enemies[40], enemySpotTrans);
            earnXPAmount = Random.Range(1400,1600);
        }
        if(enemyScript.currentEnemy == "Earth Lion")
        {
            enemyGO = Instantiate(enemies[41], enemySpotTrans);
            earnXPAmount = Random.Range(1400,1600);
        }
        if(enemyScript.currentEnemy == "Earth Turtle")
        {
            enemyGO = Instantiate(enemies[42], enemySpotTrans); 
            earnXPAmount = Random.Range(2800,2900);
        }
        if(enemyScript.currentEnemy == "Earth Mandrake")
        {
            enemyGO = Instantiate(enemies[43], enemySpotTrans);
            earnXPAmount = Random.Range(2400,2600); 
        }
        if(enemyScript.currentEnemy == "Earth Snake")
        {
            enemyGO = Instantiate(enemies[44], enemySpotTrans);
            earnXPAmount = Random.Range(2100,2200); 
        }
        if(enemyScript.currentEnemy == "Earth Wisp")
        {
            enemyGO = Instantiate(enemies[45], enemySpotTrans);
            earnXPAmount = Random.Range(2100,2200); 
        }
        if(enemyScript.currentEnemy == "Earth Gem Golem")
        {
            enemyGO = Instantiate(enemies[46], enemySpotTrans);
            earnXPAmount = Random.Range(2700,2800);
        }
        if(enemyScript.currentEnemy == "Mew")
        {
            enemyGO = Instantiate(enemies[47], enemySpotTrans); 
            earnXPAmount = Random.Range(3000,3200);
        }
        #endregion

        #region LVL27-32
        if(enemyScript.currentEnemy == "Colossal Hydra")
        {
            enemyGO = Instantiate(enemies[48], enemySpotTrans);
            earnXPAmount = Random.Range(2800,2900);
        }
        
        if(enemyScript.currentEnemy == "Colossal Cobra")
        {
            enemyGO = Instantiate(enemies[49], enemySpotTrans);
            earnXPAmount = Random.Range(2800,2900);
        }

        if(enemyScript.currentEnemy == "Colossal Rex")
        {
            enemyGO = Instantiate(enemies[50], enemySpotTrans);
            earnXPAmount = Random.Range(2800,2900);
        }
        
        if(enemyScript.currentEnemy == "Colossal Scorpion")
        {
            enemyGO = Instantiate(enemies[51], enemySpotTrans);
            earnXPAmount = Random.Range(2800,2900);
        }

        if(enemyScript.currentEnemy == "Blue Eyes White Dragon")
        {
            enemyGO = Instantiate(enemies[52], enemySpotTrans);
            earnXPAmount = Random.Range(3900,4200);
        }
        
        if(enemyScript.currentEnemy == "The Creature")
        {
            enemyGO = Instantiate(enemies[53], enemySpotTrans); 
            earnXPAmount = Random.Range(320,3400); 
        }

        if(enemyScript.currentEnemy == "Raiden")
        {
            enemyGO = Instantiate(enemies[54], enemySpotTrans); 
            earnXPAmount = Random.Range(3200,3400); 
        }

        if(enemyScript.currentEnemy == "Dragon Wyrm")
        {
            enemyGO = Instantiate(enemies[55], enemySpotTrans); 
            earnXPAmount = Random.Range(3100,3300); 
        }

        if(enemyScript.currentEnemy == "Red Wyvern")
        {
            enemyGO = Instantiate(enemies[56], enemySpotTrans); 
            earnXPAmount = Random.Range(3100,3300); 
        }
        if(enemyScript.currentEnemy == "White Dragon King")
        {
            enemyGO = Instantiate(enemies[57], enemySpotTrans); 
            earnXPAmount = Random.Range(3900,4200);
        }

        if(enemyScript.currentEnemy == "Oriental Dragon")
        {
            enemyGO = Instantiate(enemies[58], enemySpotTrans); 
            earnXPAmount = Random.Range(2800,2900);
        }
        #endregion

        #region LV33-38
        if(enemyScript.currentEnemy == "Jonathan Morris")
        {
            enemyGO = Instantiate(enemies[59], enemySpotTrans); 
            earnXPAmount = Random.Range(4100,4300);
        }

        if(enemyScript.currentEnemy == "Snake")
        {
            enemyGO = Instantiate(enemies[60], enemySpotTrans); 
            earnXPAmount = Random.Range(4300,4500);
        }

        if(enemyScript.currentEnemy == "Excalibur Sonic")
        {
            enemyGO = Instantiate(enemies[61], enemySpotTrans); 
            earnXPAmount = Random.Range(4100,4300);
        }

        if(enemyScript.currentEnemy == "God Jupiter")
        {
            enemyGO = Instantiate(enemies[62], enemySpotTrans); 
            earnXPAmount = Random.Range(4400,4600);
        }

        if(enemyScript.currentEnemy == "Goddess Aphrodite")
        {
            enemyGO = Instantiate(enemies[63], enemySpotTrans); 
            earnXPAmount = Random.Range(4600,4800);
        }

        if(enemyScript.currentEnemy == "God Poseidon")
        {
            enemyGO = Instantiate(enemies[64], enemySpotTrans); 
            earnXPAmount = Random.Range(4100,4300);
        }

        if(enemyScript.currentEnemy == "God Hades")
        {
            enemyGO = Instantiate(enemies[65], enemySpotTrans); 
            earnXPAmount = Random.Range(4800,5100);
        }

        if(enemyScript.currentEnemy == "God Dagon")
        {
            enemyGO = Instantiate(enemies[66], enemySpotTrans); 
            earnXPAmount = Random.Range(4700,5000);
        }

        if(enemyScript.currentEnemy == "Goddess Airi")
        {
            enemyGO = Instantiate(enemies[67], enemySpotTrans); 
            earnXPAmount = Random.Range(4200,4400);
        }

        if(enemyScript.currentEnemy == "Dracula")
        {
            enemyGO = Instantiate(enemies[68], enemySpotTrans); 
            earnXPAmount = Random.Range(5300,5600);
        }
        #endregion

        #region LV39-44
        if(enemyScript.currentEnemy == "Seto Kaiba")
        {
            enemyGO = Instantiate(enemies[69], enemySpotTrans); 
            earnXPAmount = Random.Range(5000,5200);
        }

        if(enemyScript.currentEnemy == "Volvagia")
        {
            enemyGO = Instantiate(enemies[70], enemySpotTrans); 
            earnXPAmount = Random.Range(5200,5400);
        }

        if(enemyScript.currentEnemy == "Gohma")
        {
            enemyGO = Instantiate(enemies[71], enemySpotTrans); 
            earnXPAmount = Random.Range(5000,5100);
        }

        if(enemyScript.currentEnemy == "Lugia")
        {
            enemyGO = Instantiate(enemies[72], enemySpotTrans); 
            earnXPAmount = Random.Range(5200,5400);
        }

        if(enemyScript.currentEnemy == "Scorpion")
        {
            enemyGO = Instantiate(enemies[73], enemySpotTrans); 
            earnXPAmount = Random.Range(5200,5400);
        }

        if(enemyScript.currentEnemy == "Bowser Jr")
        {
            enemyGO = Instantiate(enemies[74], enemySpotTrans); 
            earnXPAmount = Random.Range(5300,5500);
        }

        if(enemyScript.currentEnemy == "Zeograth")
        {
            enemyGO = Instantiate(enemies[75], enemySpotTrans); 
            earnXPAmount = Random.Range(5500,5700);
        }

        if(enemyScript.currentEnemy == "The Horde")
        {
            enemyGO = Instantiate(enemies[76], enemySpotTrans); 
            earnXPAmount = Random.Range(5500,5700);
        }

        if(enemyScript.currentEnemy == "Spirit Fighter")
        {
            enemyGO = Instantiate(enemies[77], enemySpotTrans); 
            earnXPAmount = Random.Range(5600,5800);
        }

        if(enemyScript.currentEnemy == "Urmica")
        {
            enemyGO = Instantiate(enemies[78], enemySpotTrans); 
            earnXPAmount = Random.Range(5200,5400);
        }

        if(enemyScript.currentEnemy == "Sea Leviathan")
        {
            enemyGO = Instantiate(enemies[79], enemySpotTrans); 
            earnXPAmount = Random.Range(5000,5100);
        }

        if(enemyScript.currentEnemy == "Undeen")
        {
            enemyGO = Instantiate(enemies[80], enemySpotTrans); 
            earnXPAmount = Random.Range(5200,5400);
        }
  
        if(enemyScript.currentEnemy == "Mecha Drake")
        {
            enemyGO = Instantiate(enemies[81], enemySpotTrans); 
            earnXPAmount = Random.Range(5200,5400);
        }

        if(enemyScript.currentEnemy == "Feral Kitsune")
        {
            enemyGO = Instantiate(enemies[82], enemySpotTrans); 
            earnXPAmount = Random.Range(5330,5500);
        }

        if(enemyScript.currentEnemy == "Hellhound")
        {
            enemyGO = Instantiate(enemies[83], enemySpotTrans); 
            earnXPAmount = Random.Range(5100,5300);
        }

        if(enemyScript.currentEnemy == "Bowser")
        {
            enemyGO = Instantiate(enemies[84], enemySpotTrans); 
            earnXPAmount = Random.Range(7000,7200);
        }

        if(enemyScript.currentEnemy == "Zant")
        {
            enemyGO = Instantiate(enemies[85], enemySpotTrans); 
            earnXPAmount = Random.Range(5100,5300);
        }
        #endregion

        #region LV45-50
        if(enemyScript.currentEnemy == "Ancient Priestess")
        {
            enemyGO = Instantiate(enemies[86], enemySpotTrans); 
            earnXPAmount = Random.Range(6000,6200);
        }

        if(enemyScript.currentEnemy == "Majora")
        {
            enemyGO = Instantiate(enemies[87], enemySpotTrans); 
            earnXPAmount = Random.Range(6100,6300);
        }
        if(enemyScript.currentEnemy == "Fierce Deity Link")
        {
            enemyGO = Instantiate(enemies[88], enemySpotTrans); 
            earnXPAmount = Random.Range(6200,6400);
        }

        if(enemyScript.currentEnemy == "Rayquaza")
        {
            enemyGO = Instantiate(enemies[89], enemySpotTrans); 
            earnXPAmount = Random.Range(6100,6300);
        }

        if(enemyScript.currentEnemy == "Death")
        {
            enemyGO = Instantiate(enemies[90], enemySpotTrans); 
            earnXPAmount = Random.Range(6100,6300);
        }

        if(enemyScript.currentEnemy == "The Werewolf")
        {
            enemyGO = Instantiate(enemies[91], enemySpotTrans); 
            earnXPAmount = Random.Range(6100,6300);
        }
        
        if(enemyScript.currentEnemy == "Tiamat")
        {
            enemyGO = Instantiate(enemies[92], enemySpotTrans); 
            earnXPAmount = Random.Range(6200,6400);
        }
                
        if(enemyScript.currentEnemy == "Sabre Gargoyle")
        {
            enemyGO = Instantiate(enemies[93], enemySpotTrans); 
            earnXPAmount = Random.Range(6200,6400);
        }

        if(enemyScript.currentEnemy == "Son of Valhalla")
        {
            enemyGO = Instantiate(enemies[94], enemySpotTrans); 
            earnXPAmount = Random.Range(6200,6400);
        }
        
        if(enemyScript.currentEnemy == "Xoer")
        {
            enemyGO = Instantiate(enemies[95], enemySpotTrans); 
            earnXPAmount = Random.Range(6400,6600);
        }
                
        if(enemyScript.currentEnemy == "Sorceress Duessa")
        {
            enemyGO = Instantiate(enemies[96], enemySpotTrans); 
            earnXPAmount = Random.Range(6400,6600);
        }

        if(enemyScript.currentEnemy == "Abomination")
        {
            enemyGO = Instantiate(enemies[97], enemySpotTrans); 
            earnXPAmount = Random.Range(6400,6600);
        }

        if(enemyScript.currentEnemy == "Rukkha")
        {
            enemyGO = Instantiate(enemies[98], enemySpotTrans); 
            earnXPAmount = Random.Range(6400,6600);
        }
 
        if(enemyScript.currentEnemy == "Lich King")
        {
            enemyGO = Instantiate(enemies[99], enemySpotTrans); 
            earnXPAmount = Random.Range(6400,6600);
        }
        
        if(enemyScript.currentEnemy == "Deoxys")
        {
            enemyGO = Instantiate(enemies[100], enemySpotTrans); 
            earnXPAmount = Random.Range(6700,6900);
        }
                
        if(enemyScript.currentEnemy == "Phantom Ganon")
        {
            enemyGO = Instantiate(enemies[101], enemySpotTrans); 
            earnXPAmount = Random.Range(6700,6900);
        }

        if(enemyScript.currentEnemy == "Sans")
        {
            enemyGO = Instantiate(enemies[102], enemySpotTrans); 
            earnXPAmount = Random.Range(6700,6900);
        }
        
        if(enemyScript.currentEnemy == "Toriel")
        {
            enemyGO = Instantiate(enemies[103], enemySpotTrans); 
            earnXPAmount = Random.Range(6700,6900);
        }

        if(enemyScript.currentEnemy == "Goro")
        {
            enemyGO = Instantiate(enemies[104], enemySpotTrans); 
            earnXPAmount = Random.Range(7000,7200);
        }

        if(enemyScript.currentEnemy == "Frisk")
        {
            enemyGO = Instantiate(enemies[105], enemySpotTrans); 
            earnXPAmount = Random.Range(7000,7200);
        }

        if(enemyScript.currentEnemy == "Nuckaleeve")
        {
            enemyGO = Instantiate(enemies[106], enemySpotTrans); 
            earnXPAmount = Random.Range(7000,7200);
        }

        if(enemyScript.currentEnemy == "Ignis")
        {
            enemyGO = Instantiate(enemies[107], enemySpotTrans); 
            earnXPAmount = Random.Range(7000,7200);
        }

        if(enemyScript.currentEnemy == "Ganondorf")
        {
            enemyGO = Instantiate(enemies[108], enemySpotTrans); 
            
            if(playerUnit.lvl < 50)
                earnXPAmount = Random.Range(8000,8500);
        }

        if(enemyScript.currentEnemy == "Mewtwo")
        {
            enemyGO = Instantiate(enemies[109], enemySpotTrans); 
            
            if(playerUnit.lvl < 50)
                earnXPAmount = Random.Range(8000,8500);
        }
    
        enemyUnit = enemyGO.GetComponent<Unit>();

        #endregion

    }

    void EarnedNewplayer()
    {   
        if(enemyUnit.unitName == "Egyptian Sphinx")
        {
            Unit sphinxUnit = fighters[2].GetComponent<Unit>();
            if(!sphinxUnit.unlocked)
            {
                sphinxUnit.unlocked = true; 
                SaveGame.Save<bool>("[2]unlocked", true);
                earnedFighter = true;
                SaveGame.Save<string>("newunlock", "Sphinx"); 
            }  //saving the new unlock 
        }

        if(enemyUnit.unitName == "Mario")
        {
            Unit marioUnit = fighters[3].GetComponent<Unit>();
            if(!marioUnit.unlocked)
            {
                marioUnit.unlocked = true; 
                SaveGame.Save<bool>("[3]unlocked", true);
                earnedFighter = true;
                SaveGame.Save<string>("newunlock", "Mario"); 
            }
        }

        if(enemyUnit.unitName == "Charizard")
        {
            Unit charizardUnit = fighters[4].GetComponent<Unit>();
            if(!charizardUnit.unlocked)
            {
                charizardUnit.unlocked = true; 
                SaveGame.Save<bool>("[4]unlocked", true);
                earnedFighter = true;
                SaveGame.Save<string>("newunlock", "Charizard"); 
            }
        }

        if(enemyUnit.unitName == "Luigi")
        {
            Unit luigiUnit = fighters[5].GetComponent<Unit>();
            if(!luigiUnit.unlocked)
            {
                luigiUnit.unlocked = true; 
                SaveGame.Save<bool>("[5]unlocked", true);
                earnedFighter = true;
                SaveGame.Save<string>("newunlock", "Luigi"); 
            }
        }

        if(enemyUnit.unitName == "Link")
        {
            Unit linkUnit = fighters[6].GetComponent<Unit>();
            if(!linkUnit.unlocked)
            {
                linkUnit.unlocked = true; 
                SaveGame.Save<bool>("[6]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Link"); 
            }
        }

        if(enemyUnit.unitName == "Mew")
        {
            Unit mewUnit = fighters[7].GetComponent<Unit>();
            if(!mewUnit.unlocked)
            {
                mewUnit.unlocked = true; 
                SaveGame.Save<bool>("[7]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Mew"); 
            }
        }

        if(enemyUnit.unitName == "Blue Eyes White Dragon")
        {
            Unit blueeyesUnit = fighters[8].GetComponent<Unit>();
            if(!blueeyesUnit.unlocked)
            {
                blueeyesUnit.unlocked = true; 
                SaveGame.Save<bool>("[8]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Blue Eyes White Dragon"); 
            }
        }

        if(enemyUnit.unitName == "Dracula")
        {
            Unit draculaUnit = fighters[9].GetComponent<Unit>();
            if(!draculaUnit.unlocked)
            {
                draculaUnit.unlocked = true; 
                SaveGame.Save<bool>("[9]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Dracula"); 
            }
        }

        if(enemyUnit.unitName == "Bowser")
        {
            Unit bowserUnit = fighters[10].GetComponent<Unit>();
            if(!bowserUnit.unlocked)
            {
                bowserUnit.unlocked = true; 
                SaveGame.Save<bool>("[10]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Bowser"); 
            }
        }

        if(enemyUnit.unitName == "Ganondorf")
        {
            Unit bowserUnit = fighters[11].GetComponent<Unit>();
            if(!bowserUnit.unlocked)
            {
                bowserUnit.unlocked = true; 
                SaveGame.Save<bool>("[11]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Ganondorf"); 
            }
        }

        if(enemyUnit.unitName == "Mewtwo")
        {
            Unit bowserUnit = fighters[12].GetComponent<Unit>();
            if(!bowserUnit.unlocked)
            {
                bowserUnit.unlocked = true; 
                SaveGame.Save<bool>("[12]unlocked", true);
                earnedFighter = true; 
                SaveGame.Save<string>("newunlock", "Mewtwo"); 
            }
        }

    }

    void NewShopFighter()
    {
        if(enemyUnit.unitName == "Ganondorf")
        {
            Unit zeldaUnit = fighters[13].GetComponent<Unit>(); 
            if(!zeldaUnit.unlocked && !SaveGame.Exists("[13]shop") || SaveGame.Load<bool>("[13]shop") == false)
            {
                zeldaUnit.unlocked = true; 
                SaveGame.Save<bool>("[13]shop", true);
                fighterInShop = true;  
            }
        }

        if(enemyUnit.unitName == "Blood Slime")
        {
            Unit bSlimeUnit = fighters[14].GetComponent<Unit>(); 
            if(!bSlimeUnit.unlocked && !SaveGame.Exists("[14]shop") || SaveGame.Load<bool>("[14]shop") == false)
            {
                bSlimeUnit.unlocked = true; 
                SaveGame.Save<bool>("[14]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Blood Slime"); 
            }
        }

        if(enemyUnit.unitName == "Elven Naturess")
        {
            Unit elvNatureUnit = fighters[15].GetComponent<Unit>(); 
            if(!elvNatureUnit.unlocked && !SaveGame.Exists("[15]shop") || SaveGame.Load<bool>("[15]shop") == false)
            {
                elvNatureUnit.unlocked = true; 
                SaveGame.Save<bool>("[15]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Elven Naturess"); 
            }
        }
      
        if(enemyUnit.unitName == "Earth Gem Golem")
        {
            Unit elvNatureUnit = fighters[16].GetComponent<Unit>(); 
            if(!elvNatureUnit.unlocked && !SaveGame.Exists("[16]shop") || SaveGame.Load<bool>("[16]shop") == false)
            {
                elvNatureUnit.unlocked = true; 
                SaveGame.Save<bool>("[16]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Earth Gem Golem"); 
            }
        }
                
        if(enemyUnit.unitName == "Colossal Rex")
        {
            Unit elvNatureUnit = fighters[17].GetComponent<Unit>(); 
            if(!elvNatureUnit.unlocked && !SaveGame.Exists("[17]shop") || SaveGame.Load<bool>("[17]shop") == false)
            {
                elvNatureUnit.unlocked = true; 
                SaveGame.Save<bool>("[17]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Colossal Rex"); 
            }
        }
                        
        if(enemyUnit.unitName == "Undeen")
        {
            Unit elvNatureUnit = fighters[18].GetComponent<Unit>(); 
            if(!elvNatureUnit.unlocked && !SaveGame.Exists("[18]shop") || SaveGame.Load<bool>("[18]shop") == false)
            {
                elvNatureUnit.unlocked = true; 
                SaveGame.Save<bool>("[18]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Undeen");
            }
        }
                                
        if(enemyUnit.unitName == "Ignis")
        {
            Unit elvNatureUnit = fighters[19].GetComponent<Unit>(); 
            if(!elvNatureUnit.unlocked && !SaveGame.Exists("[19]shop") || SaveGame.Load<bool>("[19]shop") == false)
            {
                elvNatureUnit.unlocked = true; 
                SaveGame.Save<bool>("[19]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Ignis"); 
            }
        }
                                        
        if(enemyUnit.unitName == "Scorpion")
        {
            Unit elvNatureUnit = fighters[20].GetComponent<Unit>(); 
            if(!elvNatureUnit.unlocked && !SaveGame.Exists("[20]shop") || SaveGame.Load<bool>("[20]shop") == false)
            {
                elvNatureUnit.unlocked = true; 
                SaveGame.Save<bool>("[20]shop", true);
                fighterInShop = true;  
                SaveGame.Save<string>("newunlock", "Scorpion"); 
            }
        }
    }
    
    public void GameSpeed()
    {
        Time.timeScale = gameSpeed.value;
        SaveGame.Save<float>("gamespeedvalue", gameSpeed.value);
    }

    public void SaveFighter()
    {
        if(playerScript.currentFighter == "Egyptian Warrior")
        { 
            SaveGame.Save<int>("[0]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[0]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[0]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[0]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[0]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[0]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[0]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[0]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[0]unlocked", currFighterUnit.unlocked);
        }
        if(playerScript.currentFighter == "Egyptian Mage")
        {
            SaveGame.Save<int>("[1]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[1]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[1]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[1]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[1]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[1]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[1]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[1]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[1]unlocked", currFighterUnit.unlocked);
        }
        if(playerScript.currentFighter == "Egyptian Sphinx")
        {
            SaveGame.Save<int>("[2]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[2]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[2]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[2]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[2]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[2]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[2]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[2]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[2]unlocked", currFighterUnit.unlocked);
        }

        if(playerScript.currentFighter == "Mario")
        {
            SaveGame.Save<int>("[3]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[3]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[3]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[3]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[3]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[3]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[3]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[3]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[3]unlocked", currFighterUnit.unlocked);
        }

        if(playerScript.currentFighter == "Charizard")
        {
            SaveGame.Save<int>("[4]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[4]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[4]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[4]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[4]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[4]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[4]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[4]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[4]unlocked", currFighterUnit.unlocked);           
        }

        if(playerScript.currentFighter == "Luigi")
        {
            SaveGame.Save<int>("[5]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[5]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[5]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[5]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[5]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[5]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[5]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[5]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[5]unlocked", currFighterUnit.unlocked);           
        }

        if(playerScript.currentFighter == "Link")
        {
            SaveGame.Save<int>("[6]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[6]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[6]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[6]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[6]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[6]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[6]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[6]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[6]unlocked", currFighterUnit.unlocked);           
        }
        
        if(playerScript.currentFighter == "Mew")
        {
            SaveGame.Save<int>("[7]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[7]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[7]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[7]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[7]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[7]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[7]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[7]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[7]unlocked", currFighterUnit.unlocked);           
        }

        if(playerScript.currentFighter == "Blue Eyes White Dragon")
        {
            SaveGame.Save<int>("[8]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[8]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[8]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[8]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[8]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[8]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[8]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[8]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[8]unlocked", currFighterUnit.unlocked);           
        }
        
        if(playerScript.currentFighter == "Dracula")
        {
            SaveGame.Save<int>("[9]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[9]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[9]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[9]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[9]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[9]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[9]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[9]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[9]unlocked", currFighterUnit.unlocked);           
        }

        if(playerScript.currentFighter == "Bowser")
        {
            SaveGame.Save<int>("[10]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[10]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[10]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[10]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[10]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[10]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[10]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[10]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[10]unlocked", currFighterUnit.unlocked);           
        }

        if(playerScript.currentFighter == "Ganondorf")
        {
            SaveGame.Save<int>("[11]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[11]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[11]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[11]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[11]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[11]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[11]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[11]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[11]unlocked", currFighterUnit.unlocked);           
        }
        
        if(playerScript.currentFighter == "Mewtwo")
        {
            SaveGame.Save<int>("[12]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[12]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[12]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[12]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[12]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[12]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[12]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[12]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[12]unlocked", currFighterUnit.unlocked);           
        }
                
        if(playerScript.currentFighter == "Princess Zelda")
        {
            SaveGame.Save<int>("[13]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[13]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[13]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[13]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[13]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[13]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[13]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[13]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[13]unlocked", currFighterUnit.unlocked);           
        }
                        
        if(playerScript.currentFighter == "Blood Slime")
        {
            SaveGame.Save<int>("[14]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[14]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[14]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[14]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[14]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[14]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[14]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[14]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[14]unlocked", currFighterUnit.unlocked);           
        }

                                
        if(playerScript.currentFighter == "Elven Naturess")
        {
            SaveGame.Save<int>("[15]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[15]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[15]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[15]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[15]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[15]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[15]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[15]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[15]unlocked", currFighterUnit.unlocked);           
        }
                                        
        if(playerScript.currentFighter == "Earth Gem God")
        {
            SaveGame.Save<int>("[16]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[16]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[16]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[16]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[16]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[16]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[16]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[16]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[16]unlocked", currFighterUnit.unlocked);           
        }
                                                
        if(playerScript.currentFighter == "Colossal Rex")
        {
            SaveGame.Save<int>("[17]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[17]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[17]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[17]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[17]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[17]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[17]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[17]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[17]unlocked", currFighterUnit.unlocked);           
        }
                                                        
        if(playerScript.currentFighter == "Undeen")
        {
            SaveGame.Save<int>("[18]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[18]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[18]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[18]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[18]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[18]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[18]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[18]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[18]unlocked", currFighterUnit.unlocked);           
        }
                                                                
        if(playerScript.currentFighter == "Ignis")
        {
            SaveGame.Save<int>("[19]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[19]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[19]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[19]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[19]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[19]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[19]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[19]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[19]unlocked", currFighterUnit.unlocked);           
        }
                                                                        
        if(playerScript.currentFighter == "Scorpion")
        {
            SaveGame.Save<int>("[20]maxhp", currFighterUnit.maxHP);
            SaveGame.Save<int>("[20]curhp", currFighterUnit.currentHP);
            SaveGame.Save<int>("[20]atk", currFighterUnit.attack);
            SaveGame.Save<int>("[20]spd", currFighterUnit.speed);
            SaveGame.Save<int>("[20]heal", currFighterUnit.heal);
            SaveGame.Save<int>("[20]lvl", currFighterUnit.lvl);
            SaveGame.Save<int>("[20]xp", currFighterUnit.xp);
            SaveGame.Save<int>("[20]maxxp", currFighterUnit.maxXP);
            SaveGame.Save<bool>("[20]unlocked", currFighterUnit.unlocked);           
        }

        SaveGame.Save<float>("gamespeedvalue", gameSpeed.value);
    }

    public void LoadFighter()
    {

        if(SaveGame.Exists("gamespeedvalue"))
        {
            Time.timeScale = SaveGame.Load<float>("gamespeedvalue"); 
        }
        else
        {
            SaveGame.Save<float>("gamespeedvalue", 1);  
        }

        if(SaveGame.Exists("gamespeed"))
        {
            gameSpeed.maxValue = SaveGame.Load<float>("gamespeed");
        }
        else
        {
            SaveGame.Save<float>("gamespeed", 1); 
        }

        if(playerScript.currentFighter == "Egyptian Warrior")
        { 
            if(SaveGame.Exists("[0]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[0]maxhp");

            if(SaveGame.Exists("[0]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[0]curhp");

            if(SaveGame.Exists("[0]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[0]atk");
            
            if(SaveGame.Exists("[0]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[0]spd"); 

            if(SaveGame.Exists("[0]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[0]heal"); 

            if(SaveGame.Exists("[0]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[0]lvl");

            if(SaveGame.Exists("[0]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[0]xp");

            if(SaveGame.Exists("[0]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[0]maxxp");

            if(SaveGame.Exists("[0]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[0]unlocked");
        }

        if(playerScript.currentFighter == "Egyptian Mage") 
        {
            if(SaveGame.Exists("[1]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[1]maxhp");

            if(SaveGame.Exists("[1]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[1]curhp");

            if(SaveGame.Exists("[1]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[1]atk");
            
            if(SaveGame.Exists("[1]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[1]spd"); 

            if(SaveGame.Exists("[1]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[1]heal"); 

            if(SaveGame.Exists("[1]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[1]lvl");

            if(SaveGame.Exists("[1]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[1]xp");

            if(SaveGame.Exists("[1]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[1]maxxp");

            if(SaveGame.Exists("[1]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[1]unlocked");
        }
        
        if(playerScript.currentFighter == "Egyptian Sphinx")
        {
            if(SaveGame.Exists("[2]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[2]maxhp");

            if(SaveGame.Exists("[2]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[2]curhp");

            if(SaveGame.Exists("[2]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[2]atk");
            
            if(SaveGame.Exists("[2]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[2]spd"); 

            if(SaveGame.Exists("[2]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[2]heal"); 

            if(SaveGame.Exists("[2]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[2]lvl");

            if(SaveGame.Exists("[2]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[2]xp");

            if(SaveGame.Exists("[2]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[2]maxxp");

            if(SaveGame.Exists("[2]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[2]unlocked");
        }          
        
        if(playerScript.currentFighter == "Mario")
        {
            if(SaveGame.Exists("[3]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[3]maxhp");

            if(SaveGame.Exists("[3]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[3]curhp");

            if(SaveGame.Exists("[3]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[3]atk");
            
            if(SaveGame.Exists("[3]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[3]spd"); 

            if(SaveGame.Exists("[3]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[3]heal"); 

            if(SaveGame.Exists("[3]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[3]lvl");

            if(SaveGame.Exists("[3]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[3]xp");

            if(SaveGame.Exists("[3]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[3]maxxp");

            if(SaveGame.Exists("[3]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[3]unlocked");
        }

        if(playerScript.currentFighter == "Charizard")
        {
            if(SaveGame.Exists("[4]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[4]maxhp");

            if(SaveGame.Exists("[4]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[4]curhp");

            if(SaveGame.Exists("[4]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[4]atk");
            
            if(SaveGame.Exists("[4]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[4]spd"); 

            if(SaveGame.Exists("[4]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[4]heal"); 

            if(SaveGame.Exists("[4]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[4]lvl");

            if(SaveGame.Exists("[4]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[4]xp");

            if(SaveGame.Exists("[4]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[4]maxxp");

            if(SaveGame.Exists("[4]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[4]unlocked");
        }

        if(playerScript.currentFighter == "Luigi")
        {
            if(SaveGame.Exists("[5]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[5]maxhp");

            if(SaveGame.Exists("[5]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[5]curhp");

            if(SaveGame.Exists("[5]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[5]atk");
            
            if(SaveGame.Exists("[5]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[5]spd"); 

            if(SaveGame.Exists("[5]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[5]heal"); 

            if(SaveGame.Exists("[5]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[5]lvl");

            if(SaveGame.Exists("[5]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[5]xp");

            if(SaveGame.Exists("[5]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[5]maxxp");

            if(SaveGame.Exists("[5]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[5]unlocked");
        }

        if(playerScript.currentFighter == "Link")
        {
            if(SaveGame.Exists("[6]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[6]maxhp");

            if(SaveGame.Exists("[6]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[6]curhp");

            if(SaveGame.Exists("[6]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[6]atk");
            
            if(SaveGame.Exists("[6]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[6]spd"); 

            if(SaveGame.Exists("[6]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[6]heal"); 

            if(SaveGame.Exists("[6]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[6]lvl");

            if(SaveGame.Exists("[6]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[6]xp");

            if(SaveGame.Exists("[6]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[6]maxxp");

            if(SaveGame.Exists("[6]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[6]unlocked");
        }

        if(playerScript.currentFighter == "Mew")
        {
            if(SaveGame.Exists("[7]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[7]maxhp");

            if(SaveGame.Exists("[7]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[7]curhp");

            if(SaveGame.Exists("[7]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[7]atk");
            
            if(SaveGame.Exists("[7]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[7]spd"); 

            if(SaveGame.Exists("[7]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[7]heal"); 

            if(SaveGame.Exists("[7]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[7]lvl");

            if(SaveGame.Exists("[7]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[7]xp");

            if(SaveGame.Exists("[7]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[7]maxxp");

            if(SaveGame.Exists("[7]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[7]unlocked");
        }

        if(playerScript.currentFighter == "Blue Eyes White Dragon")
        {
            if(SaveGame.Exists("[8]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[8]maxhp");

            if(SaveGame.Exists("[8]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[8]curhp");

            if(SaveGame.Exists("[8]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[8]atk");
            
            if(SaveGame.Exists("[8]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[8]spd"); 

            if(SaveGame.Exists("[8]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[8]heal"); 

            if(SaveGame.Exists("[8]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[8]lvl");

            if(SaveGame.Exists("[8]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[8]xp");

            if(SaveGame.Exists("[8]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[8]maxxp");

            if(SaveGame.Exists("[8]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[8]unlocked");
        }

        
        if(playerScript.currentFighter == "Dracula")
        {
            if(SaveGame.Exists("[9]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[9]maxhp");

            if(SaveGame.Exists("[9]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[9]curhp");

            if(SaveGame.Exists("[9]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[9]atk");
            
            if(SaveGame.Exists("[9]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[9]spd"); 

            if(SaveGame.Exists("[9]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[9]heal"); 

            if(SaveGame.Exists("[9]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[9]lvl");

            if(SaveGame.Exists("[9]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[9]xp");

            if(SaveGame.Exists("[9]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[9]maxxp");

            if(SaveGame.Exists("[9]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[9]unlocked");
        }

        if(playerScript.currentFighter == "Bowser")
        {
            if(SaveGame.Exists("[10]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[10]maxhp");

            if(SaveGame.Exists("[10]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[10]curhp");

            if(SaveGame.Exists("[10]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[10]atk");
            
            if(SaveGame.Exists("[10]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[10]spd"); 

            if(SaveGame.Exists("[10]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[10]heal"); 

            if(SaveGame.Exists("[10]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[10]lvl");

            if(SaveGame.Exists("[10]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[10]xp");

            if(SaveGame.Exists("[10]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[10]maxxp");

            if(SaveGame.Exists("[10]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[10]unlocked");
        }

        
        if(playerScript.currentFighter == "Ganondorf")
        {
            if(SaveGame.Exists("[11]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[11]maxhp");

            if(SaveGame.Exists("[11]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[11]curhp");

            if(SaveGame.Exists("[11]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[11]atk");
            
            if(SaveGame.Exists("[11]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[11]spd"); 

            if(SaveGame.Exists("[11]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[11]heal"); 

            if(SaveGame.Exists("[11]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[11]lvl");

            if(SaveGame.Exists("[11]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[11]xp");

            if(SaveGame.Exists("[11]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[11]maxxp");

            if(SaveGame.Exists("[11]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[11]unlocked");
        }

        if(playerScript.currentFighter == "Mewtwo")
        {
            if(SaveGame.Exists("[12]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[12]maxhp");

            if(SaveGame.Exists("[12]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[12]curhp");

            if(SaveGame.Exists("[12]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[12]atk");
            
            if(SaveGame.Exists("[12]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[12]spd"); 

            if(SaveGame.Exists("[12]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[12]heal"); 

            if(SaveGame.Exists("[12]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[12]lvl");

            if(SaveGame.Exists("[12]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[12]xp");

            if(SaveGame.Exists("[12]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[12]maxxp");

            if(SaveGame.Exists("[12]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[12]unlocked");
        }

        if(playerScript.currentFighter == "Princess Zelda")
        {
            if(SaveGame.Exists("[13]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[13]maxhp");

            if(SaveGame.Exists("[13]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[13]curhp");

            if(SaveGame.Exists("[13]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[13]atk");
            
            if(SaveGame.Exists("[13]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[13]spd"); 

            if(SaveGame.Exists("[13]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[13]heal"); 

            if(SaveGame.Exists("[13]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[13]lvl");

            if(SaveGame.Exists("[13]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[13]xp");

            if(SaveGame.Exists("[13]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[13]maxxp");

            if(SaveGame.Exists("[13]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[13]unlocked");
        }
        
        if(playerScript.currentFighter == "Blood Slime")
        {
            if(SaveGame.Exists("[14]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[14]maxhp");

            if(SaveGame.Exists("[14]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[14]curhp");

            if(SaveGame.Exists("[14]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[14]atk");
            
            if(SaveGame.Exists("[14]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[14]spd"); 

            if(SaveGame.Exists("[14]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[14]heal"); 

            if(SaveGame.Exists("[14]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[14]lvl");

            if(SaveGame.Exists("[14]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[14]xp");

            if(SaveGame.Exists("[14]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[14]maxxp");

            if(SaveGame.Exists("[14]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[14]unlocked");
        }
               
        if(playerScript.currentFighter == "Blood Slime")
        {
            if(SaveGame.Exists("[14]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[14]maxhp");

            if(SaveGame.Exists("[14]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[14]curhp");

            if(SaveGame.Exists("[14]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[14]atk");
            
            if(SaveGame.Exists("[14]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[14]spd"); 

            if(SaveGame.Exists("[14]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[14]heal"); 

            if(SaveGame.Exists("[14]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[14]lvl");

            if(SaveGame.Exists("[14]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[14]xp");

            if(SaveGame.Exists("[14]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[14]maxxp");

            if(SaveGame.Exists("[14]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[14]unlocked");
        }
               
        if(playerScript.currentFighter == "Elven Naturess")
        {
            if(SaveGame.Exists("[15]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[15]maxhp");

            if(SaveGame.Exists("[15]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[15]curhp");

            if(SaveGame.Exists("[15]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[15]atk");
            
            if(SaveGame.Exists("[15]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[15]spd"); 

            if(SaveGame.Exists("[15]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[15]heal"); 

            if(SaveGame.Exists("[15]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[15]lvl");

            if(SaveGame.Exists("[15]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[15]xp");

            if(SaveGame.Exists("[15]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[15]maxxp");

            if(SaveGame.Exists("[15]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[15]unlocked");
        }
                       
        if(playerScript.currentFighter == "Earth Gem Golem")
        {
            if(SaveGame.Exists("[16]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[16]maxhp");

            if(SaveGame.Exists("[16]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[16]curhp");

            if(SaveGame.Exists("[16]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[16]atk");
            
            if(SaveGame.Exists("[16]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[16]spd"); 

            if(SaveGame.Exists("[16]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[16]heal"); 

            if(SaveGame.Exists("[16]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[16]lvl");

            if(SaveGame.Exists("[16]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[16]xp");

            if(SaveGame.Exists("[16]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[16]maxxp");

            if(SaveGame.Exists("[16]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[16]unlocked");
        }

        if(playerScript.currentFighter == "Colossal Rex")
        {
            if(SaveGame.Exists("[17]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[17]maxhp");

            if(SaveGame.Exists("[17]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[17]curhp");

            if(SaveGame.Exists("[17]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[17]atk");
            
            if(SaveGame.Exists("[17]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[17]spd"); 

            if(SaveGame.Exists("[17]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[17]heal"); 

            if(SaveGame.Exists("[17]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[17]lvl");

            if(SaveGame.Exists("[17]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[17]xp");

            if(SaveGame.Exists("[17]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[17]maxxp");

            if(SaveGame.Exists("[17]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[17]unlocked");
        }
        
        if(playerScript.currentFighter == "Undeen")
        {
            if(SaveGame.Exists("[18]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[18]maxhp");

            if(SaveGame.Exists("[18]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[18]curhp");

            if(SaveGame.Exists("[18]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[18]atk");
            
            if(SaveGame.Exists("[18]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[18]spd"); 

            if(SaveGame.Exists("[18]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[18]heal"); 

            if(SaveGame.Exists("[18]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[18]lvl");

            if(SaveGame.Exists("[18]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[18]xp");

            if(SaveGame.Exists("[18]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[18]maxxp");

            if(SaveGame.Exists("[18]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[18]unlocked");
        }
                
        if(playerScript.currentFighter == "Ignis")
        {
            if(SaveGame.Exists("[19]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[19]maxhp");

            if(SaveGame.Exists("[19]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[19]curhp");

            if(SaveGame.Exists("[19]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[19]atk");
            
            if(SaveGame.Exists("[19]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[19]spd"); 

            if(SaveGame.Exists("[19]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[19]heal"); 

            if(SaveGame.Exists("[19]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[19]lvl");

            if(SaveGame.Exists("[19]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[19]xp");

            if(SaveGame.Exists("[19]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[19]maxxp");

            if(SaveGame.Exists("[19]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[19]unlocked");
        }
                        
        if(playerScript.currentFighter == "Scorpion")
        {
            if(SaveGame.Exists("[20]maxhp"))
                currFighterUnit.maxHP = SaveGame.Load<int>("[20]maxhp");

            if(SaveGame.Exists("[20]curhp"))
                currFighterUnit.currentHP = SaveGame.Load<int>("[20]curhp");

            if(SaveGame.Exists("[20]atk"))
                currFighterUnit.attack = SaveGame.Load<int>("[20]atk");
            
            if(SaveGame.Exists("[20]spd"))
                currFighterUnit.speed = SaveGame.Load<int>("[20]spd"); 

            if(SaveGame.Exists("[20]heal"))
                currFighterUnit.heal = SaveGame.Load<int>("[20]heal"); 

            if(SaveGame.Exists("[20]lvl"))
                currFighterUnit.lvl = SaveGame.Load<int>("[20]lvl");

            if(SaveGame.Exists("[20]xp"))
                currFighterUnit.xp = SaveGame.Load<int>("[20]xp");

            if(SaveGame.Exists("[20]maxxp"))
                currFighterUnit.maxXP = SaveGame.Load<int>("[20]maxxp");

            if(SaveGame.Exists("[20]unlocked"))
                currFighterUnit.unlocked = SaveGame.Load<bool>("[20]unlocked");
        }

    }
}
