using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public enum AdPurpose
{
    AddEnergy,
    SpeedUpSeed
}

public class AdHandler : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] public AdPurpose AdPurpose;
    [SerializeField] public Button ShowAdButton;
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS";

    public Action SpeedUpAction { get; set; }

    private string _adUnitId;
    private Inventory _inventory;

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            ShowAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            if (ShowAdButton != null)
                ShowAdButton.interactable = true;
        }
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded: Ad Completed");

            // Grant a reward.
            switch (AdPurpose)
            {
                case AdPurpose.AddEnergy:
                    UpdateEnergy();
                    break;
                case AdPurpose.SpeedUpSeed:
                    SpeedUpAction.Invoke();
                    break;
                default:
                    Debug.Log("Действие за выполнение рекламы не назначено!");
                    break;
            }

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void DebugResetEnergy()
    {
        _inventory.ConsumeEnergy(_inventory.Energy);
    }

    private void UpdateEnergy()
    {
        _inventory.RegenEnergy(1);
    }

    // Crutch to guarantee sdk initialization
    private IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(1f);

        LoadAd();
    }

    // Load content to the Ad Unit:
    private void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // Implement a method to execute when the user clicks the button.
    private void ShowAd()
    {
        // Disable the button: 
        ShowAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    private void Awake()
    {
        _inventory ??= GameObject.Find("DataKeeper").GetComponent<Inventory>();
        // Get the Ad Unit ID for the current platform
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSAdUnitId
            : _androidAdUnitId;
        //Disable button until ad is ready to show
        if (ShowAdButton != null)
            ShowAdButton.interactable = false;
    }

    private void Start()
    {
        StartCoroutine(WaitForLoad());
    }

    private void OnDestroy()
    {
        // Clean up the button listeners:
        ShowAdButton.onClick.RemoveAllListeners();
    }
}
