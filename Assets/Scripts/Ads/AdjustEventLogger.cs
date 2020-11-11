using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class AdjustEventLogger : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string Getidfa();
#endif
#if UNITY_IOS
    public const string APP_TOKEN = "";
#elif UNITY_ANDROID
    public const string APP_TOKEN = "nwle7bsx7itc";
    public const string TOKEN_open = "3grjd1";
    public const string TOKEN_ad = "yfwijb";
    public const string TOKEN_noads = "o3cdw4";
    public const string TOKEN_stage_end = "a5iml7";
    public const string TOKEN_task = "h4f80y";
    public const string TOKEN_eamil = "9o2t98";
    public const string TOKEN_deeplink = "cpubgo";
#endif
    public static AdjustEventLogger Instance;
    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Sandbox);
#else
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Production);
#endif
        adjustConfig.sendInBackground = true;
        adjustConfig.launchDeferredDeeplink = true;
        adjustConfig.eventBufferingEnabled = true;
        adjustConfig.logLevel = AdjustLogLevel.Info;
        adjustConfig.setAttributionChangedDelegate(OnAttributionChangedCallback);
        Adjust.start(adjustConfig);
    }
    private void Start()
    {
        GetAdID();
        StartCoroutine(CheckAttributeTo());
        Master.Instance.SendAdjustGameStartEvent();
    }
    public void AdjustEventNoParam(string token)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        Adjust.trackEvent(adjustEvent);
    }
    public void AdjustEvent(string token, params (string key, string value)[] list)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        foreach (var (key, value) in list)
        {
            adjustEvent.addCallbackParameter(key, value);
        }
        Adjust.trackEvent(adjustEvent);
    }
    void OnAttributionChangedCallback(AdjustAttribution attribution)
    {
        if (attribution.network.Equals("Organic"))
        {
        }
        else
        {
            if (!Master.isLoadingEnd)
            {
                Save.data.isPackB = true;
            }
        }
    }
    private string AppName = Ads.AppName;
    private string ifa;
    private IEnumerator CheckAttributeTo()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(ifa));
        string url = string.Format("http://a.mobile-mafia.com:3838/adjust/is_unity_clicked?ifa={0}&app_name={1}", ifa, AppName);

        var web = new UnityWebRequest(url);
        web.downloadHandler = new DownloadHandlerBuffer();
        yield return web.SendWebRequest();
        if (web.responseCode == 200)
        {
            if (web.downloadHandler.text.Equals("1"))
            {
                if (!Master.isLoadingEnd)
                {
                    Save.data.isPackB = true;
                }
            }
        }
    }
    private void GetAdID()
    {
#if UNITY_ANDROID
        Application.RequestAdvertisingIdentifierAsync(
           (string advertisingId, bool trackingEnabled, string error) =>
           {
               ifa = advertisingId;
           }
       );
#elif UNITY_IOS && !UNITY_EDITOR
        ifa = Getidfa();
#endif
    }
}

    