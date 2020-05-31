using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAdsScript : MonoBehaviour { 

    string gameIdIOS = "3630062"; // iOS
    string gameIdAndroid = "3630063"; // Android
    bool testMode = false;

    string placementId = "PlayScreen";

    void Start () {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            Advertisement.Initialize (gameIdIOS, testMode);
            StartCoroutine (ShowBannerWhenReady ());
        } else if (Application.platform == RuntimePlatform.Android) {
            Advertisement.Initialize (gameIdAndroid, testMode); 
            StartCoroutine (ShowBannerWhenReady ());
        }
    }

    IEnumerator ShowBannerWhenReady () {
        while (!Advertisement.IsReady (placementId)) {
            yield return new WaitForSeconds (0.5f);
        }
        Advertisement.Banner.Show (placementId);
    }
}