using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Ads : MonoBehaviour
{
	//ios FBID 728969364687204  Hi Spin
#if UNITY_ANDROID
	private const string APP_KEY = "de040b19";
#elif UNITY_IOS
	private const string APP_KEY = "debe9209";
#endif
	public static Ads _instance;
	[NonSerialized]
	public string adDes = string.Empty;
	public const string AppName = "A048_HiSpin";
	private void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		//Dynamic config example
		IronSourceConfig.Instance.setClientSideCallbacks(true);

		string id = IronSource.Agent.getAdvertiserId();

		//IronSource.Agent.validateIntegration();

		// SDK init
		IronSource.Agent.init(APP_KEY);
		IronSource.Agent.loadInterstitial();

	}
	public bool ShowOfferwallAd()
	{
#if UNITY_EDITOR
		Debug.Log("Show OW.");
		return true;
#endif
		if (IronSource.Agent.isOfferwallAvailable())
		{
			IronSource.Agent.showOfferwall();
			return true;
		}
		return false;
	}
	public bool ShowRewardVideo(Action rewardedCallback, int clickAdTime,string des,Action failCallback)
	{
		adDes = des;
		rewardCallback = rewardedCallback;
		rewardFailCallback = failCallback;
#if UNITY_EDITOR
        Server.Instance.OperationData_RvEvent(rewardCallback, null);
		TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WatchRvOnce, 1);
		Debug.Log("RV:【" + des + "】");
        return true;
#endif
#if UNITY_IOS
		if (!Save.data.isPackB)
		{
			rewardCallback?.Invoke();
			return true;
		}
#endif
        if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
			return true;
		}
		else
		{
			StartCoroutine(WaitLoadAD(true,clickAdTime));
			return false;
		}
	}
	float interstialLasttime = 0;
	public void ShowInterstialAd(Action callback, string des)
	{
		popCallback = callback;
		adDes = des;
#if UNITY_EDITOR
        callback?.Invoke();
        Debug.Log("IV:【" + des + "】");
        return;
#endif
#if UNITY_IOS
		if (!Save.data.isPackB) 
		{
			callback?.Invoke();
			return;
		}
#endif
        if (timer - interstialLasttime < 60)
        {
			callback?.Invoke();
			return;
        }
		if (IronSource.Agent.isInterstitialReady())
		{
			interstialLasttime = timer;
			IronSource.Agent.showInterstitial();
		}
		else
		{
			callback?.Invoke();
			Master.Instance.SendAdjustPlayAdEvent(false, false, adDes);
		}
	}
	void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}
	public GameObject adLoadingTip;
	const string text = "No Video is ready , please try again later.";
	IEnumerator WaitLoadAD(bool isRewardedAd,int clickAdTime)
	{
		adLoadingTip.SetActive(true);
		StringBuilder content = new StringBuilder("Loading.");
		Text noticeText = adLoadingTip.GetComponentInChildren<Text>();
		noticeText.text = content.ToString();
		int timeOut = 6;
		while (timeOut > 0)
		{
			yield return new WaitForSeconds(Time.timeScale);
			timeOut--;
			content.Append('.');
			noticeText.text = content.ToString();
			if (isRewardedAd && IronSource.Agent.isRewardedVideoAvailable())
			{
				IronSource.Agent.showRewardedVideo();
				adLoadingTip.SetActive(false);
				yield break;
			}
		}
		adLoadingTip.SetActive(false);
		Master.Instance.SendAdjustPlayAdEvent(false, true, adDes);
		if (clickAdTime >= 2)
		{
			rewardFailCallback?.Invoke();
			Master.Instance.ShowTip(text, 2);
		}
	}
	Action rewardCallback;
	Action rewardFailCallback;
	private bool canGetReward = false;
	public void GetReward()
	{
		canGetReward = true;
	}
	public void InvokeGetRewardMethod()
	{
		if (canGetReward)
		{
			Server.Instance.OperationData_RvEvent(rewardCallback, null);
			TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WatchRvOnce, 1);
			canGetReward = false;
		}
	}
	Action popCallback;
	public void InvokePopAd()
    {
		popCallback?.Invoke();
    }
	float timer = 0;
	bool isOut = false;
    private void Update()
    {
		if (!isOut)
			timer += Time.deltaTime;
    }
    private void OnApplicationFocus(bool focus)
    {
		isOut = !focus;
    }
}
       