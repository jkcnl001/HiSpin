using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Master : MonoBehaviour
{
    public static bool IsBigScreen = false;
    public static float ExpandCoe = 1;
    public const float TopMoveDownOffset = 100;
    public const string PackageName = "com.HiSpin.DailyCash.HugeRewards.FreeGame";
    public const int Version = 3;
    public const string AppleId = "";
    public static bool isLoadingEnd = false;
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
        float coe = (float)Screen.height / Screen.width;
        float originCoe = 16f / 9;
        ExpandCoe = coe > originCoe ? coe / originCoe : originCoe / coe;
        IsBigScreen = ExpandCoe != 1;
        bgImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1080 * ExpandCoe, 1920 * ExpandCoe);
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
        isLoadingEnd = true;
        CheckLocalSavaData();
        StartTimeDown();
        if (!Save.data.isPackB)
            Save.data.isPackB = Save.data.allData.fission_info.up_user;
        UI.ShowMenuPanel();
    }
    public void StartTimeDown()
    {
        StopCoroutine("AutoTimedown");
        StartCoroutine("AutoTimedown", Save.data.allData.get_time.server_time + 5);
    }
    private void CheckLocalSavaData()
    {
        List<bool> head_new = Save.data.head_icon_hasCheck;
        if (head_new == null || head_new.Count != Save.data.allData.user_panel.title_list.Count)
        {
            int count = Save.data.allData.user_panel.title_list.Count;
            head_new = new List<bool>();
            for(int i = 0; i < count; i++)
            {
                head_new.Add(false);
            }
            if (head_new.Count > 1)
                head_new[0] = true;
        }
        Save.data.head_icon_hasCheck = head_new;
    }
    public void OnChangePackb()
    {
        Slots slots = UI.GetUI(BasePanel.Slots) as Slots;
        if (slots != null)
            slots.OnChangePackB();
        Friends friends = UI.GetUI(BasePanel.Friend) as Friends;
        if (friends != null)
            friends.OnChangePackB();
        Setting setting = UI.GetUI(PopPanel.Setting) as Setting;
        if (setting != null)
            setting.OnChangePackB();
    }
    public void ChangeBg(int index)
    {
        bgImage.sprite = Sprites.GetBGSprite("bg_" + index);
    }
    public static string time;
    private IEnumerator AutoTimedown(int leftSeconds)
    {
        WaitForSeconds oneSecond = new WaitForSeconds(1);
        while (true)
        {
            int second = leftSeconds % 60;
            int minute = leftSeconds % 3600 / 60;
            int hour = leftSeconds / 3600;
            time = (hour < 10 ? "0" + hour : hour.ToString()) + ":" + (minute < 10 ? "0" + minute : minute.ToString()) + ":" + (second < 10 ? "0" + second : second.ToString());
            Slots slots = UI.GetUI(BasePanel.Slots) as Slots;
            if (slots != null)
                slots.UpdateTimedownText(time);
            Betting betting = UI.GetUI(BasePanel.Betting) as Betting;
            if (betting != null)
                betting.UpdateTimeDownText(time);
            yield return oneSecond;
            leftSeconds--;
            if (leftSeconds == 0)
            {
                RequestAllData();
                yield break;
            }
        }
    }
    private void RequestAllData()
    {
        Server.Instance.RequestData(Server.Server_RequestType.AllData, OnRequestAllDataCallback, null);
    }
    private void OnRequestAllDataCallback()
    {
        StartTimeDown();
        UI.MenuPanel.RefreshTokenText();
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Slots))
        {
            Slots slots = UI.GetUI(BasePanel.Slots) as Slots;
            slots.RefreshSlotsCardState();
        }
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Task))
        {
            Tasks tasks = UI.GetUI(BasePanel.Task) as Tasks;
            tasks.RefreshTaskInfo();
        }
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Betting))
        {
            Betting betting = UI.GetUI(BasePanel.Betting) as Betting;
            betting.RefreshBettingWinner();
        }
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Rank))
        {
            Rank rank = UI.GetUI(BasePanel.Rank) as Rank;
            rank.RefreshRankList();
        }
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.Friend))
        {
            Friends friends = UI.GetUI(BasePanel.Friend) as Friends;
            friends.RefreshFriendList();
        }
        if (UI.CurrentBasePanel == UI.GetUI(BasePanel.CashoutRecord))
        {
            CashoutRecord cashoutRecord = UI.GetUI(BasePanel.CashoutRecord) as CashoutRecord;
            cashoutRecord.InitRecord();
        }
    }
    public void SetBgDefault()
    {
        bgImage.sprite = Sprites.GetBGSprite("bg");
    }
    public void AddLocalExp(int value)
    {
        Save.data.allData.user_panel.user_exp += value;
        UI.MenuPanel.UpdateHeadIcon();
        if (Save.data.allData.user_panel.user_exp >= Save.data.allData.user_panel.level_exp)
        {
            Save.data.allData.user_panel.user_level++;
            Save.data.allData.user_panel.user_exp -= Save.data.allData.user_panel.level_exp;
            UI.ShowPopPanel(PopPanel.GetReward, (int)Save.data.allData.user_panel.level_type, Save.data.allData.user_panel.next_level, (int)GetRewardArea.LevelUp, Save.data.allData.user_panel.user_level);
        }
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
            ("player_id", Save.data.allData.user_panel.user_id),
            ("install_version", "1")
            );
    }
    public void SendAdjustPlayAdEvent(bool hasAd, bool isRewardAd, string adByWay)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(hasAd ? AdjustEventLogger.TOKEN_ad : AdjustEventLogger.TOKEN_noads,
            ("player_id", Save.data.allData.user_panel.user_id),
            //广告位置
            ("id", adByWay),
            //广告类型，0插屏1奖励视频
            ("type", isRewardAd ? "1" : "0"),
            //当前票
            ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
            );
    }
    public void SendAdjustEnterSlotsEvent(bool isAd)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_end,
            ("player_id", Save.data.allData.user_panel.user_id),
            //第几个老虎机
            ("id", Save.data.allData.user_panel.lucky_count.ToString()),
            //老虎机类型
            ("type", isAd ? "1" : "0"),
            //累计美元
            ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
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
            ("player_id", Save.data.allData.user_panel.user_id),
            //任务id
            ("id", taskId.ToString()),
            //任务类型
            ("type", taskType.ToString()),
            //奖励数量
            ("value", rewardNum.ToString()),
            //奖励类型
            ("power_ok", rewardTypeIndex.ToString()),
            //累计美元
            ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
            );
    }
    public void SendAdjustInputEmailEvent(string email)
    {
        Save.data.input_eamil_time++;
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_eamil,
            ("player_id", Save.data.allData.user_panel.user_id),
            //填写内容
            ("id", email),
            //第几次填写
            ("type", Save.data.input_eamil_time.ToString()),
            //累计美元
            ("other_int1", Save.data.allData.user_panel.user_tickets.ToString()),
            //当前金币
            ("other_int2", Save.data.allData.user_panel.user_gold_live.ToString())
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
