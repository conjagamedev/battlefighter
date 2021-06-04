using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using BayatGames.SaveGameFree;

public class Settings : MonoBehaviour
{
    Player player; 
    public Sprite[] soundSprites; 
    public Image soundImg; 
    public Slider soundSlider; 
    bool isMuted; 

    public Text msgboxText; 
    public Animation msgboxAnim;

    public GameObject themeMusic; 
    public GameObject returnBtn; 

    public Text msgBoxOKTxt; 
    public Animation msgBoxOKAnim; 

    public Text creditTxt; 
    public Animation creditAnim; 


    void Start() 
    {
        if(GameObject.Find("HomeSystem") != null)
            player = GameObject.Find("HomeSystem").GetComponent<Player>();
        else
            return; 
        
        if(SaveGame.Exists("ismuted"))
            isMuted = SaveGame.Load<bool>("ismuted");


        if(SaveGame.Exists("volume"))
        {
            soundSlider.value = SaveGame.Load<float>("volume"); 
            
            if(GameObject.FindGameObjectWithTag("themeMusic") != null)
            {
                AudioSource music = GameObject.FindGameObjectWithTag("themeMusic").GetComponent<AudioSource>();
                music.volume = soundSlider.value; 
            }
            
        }
        else
        {
            isMuted = false;
            soundSlider.value = 0.5f; 
        } 

        if(isMuted)
        {
            soundImg.sprite = soundSprites[0];
        }
        else
        {
            soundImg.sprite = soundSprites[1];
        }
    }

    public void SlideSound()
    {   
        if(soundSlider.value > soundSlider.minValue)
        {
            isMuted = false; 
            SaveGame.Save<bool>("ismuted", false);
        } 
        else
        {
            isMuted = true; 
            SaveGame.Save<bool>("ismuted", true);
        }

        if(GameObject.FindGameObjectWithTag("themeMusic") != null)
        {
            AudioSource music = GameObject.FindGameObjectWithTag("themeMusic").GetComponent<AudioSource>();
            music.volume = soundSlider.value;
            SaveGame.Save<float>("volume", soundSlider.value);
        }

    }

    void Update()
    {
        if(soundSlider.value > 0)
        {
            if(GameObject.FindGameObjectWithTag("themeMusic") == null)
            {
                Instantiate(themeMusic); 
            }
            isMuted = false;
            soundImg.sprite = soundSprites[1];
        }

        if(soundSlider.value == soundSlider.minValue)
        {
            isMuted = true; 
            soundImg.sprite = soundSprites[0];
            Destroy(GameObject.FindGameObjectWithTag("themeMusic"));
        }
    }

    public void ClearGameData()
    {
        if(GameObject.Find("returnBtn") != null)
        {
            returnBtn.active = false; 
            msgboxAnim.Play("msgBoxAnim"); 
            msgboxText.text = "Are you sure you want to delete ALL of your game data?";
        }
    }

    public void MsgBoxYes()
    {
        SaveGame.Clear(); 
        player.currentFighter = "Egyptian Mage"; 
        msgboxAnim.Play("msgBoxAwayAnim"); 
        returnBtn.active = true; 
    }

    public void MsgBoxNo()
    {
        msgboxAnim.Play("msgBoxAwayAnim");
        returnBtn.active = true; 
    }

    public void ToggleSound()
    {
        if(isMuted == false)
        {
            soundSlider.value = soundSlider.minValue; 
            isMuted = true;
            SaveGame.Save<bool>("ismuted", true);
            SaveGame.Save<float>("volume", soundSlider.value);
            Destroy(GameObject.FindGameObjectWithTag("themeMusic")); 
            soundImg.sprite = soundSprites[0]; 
        }
        else if(isMuted == true)
        {
            soundSlider.value = soundSlider.maxValue; 
            SaveGame.Save<bool>("ismuted", false);
            SaveGame.Save<float>("volume", soundSlider.value);
            isMuted = false;
            Instantiate(themeMusic); 
            DontDestroyOnLoad(GameObject.FindGameObjectWithTag("themeMusic")); 
            soundImg.sprite = soundSprites[1];
        }
    }

    public void FullTutorialMsgBox()
    {
        if(GameObject.Find("returnBtn") != null)
        {
            returnBtn.active = false; 
            msgBoxOKAnim.Play("tutorialBoxAnim"); 
        }
    }

    public void OKMsgBox()
    {
        msgBoxOKAnim.Play("tutorialBoxAwayAnim"); 
        returnBtn.active = true; 
    }

    public void CreditMsgBox()
    {
        creditAnim.Play("tutorialBoxAnim");
    }

    public void CloseCreditBox()
    {
        creditAnim.Play("tutorialBoxAwayAnim");
    }
}
