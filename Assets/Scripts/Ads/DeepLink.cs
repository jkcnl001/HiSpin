using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeepLink : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void getDeepLink(string objName);
#endif
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
#if UNITY_ANDROID
        AndroidJavaClass aj = new AndroidJavaClass("com.wyx.deeplink.GetDeepLinkURI");
        aj.CallStatic("start", gameObject.name);
#elif UNITY_IOS && !UNITY_EDITOR
        getDeepLink(gameObject.name);
#endif
    }
    
    public void ReceiveURI(string uri)
    {
        Debug.unityLogger.logEnabled = true;
        Debug.Log("收到DeepLink回调" + uri);
        Debug.unityLogger.logEnabled = false;
        if (!string.IsNullOrEmpty(uri))
        {
            Master.Instance.SendAdjustDeeplinkEvent(uri);
        }
    }
}
        