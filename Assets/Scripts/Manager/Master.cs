using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Master : MonoBehaviour
{
    public static bool IsBigScreen = false;
    public static Master Instance;
    public Image bgImage;
    public Transform BaseRoot;
    public Transform MenuRoot;
    public Transform PopRoot;
    public GameObject AudioRoot;
    public AnimationCurve popAnimationScale;
    public AnimationCurve popAnimationAlpha;
    [HideInInspector]
    public UI UI;
    [HideInInspector]
    public Save Save;
    [HideInInspector]
    public Audio Audio;
    private void Awake()
    {
        Instance = this;
        UI = new UI(this, BaseRoot, MenuRoot, PopRoot);
        Save = new Save();
        Audio = new Audio(AudioRoot);
        IsBigScreen = (float)Screen.height / Screen.width > 16f / 9;
        bgImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1080 * (Screen.width / 1080f), 1920 * (Screen.height / 1920f));
    }
    private void Start()
    {
        UI.ShowPopPanel(PopPanel.Loading);
    }
    public static void PlayButtonClickSound()
    {
        Audio.PlayOneShot(AudioPlayArea.Button);
    }
    public void OnLoadingEnd()
    {
        UI.ShowMenuPanel();
    }
    public void ChangeBg(int index)
    {
        bgImage.sprite = Sprites.GetBGSprite("bg_" + index);
    }
    public void SetBgDefault()
    {
        bgImage.sprite = Sprites.GetBGSprite("bg");
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save.SaveLocalData();
        }
    }
    private void OnApplicationQuit()
    {
        Save.SaveLocalData();
    }
    public GameObject tipGo;
    public Text tipText;
    public void ShowTip(string content,float hideDelayTime = 1)
    {
        tipText.text = content;
        tipGo.SetActive(true);
        StopCoroutine("DelayHideTip");
        StartCoroutine("DelayHideTip", hideDelayTime);
    }
    private IEnumerator DelayHideTip(float time)
    {
        yield return new WaitForSeconds(time);
        tipGo.SetActive(false);
    }
    public void SendAdjustGameStartEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_open,
            ("player_id", Save.data.mainData.user_id),
            ("install_version", "1")
            );
    }
    public void SendAdjustPlayAdEvent(bool hasAd, bool isRewardAd, string adByWay)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(hasAd ? AdjustEventLogger.TOKEN_ad : AdjustEventLogger.TOKEN_noads,
            ("player_id", Save.data.mainData.user_id),
            //广告位置
            ("id", adByWay),
            //广告类型，0插屏1奖励视频
            ("type", isRewardAd ? "1" : "0"),
            //当前票
            ("other_int1", Save.data.mainData.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.mainData.user_gold_live.ToString())
            );
    }
    public void SendAdjustEnterSlotsEvent(bool isAd)
    {
        Save.data.enter_slots_time++;
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_end,
            ("player_id", Save.data.mainData.user_id),
            //第几个老虎机
            ("id", Save.data.enter_slots_time.ToString()),
            //老虎机类型
            ("type", isAd ? "1" : "0"),
            //累计美元
            ("other_int1", Save.data.mainData.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.mainData.user_gold_live.ToString())
            );
    }
    public void SendAdjustFinishTaskEvent(int taskId,int taskType,Reward rewardType,int rewardNum)
    {
#if UNITY_EDITOR
        return;
#endif
        int rewardTypeIndex = 0;
        switch (rewardType)
        {
            case Reward.Gold:
                rewardTypeIndex = 0;
                break;
            case Reward.Cash:
                rewardTypeIndex = 1;
                break;
            case Reward.Ticket:
                rewardTypeIndex = 2;
                break;
        }
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_task,
            ("player_id", Save.data.mainData.user_id),
            //任务id
            ("id", taskId.ToString()),
            //任务类型
            ("type", taskType.ToString()),
            //奖励数量
            ("value", rewardNum.ToString()),
            //奖励类型
            ("power_ok", rewardTypeIndex.ToString()),
            //累计美元
            ("other_int1", Save.data.mainData.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.mainData.user_gold_live.ToString())
            );
    }
    public void SendAdjustInputEmailEvent(string email)
    {
        Save.data.input_eamil_time++;
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_eamil,
            ("player_id", Save.data.mainData.user_id),
            //填写内容
            ("id", email),
            //第几次填写
            ("type", Save.data.input_eamil_time.ToString()),
            //累计美元
            ("other_int1", Save.data.mainData.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.mainData.user_gold_live.ToString())
            );
    }
    public void SendAdjustDeeplinkEvent(string uri)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_deeplink,
                ("link", uri),
                ("order_id", uri)
            );
    }
}
public enum Reward
{
    Null,
    Gold,
    Cash,
    Ticket,
}
