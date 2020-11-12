using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public class Loading : MonoBehaviour,IUIBase
{
    public Slider progressSlider;
    public Text progressText;
    public Text loadingText;
    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        loadingText.text = "loading";
        StartCoroutine(LoadingSlider());
    }
    IEnumerator LoadingSlider()
    {
        progressSlider.value = 0;
        progressText.text = "0%";
        float progress = 0;
        float speed = 1f;
        float loadingPointInterval = 1f;
        float intervalTimer = 0;
        bool hasRequestData = false;
        if(!Save.data.isPackB)
            StartCoroutine("WaitFor");
        while (progress < 1)
        {
            yield return null;
            float deltatime = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
            intervalTimer += deltatime;
            if (intervalTimer >= loadingPointInterval)
            {
                intervalTimer = 0;
                loadingText.text += ".";
                if (loadingText.text.Length > 10)
                    loadingText.text = "loading";
            }
            progress += deltatime * speed;
            progress = Mathf.Clamp(progress, 0, 1);
            if (!hasRequestData)
            {
                if (progress > 0.3f)
                {
                    speed = 0;
                    Server.Instance.RequestData(Server.Server_RequestType.AllData, () => { speed = 1; }, () => { speed = 0; }, false);
                    hasRequestData = true;
                }
            }
            progressSlider.value = progress;
            progressText.text = (int)(progress * 100) + "%";
        }
        StopCoroutine("WaitFor");
        UI.ClosePopPanel(this);
        Master.Instance.OnLoadingEnd();
    }
    IEnumerator WaitFor()
    {
#if UNITY_EDITOR
        yield break;
#endif
#if UNITY_ANDROID
        UnityWebRequest webRequest = new UnityWebRequest(string.Format("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package={0}&version={1}&os=android", Master.PackageName, Master.Version));
#elif UNITY_IOS
            UnityWebRequest webRequest = new UnityWebRequest(string.Format("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package={0}&version={1}&os=ios", Master.PackageName, Master.Version));
#endif
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.responseCode == 200)
        {
            if (webRequest.downloadHandler.text.Equals("{\"store_review\": true}"))
                Save.data.isPackB = true;
        }
    }
    public IEnumerator Show(params int[] args)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        yield return null;
    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }

    public void Resume()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        Destroy(gameObject);
        yield return null;
    }
}

