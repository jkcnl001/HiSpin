using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIntersitalAds : MonoBehaviour
{
    private Ads adM;
    // Start is called before the first frame update
    void Start()
    {
        adM = GetComponent<Ads>();
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
    }

    private void InterstitialAdClosedEvent()
    {
        IronSource.Agent.loadInterstitial();
        Audio.PauseBgm(false);
        adM.InvokePopAd();
    }

    private void InterstitialAdOpenedEvent()
    {
        Audio.PauseBgm(true);
    }

    private void InterstitialAdClickedEvent()
    {
    }

    private void InterstitialAdShowFailedEvent(IronSourceError obj)
    {
        IronSource.Agent.loadInterstitial();
        Master.Instance.SendAdjustPlayAdEvent(false, false, adM.adDes);
        adM.InvokePopAd();
    }

    private void InterstitialAdShowSucceededEvent()
    {
        Master.Instance.SendAdjustPlayAdEvent(true, false, adM.adDes);
    }

    private void InterstitialAdLoadFailedEvent(IronSourceError obj)
    {
        IronSource.Agent.loadInterstitial();
    }

    private void InterstitialAdReadyEvent()
    {
    }
}
            