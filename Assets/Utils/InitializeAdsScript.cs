using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAdsScript : MonoBehaviour { 

    string gameIdIOS = "3680948"; // iOS
    string gameIdAndroid = "3680949"; // Android
    bool testMode = false;

    string placementId = "PlayScreen";

    void Start () {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            Advertisement.Initialize (gameIdIOS, testMode);
            StartCoroutine (ShowBannerWhenReady ());
        }
        if (Application.platform == RuntimePlatform.Android) {
            Advertisement.Initialize (gameIdAndroid, testMode); 
            StartCoroutine (ShowBannerWhenReady ());
        }
    }

    IEnumerator ShowBannerWhenReady () {
        while (!Advertisement.IsReady (placementId)) {
            yield return new WaitForSeconds (0.5f);
        }
        Advertisement.Banner.SetPosition (BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show (placementId);
    }
}