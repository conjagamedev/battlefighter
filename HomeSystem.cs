using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BayatGames.SaveGameFree;

public class HomeSystem : MonoBehaviour
{   
    Player player; 
    
    private void Start()
    {
        if(GameObject.Find("HomeSystem") != null)
            player = GameObject.Find("HomeSystem").GetComponent<Player>();
        else
            return; 

    }
    public void StartFighterButton()
    {
        SceneManager.LoadScene("fighterBattleScene", LoadSceneMode.Single);
        DontDestroyOnLoad(GameObject.Find("HomeSystem"));
        SaveGame.Save<string>("currentfighter", player.currentFighter);
       // PlayerPrefs.SetString("currentFighter", player.currentFighter);
    }

    public void LeaveBattle()
    {
        SceneManager.LoadScene("homeScene", LoadSceneMode.Single);
        //player.currentFighter = PlayerPrefs.GetString("currentFighter");
        Destroy(GameObject.Find("HomeSystem"));
    }

    public void ReturnHome()
    {
        SaveGame.Save<string>("currentfighter", player.currentFighter);
        Destroy(GameObject.Find("HomeSystem"));
        SceneManager.LoadScene("homeScene", LoadSceneMode.Single);
        player.currentFighter = SaveGame.Load<string>("currentfighter");
    }

      public void StartCollectionsButton()
    {
        SceneManager.LoadScene("collectionScene", LoadSceneMode.Single);
        DontDestroyOnLoad(GameObject.Find("HomeSystem"));
        SaveGame.Save<string>("currentfighter", player.currentFighter);
    }

    public void StartClicker()
    {
        SceneManager.LoadScene("clickerScene", LoadSceneMode.Single);
        DontDestroyOnLoad(GameObject.Find("HomeSystem"));
        SaveGame.Save<string>("currentfighter", player.currentFighter);

    }

    public void ReturnClicker()
    {
        SaveGame.Save<string>("currentfighter", player.currentFighter); 
        Destroy(GameObject.Find("HomeSystem")); 
        SaveGame.Save<string>("offlinetime", "");
        SceneManager.LoadScene("homeScene", LoadSceneMode.Single); 
        player.currentFighter = SaveGame.Load<string>("currentfighter"); 
    }
    public void StartShop()
    {
        SceneManager.LoadScene("shopScene", LoadSceneMode.Single); 
        DontDestroyOnLoad(GameObject.Find("HomeSystem")); 
        SaveGame.Save<string>("currentfighter", player.currentFighter); 
    }

    public void StartSettings()
    {
        SceneManager.LoadScene("settingsScene", LoadSceneMode.Single); 
        DontDestroyOnLoad(GameObject.Find("HomeSystem"));
        SaveGame.Save<string>("currentfighter", player.currentFighter); 
    }

}
