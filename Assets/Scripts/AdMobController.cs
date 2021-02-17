using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdMobController : MonoBehaviour
{
    private RewardedAd[] loadedAd;
    private byte maxFailedLoad = 3;
    private byte index = 0;
    private byte countAds = 3;
    private byte[] countFailedLoad;
    private string adUnitId = "ca-app-pub-4592536507600053/3519149167";//test ca-app-pub-4592536507600053/3519149167
    void Start()
    {
        SetAdId();
        SetAllAds();
    }
    public void SetAllAds()
    {
        if(loadedAd == null)
        {
            countFailedLoad = new byte[countAds];
            loadedAd = new RewardedAd[countAds];
        }

        for (int i = 0; i < loadedAd.Length; i++)
        {
            if (loadedAd[i] != null)
                if (!loadedAd[i].IsLoaded())
                    break;
                else
                    if (countFailedLoad[i] != maxFailedLoad)
                    break;
            loadedAd[i] = SetReward();
            countFailedLoad[i] = 0;
        }
        
        index = 0;
    }
    private void SetAdId()
    {
        #if UNITY_ANDROID
                adUnitId = "ca-app-pub-4592536507600053/3519149167";
        #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
                adUnitId = "unexpected_platform";
        #endif
    }
    private RewardedAd SetReward()
    {
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        return rewardedAd;
    }
    private bool FailedLoadAllAds()
    {
        for(int i =0;i< countFailedLoad.Length; i++)
            if (countFailedLoad[i] < maxFailedLoad)
                return false;
        return true;
    }
    public void ShowReward()
    {
        if (!FailedLoadAllAds())
        {
            if (index < loadedAd.Length)
                if (loadedAd[index].IsLoaded())
                {
                    loadedAd[index].Show();
                    index = 0;
                }
                else
                {
                    index++;
                    ShowReward();
                }
            else
                SetAllAds();
        }
        else
        {
            SetAllAds();
        }
    }
    private sbyte IndexOfAd(RewardedAd ad)
    {
        for (sbyte i = 0; i < loadedAd.Length; i++)
            if (ad == loadedAd[i])
                return i;
        return -1;
    }
    //---------------------------------------------------------------------------------------------------------------//
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        sbyte index = IndexOfAd((RewardedAd)sender);
        if (index != -1)
            countFailedLoad[index] = 0;
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        sbyte index = IndexOfAd((RewardedAd)sender);
        if (index != -1)
            if (countFailedLoad[index] < maxFailedLoad)
            {
                countFailedLoad[index]++;
                loadedAd[index] = SetReward();
            }
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        this.index = 0;
        sbyte index = IndexOfAd((RewardedAd)sender);
        if (index != -1)
            loadedAd[index] = SetReward();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }
}
