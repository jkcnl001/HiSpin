using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOfferwallAds : MonoBehaviour
{
    void OnEnable()
    {
        IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
        IronSourceEvents.onOfferwallOpenedEvent += OfferwallOpenedEvent;
        IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
        IronSourceEvents.onOfferwallAdCreditedEvent += OfferwallAdCreditedEvent;
        IronSourceEvents.onGetOfferwallCreditsFailedEvent += GetOfferwallCreditsFailedEvent;
        IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;
    }
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
}
