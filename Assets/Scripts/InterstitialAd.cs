// using System;
using UnityEngine;
using UnityEngine.Advertisements;
// using ShowResult = UnityEngine.Advertisements.ShowResult;
// public class InterstitialAd : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
public class InterstitialAd : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";

    // skip add this time using boolean
    public static bool isAddRunning = false;
    string _adUnitId;

    [SerializeField] string _gameId;
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = false;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;
        // Advertisement.AddListener(this);
        if (!Advertisement.isInitialized)
        {
            InitializeAds();
        }
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        int adsCounter = PlayerPrefs.GetInt("Ads", 0);
        if (adsCounter < 2)
        {
            PlayerPrefs.SetInt("Ads", adsCounter + 1);
            return;
        }
        PlayerPrefs.SetInt("Ads", 0);
        isAddRunning = true;
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Advertisement.Show(_adUnitId, this);
        print("Showing Ad: " + _adUnitId);
    }

    // Implement Load Listener Listener interface methods: 
    // IUnityAdsLoadListener start
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        print("Ad loaded");
        ShowAd();
    }
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        isAddRunning = false;
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    // IUnityAdsLoadListener end


    // Implement Show Listener interface methods: 
    // IUnityAdsShowListener start
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        isAddRunning = false;
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        print("OnUnityAdsShowComplete");
        isAddRunning = false;
    }

    // intialize ads
    public void InitializeAds()
    {
        print("Initializing ads");
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }


    // Implement Initialization Listener interface methods:
    // IUnityAdsInitializationListener start
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    // IUnityAdsInitializationListener send
}
