using FyberPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOfferwallAds : MonoBehaviour
{
    public Ad ofwAd = null;
    void OnEnable()
    {
        IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
        IronSourceEvents.onOfferwallOpenedEvent += OfferwallOpenedEvent;
        IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
        IronSourceEvents.onOfferwallAdCreditedEvent += OfferwallAdCreditedEvent;
        IronSourceEvents.onGetOfferwallCreditsFailedEvent += GetOfferwallCreditsFailedEvent;
        IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;


        FyberCallback.AdAvailable += OnAdAvailable;
        FyberCallback.AdNotAvailable += OnAdNotAvailable;
        FyberCallback.RequestFail += OnRequestFail;
    }
    #region is offerwall
    void OfferwallAvailableEvent(bool canShowOfferwall)
    {

    }
    void OfferwallOpenedEvent()
    {
    }
    void OfferwallShowFailedEvent(IronSourceError error)
    {
    }
    void OfferwallAdCreditedEvent(Dictionary<string, object> dict)
    {
    }
    void GetOfferwallCreditsFailedEvent(IronSourceError error)
    {
    }
    void OfferwallClosedEvent()
    {
    }
    #endregion
    #region fyber offerwall
    private void OnAdAvailable(Ad ad)
    {
        // store ad response
        if (ad.AdFormat == AdFormat.OFFER_WALL)
            ofwAd = ad;
    }

    private void OnAdNotAvailable(AdFormat adFormat)
    {
        // discard previous stored response
        if (adFormat == AdFormat.OFFER_WALL)
            ofwAd = null;
    }

    private void OnRequestFail(RequestError error)
    {
        // process error
        Debug.Log("OnRequestError: " + error.Description);
    }
    #endregion
    void OnDisable()
    {
        FyberCallback.AdAvailable -= OnAdAvailable;
        FyberCallback.AdNotAvailable -= OnAdNotAvailable;
        FyberCallback.RequestFail -= OnRequestFail;
    }
}
