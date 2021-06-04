using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements; 

public class bannerAd : MonoBehaviour
{
    
    string gameId = "3922605";
    bool testMode = false; 
    bool showAd;  
    string placementId = "bannerAd";
    
    void Start () {
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(ShowBannerWhenInitialized());
    }

    IEnumerator ShowBannerWhenInitialized () {
        while (!Advertisement.isInitialized) {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition (BannerPosition.BOTTOM_LEFT);
        Advertisement.Banner.Show (placementId);
    }
}
