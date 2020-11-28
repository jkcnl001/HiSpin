using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class Server_New : MonoBehaviour
{
#if UNITY_ANDROID
    public const string Platform = "android";
#elif UNITY_IOS
    public const string Platform = "ios";
#endif
    public const string Bi_name = Ads.AppName;
    public static Server_New Instance;
    public CanvasGroup canvasGroup;
    public Image state_iconImage;
    public Text titleText;
    public Text tipText;
    public Button retryButton;
    public static string localCountry = "";
    static string adID = "";
    private void Awake()
    {
        Instance = this;
        GetAdID();
        retryButton.AddClickEvent(OnRetryButtonClick);
    }
    private void OnRetryButtonClick()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        ConnectToServer(RequestType, ServerResponseOkCallback, ServerResponseErrorCallback, NetworkErrorCallback, ShowConnectingWindow, Args);
    }
    static bool isConnecting = false;
    private IEnumerator WaitConnecting()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        titleText.text = connectingTilte;
        tipText.text = connectingString;
        state_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Server, "loading");
        retryButton.gameObject.SetActive(false);
        while (isConnecting)
        {
            yield return null;
            state_iconImage.transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 300));
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string Getidfa();
#endif
    private void GetAdID()
    {
#if UNITY_EDITOR
        adID = SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
        Application.RequestAdvertisingIdentifierAsync(
           (string advertisingId, bool trackingEnabled, string error) =>
           {
               adID = advertisingId;
           }
       );
#elif UNITY_IOS && !UNITY_EDITOR
         adID = Getidfa();
#endif
    }
    public enum Server_RequestType
    {
        AllData,
        TaskData,

        ClickSlotsCard,
        GetSlotsReward,
        FinishTask,
        BuyTickets,
        WatchRvEvent,
        BindPaypal,
        Cashout,
        GetLocalCountry,
        OpenBettingPrize,
        ChangeHead_Name,
        GetLevelUpReward,
        GetNewPlayerReward,
        GetUUID,
        GetCashoutRecordList,
    }
    static readonly Dictionary<Server_RequestType, string> getdata_uri_dic = new Dictionary<Server_RequestType, string>()
    {
        {Server_RequestType.AllData,"http://admin.crsdk.com:8000/lucky_all_data/" },
        {Server_RequestType.TaskData,"http://admin.crsdk.com:8000/lucky_schedule/" },

        {Server_RequestType.ClickSlotsCard,"http://admin.crsdk.com:8000/lucky_free/" },
        {Server_RequestType.GetSlotsReward,"http://admin.crsdk.com:8000/lucky_reward/" },
        {Server_RequestType.FinishTask,"http://admin.crsdk.com:8000/lucky_task/" },
        {Server_RequestType.BuyTickets,"http://admin.crsdk.com:8000/lucky_exchange/" },
        {Server_RequestType.WatchRvEvent,"http://admin.crsdk.com:8000/lucky_rv/" },
        {Server_RequestType.BindPaypal,"http://admin.crsdk.com:8000/lucky_paypal/" },
        {Server_RequestType.Cashout,"http://admin.crsdk.com:8000/lucky_apply/" },
        {Server_RequestType.GetLocalCountry,"https://a.mafiagameglobal.com/event/country/" },
        {Server_RequestType.OpenBettingPrize,"http://admin.crsdk.com:8000/lucky_flag/" },
        {Server_RequestType.ChangeHead_Name,"http://admin.crsdk.com:8000/update_user/" },
        {Server_RequestType.GetLevelUpReward,"http://admin.crsdk.com:8000/level_reward/" },
        {Server_RequestType.GetNewPlayerReward,"http://admin.crsdk.com:8000/new_data/" },
        {Server_RequestType.GetUUID,"http://aff.luckyclub.vip:8000/get_random_id/" },
        {Server_RequestType.GetCashoutRecordList,"http://admin.crsdk.com:8000/lucky_record/" },
    };
    Server_RequestType RequestType;
    Action ServerResponseOkCallback;
    Action ServerResponseErrorCallback;
    Action NetworkErrorCallback;
    bool ShowConnectingWindow;
    string[] Args;
    private void ConnectToServer(Server_RequestType _RequestType,Action  _ServerResponseOkCallback,Action _ServerResponseErrorCallback,Action _NetworkErrorCallback,bool _ShowConnectingWindow,params string[] _Args)
    {
        RequestType = _RequestType;
        ServerResponseOkCallback = _ServerResponseOkCallback;
        ServerResponseErrorCallback = _ServerResponseErrorCallback;
        NetworkErrorCallback = _NetworkErrorCallback;
        ShowConnectingWindow = _ShowConnectingWindow;
        Args = _Args;
        StartCoroutine(ConnectToServerThread(_RequestType, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, Args));
    }
    private IEnumerator ConnectToServerThread(Server_RequestType _RequestType, Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow, params string[] _Args)
    {
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        #region request uuid
        if (string.IsNullOrEmpty(Save.data.uuid))
        {
            UnityWebRequest requestUUID = UnityWebRequest.Get(getdata_uri_dic[Server_RequestType.GetUUID]);
            yield return requestUUID.SendWebRequest();
            if (requestUUID.isNetworkError || requestUUID.isHttpError)
            {
                OnConnectServerFail();
                _NetworkErrorCallback?.Invoke();
            }
            else
            {
                string downText = requestUUID.downloadHandler.text;
                Save.data.uuid = downText;
            }
            requestUUID.Dispose();
        }
        #endregion
        iparams.Add(new MultipartFormDataSection("uuid", Save.data.uuid));
        if (!string.IsNullOrEmpty(adID))
        {
            Save.data.adid = adID;
            iparams.Add(new MultipartFormDataSection("device_id", adID));
        }
        switch (_RequestType)
        {
            case Server_RequestType.AllData:
                #region request country
                if (string.IsNullOrEmpty(localCountry))
                {
                    UnityWebRequest requestCountry = UnityWebRequest.Get(getdata_uri_dic[Server_RequestType.GetLocalCountry]);
                    yield return requestCountry.SendWebRequest();
                    if (requestCountry.isNetworkError || requestCountry.isHttpError)
                    {
                        OnConnectServerFail();
                        _NetworkErrorCallback?.Invoke();
                    }
                    else
                    {
                        string downText = requestCountry.downloadHandler.text;
                        LocalCountryData countryData = JsonMapper.ToObject<LocalCountryData>(downText);
                        localCountry = countryData.country.ToLower();
                    }
                    requestCountry.Dispose();
                }
                #endregion
                iparams.Add(new MultipartFormDataSection("app_name", Bi_name));
                iparams.Add(new MultipartFormDataSection("country", localCountry));
                iparams.Add(new MultipartFormDataSection("ad_ios", Platform));
                break;
            case Server_RequestType.TaskData:
                iparams.Add(new MultipartFormDataSection("app_name", Bi_name));
                break;
            case Server_RequestType.ClickSlotsCard:
                iparams.Add(new MultipartFormDataSection("lucky_id", _Args[0]));
                break;
            case Server_RequestType.GetSlotsReward:
                iparams.Add(new MultipartFormDataSection("reward_type",_Args[0]));//获得奖励类型
                iparams.Add(new MultipartFormDataSection("reward_num", _Args[1]));//获得奖励数量
                break;
            case Server_RequestType.FinishTask:
                iparams.Add(new MultipartFormDataSection("task_id", _Args[0]));
                iparams.Add(new MultipartFormDataSection("double", _Args[1]));//是否双倍
                break;
            case Server_RequestType.BuyTickets:
                iparams.Add(new MultipartFormDataSection("reward_type", _Args[0]));//是否是看广告购买
                break;
            case Server_RequestType.WatchRvEvent:
                break;
            case Server_RequestType.BindPaypal:
                iparams.Add(new MultipartFormDataSection("paypal", _Args[0]));
                break;
            case Server_RequestType.Cashout:
                iparams.Add(new MultipartFormDataSection("app_name", Bi_name));
                iparams.Add(new MultipartFormDataSection("withdrawal_type", _Args[0]));//提现消耗类型
                iparams.Add(new MultipartFormDataSection("withdrawal", _Args[1]));//提现消耗数量
                iparams.Add(new MultipartFormDataSection("doller", _Args[2]));//提现现金数量
                break;
            case Server_RequestType.GetLocalCountry:
                Master.Instance.ShowTip("Error code : can not use this connecting.");
                isConnecting = false;
                OnConnectServerSuccess();
                yield break;
            case Server_RequestType.OpenBettingPrize:
                break;
            case Server_RequestType.ChangeHead_Name:
                if (!string.IsNullOrEmpty(_Args[0]))
                    iparams.Add(new MultipartFormDataSection("title_id", _Args[0]));//新头像id
                if (!string.IsNullOrEmpty(_Args[1]))
                    iparams.Add(new MultipartFormDataSection("user_name", _Args[1]));//新名字
                break;
            case Server_RequestType.GetLevelUpReward:
                iparams.Add(new MultipartFormDataSection("double", _Args[0]));//升级奖励倍数
                break;
            case Server_RequestType.GetNewPlayerReward:
                iparams.Add(new MultipartFormDataSection("double", _Args[0]));//奖励倍数，默认1
                break;
            case Server_RequestType.GetUUID:
                Master.Instance.ShowTip("Error code : can not use this connecting.");
                isConnecting = false;
                OnConnectServerSuccess();
                yield break;
            case Server_RequestType.GetCashoutRecordList:
                break;
            default:
                break;
        }
        if (_ShowConnectingWindow)
            OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[_RequestType], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            _NetworkErrorCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            www.Dispose();
            if(int.TryParse(downText,out int errorcode) && errorcode < 0)
            {
                ShowConnectErrorTip(downText);
                _ServerResponseErrorCallback?.Invoke();
            }
            else
            {
                switch (_RequestType)
                {
                    case Server_RequestType.AllData:
                        AllData allData = JsonMapper.ToObject<AllData>(downText);
                        Save.data.allData = allData;
                        Save.data.uuid = string.IsNullOrEmpty(allData.user_uuid) ? Save.data.uuid : allData.user_uuid;
                        Ads._instance.InitFyber(allData.user_uuid);
                        break;
                    case Server_RequestType.TaskData:
                        AllData_TaskData taskData = JsonMapper.ToObject<AllData_TaskData>(downText);
                        Save.data.allData.lucky_schedule = taskData;
                        break;
                    case Server_RequestType.ClickSlotsCard:
                        AllData_SlotsState slotsStateData = JsonMapper.ToObject<AllData_SlotsState>(downText);
                        Save.data.allData.lucky_status = slotsStateData;
                        break;
                    case Server_RequestType.GetSlotsReward:
                        PlayerGetSlotsRewardReceiveData receiveSlotsRewardData = JsonMapper.ToObject<PlayerGetSlotsRewardReceiveData>(downText);
                        int slotsRewardType = int.Parse(_Args[0]);
                        //int slotsRewardNum = int.Parse(_Args[1]);
                        if (slotsRewardType == 0)
                        {
                            Save.data.allData.user_panel.user_gold = receiveSlotsRewardData.user_gold;
                            Save.data.allData.user_panel.user_gold_live = receiveSlotsRewardData.user_gold_live;
                        }
                        else if (slotsRewardType == 2)
                        {
                            Save.data.allData.user_panel.user_doller = receiveSlotsRewardData.user_doller;
                            Save.data.allData.user_panel.user_doller_live = receiveSlotsRewardData.user_doller_live;
                        }
                        else
                            Save.data.allData.user_panel.user_tickets = receiveSlotsRewardData.user_tickets;
                        break;
                    case Server_RequestType.FinishTask:
                        PlayerFinishTaskReceiveData receiveFinishTaskData = JsonMapper.ToObject<PlayerFinishTaskReceiveData>(downText);
                        int length = _Args.Length;
                        for (int i = 2; i < length; i++)//0为任务id，1为是否双倍
                        {
                            Reward taskChangeTokenType = (Reward)Enum.Parse(typeof(Reward), _Args[i]);
                            switch (taskChangeTokenType)
                            {
                                case Reward.Gold:
                                    Save.data.allData.user_panel.user_gold = receiveFinishTaskData.user_gold;
                                    Save.data.allData.user_panel.user_gold_live = receiveFinishTaskData.user_gold_live;
                                    break;
                                case Reward.Cash:
                                    Save.data.allData.user_panel.user_doller = receiveFinishTaskData.user_doller;
                                    Save.data.allData.user_panel.user_doller_live = receiveFinishTaskData.user_doller_live;
                                    break;
                                case Reward.Ticket:
                                    Save.data.allData.user_panel.user_tickets = receiveFinishTaskData.user_tickets;
                                    break;
                                default:
                                    Debug.LogError("错误的任务完成变动类型");
                                    break;
                            }
                        }
                        break;
                    case Server_RequestType.BuyTickets:
                        PlayerBuyTicketReceiveData receiveBuyTicketsData = JsonMapper.ToObject<PlayerBuyTicketReceiveData>(downText);
                        int isRv = int.Parse(_Args[0]);
                        if (isRv == 1)
                            Save.data.allData.user_panel.user_tickets = receiveBuyTicketsData.user_tickets;
                        else
                        {
                            Save.data.allData.user_panel.user_gold = receiveBuyTicketsData.user_gold;
                            Save.data.allData.user_panel.user_gold_live = receiveBuyTicketsData.user_gold_live;
                            Save.data.allData.user_panel.user_tickets = receiveBuyTicketsData.user_tickets;
                        }
                        break;
                    case Server_RequestType.WatchRvEvent:
                        break;
                    case Server_RequestType.BindPaypal:
                        PlayerBindPaypalReceiveData paypalData = JsonMapper.ToObject<PlayerBindPaypalReceiveData>(downText);
                        Save.data.allData.user_panel.user_paypal = paypalData.user_paypal;
                        break;
                    case Server_RequestType.Cashout:
                        PlayerCashoutReceiveData receiveCashoutData = JsonMapper.ToObject<PlayerCashoutReceiveData>(downText);
                        CashoutType cashoutType = (CashoutType)int.Parse(_Args[0]);
                        switch (cashoutType)
                        {
                            case CashoutType.PT:
                                Save.data.allData.fission_info.live_balance = receiveCashoutData.user_coin_live;
                                break;
                            case CashoutType.Cash:
                                Save.data.allData.user_panel.user_doller_live = receiveCashoutData.user_doller_live;
                                break;
                            case CashoutType.Gold:
                                Save.data.allData.user_panel.user_gold_live = receiveCashoutData.user_gold_live;
                                break;
                        }
                        break;
                    case Server_RequestType.GetLocalCountry:
                        throw new ArgumentNullException("can not use this connecting.");
                    case Server_RequestType.OpenBettingPrize:
                        Save.data.allData.day_flag = true;
                        break;
                    case Server_RequestType.ChangeHead_Name:
                        if (!string.IsNullOrEmpty(_Args[0]))
                            Save.data.allData.user_panel.user_title = int.Parse(_Args[0]);
                        if (!string.IsNullOrEmpty(_Args[1]))
                            Save.data.allData.user_panel.user_name = _Args[1];
                        break;
                    case Server_RequestType.GetLevelUpReward:
                        GetLevelupRewardReceiveData receiveLevelupData = JsonMapper.ToObject<GetLevelupRewardReceiveData>(downText);
                        Save.data.allData.user_panel.user_tickets = receiveLevelupData.user_tickets;
                        Save.data.allData.user_panel.user_level = receiveLevelupData.user_level;
                        Save.data.allData.user_panel.user_double = receiveLevelupData.user_double;
                        Save.data.allData.user_panel.next_double = receiveLevelupData.next_double;
                        Save.data.allData.user_panel.level_exp = receiveLevelupData.level_exp;
                        Save.data.allData.user_panel.user_exp = receiveLevelupData.user_exp;
                        Save.data.allData.user_panel.title_list = receiveLevelupData.title_list;
                        Save.data.allData.user_panel.next_level = receiveLevelupData.next_level;
                        break;
                    case Server_RequestType.GetNewPlayerReward:
                        GetNewPlayerRewardReceiveData receiveNewPlayerData = JsonMapper.ToObject<GetNewPlayerRewardReceiveData>(downText);
                        Save.data.allData.user_panel.user_doller_live = receiveNewPlayerData.user_doller_live;
                        break;
                    case Server_RequestType.GetUUID:
                        throw new ArgumentNullException("can not use this connecting.");
                    case Server_RequestType.GetCashoutRecordList:
                        AllData_CashoutRecordData casoutRecordData = JsonMapper.ToObject<AllData_CashoutRecordData>(downText);
                        Save.data.allData.lucky_record = casoutRecordData;
                        break;
                    default:
                        break;
                }
                _ServerResponseOkCallback?.Invoke();
            }
        }
    }
    public void ConnectToServer_GetAllData(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow)
    {
        ConnectToServer(Server_RequestType.AllData, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow);
    }
    public void ConnectToServer_GetTaskData(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow)
    {
        ConnectToServer(Server_RequestType.TaskData, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow);
    }
    public void ConnectToServer_ClickSlotsCard(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,int _SlotsIndex)
    {
        ConnectToServer(Server_RequestType.ClickSlotsCard, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, _SlotsIndex.ToString());
    }
    public void ConnectToServer_GetSlotsReward(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,Reward _SlotsRewardType,int _SlotsRewardNum)
    {
        int typeIndex;
        if (_SlotsRewardType == Reward.Gold)
            typeIndex = 0;
        else if (_SlotsRewardType == Reward.Ticket)
            typeIndex = 1;
        else if (_SlotsRewardType == Reward.Cash)
            typeIndex = 2;
        else
        {
            Debug.LogError("老虎机奖励类型错误");
            return;
        }
        ConnectToServer(Server_RequestType.GetSlotsReward, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, typeIndex.ToString(), _SlotsRewardNum.ToString());
    }
    public void ConnectToServer_FinishTask(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,int _TaskID,bool _Double,params Reward[] opTypes)
    {
        int argsLength = 2 + opTypes.Length;
        string[] args = new string[argsLength];
        args[0] = _TaskID.ToString();
        args[1] = _Double ? "2" : "1";
        for(int i = 2; i < argsLength; i++)
        {
            args[i] = opTypes[i - 2].ToString();
        }
        ConnectToServer(Server_RequestType.FinishTask, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, args);
    }
    public void ConnectToServer_BuyTickets(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,bool _IsRv)
    {
        ConnectToServer(Server_RequestType.BuyTickets, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, _IsRv ? "1" : "0");
    }
    public void ConnectToServer_WatchRvEvent(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow)
    {
        ConnectToServer(Server_RequestType.WatchRvEvent, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow);
    }
    public void ConnectToServer_BindPaypal(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,string _Paypal)
    {
        ConnectToServer(Server_RequestType.BindPaypal, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, _Paypal);
    }
    public void ConnectToServer_Cashout(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,CashoutType _CashoutNeedType,int _CashoutNeedTypeNum,int _ExchangeCashNum)
    {
        ConnectToServer(Server_RequestType.Cashout, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, ((int)_CashoutNeedType).ToString(), _CashoutNeedTypeNum.ToString(), _ExchangeCashNum.ToString());
    }
    public void ConnectToServer_OpenBettingPrize(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow)
    {
        ConnectToServer(Server_RequestType.OpenBettingPrize, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow);
    }
    public void ConnectToServer_ChangeHedOrName(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,int _NewHeadID,string _NewName)
    {
        ConnectToServer(Server_RequestType.ChangeHead_Name, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, _NewHeadID >= 0 ? _NewHeadID.ToString() : null, _NewName);
    }
    public void ConnectToServer_GetLevelupReward(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,int _RewardMultiple)
    {
        ConnectToServer(Server_RequestType.GetLevelUpReward, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, _RewardMultiple.ToString());
    }
    public void ConnectToServer_GetNewPlayerReward(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow,int _RewardMultiple=1)
    {
        ConnectToServer(Server_RequestType.GetNewPlayerReward, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow, _RewardMultiple.ToString());
    }
    public void ConnectToServer_GetCashoutRecordList(Action _ServerResponseOkCallback, Action _ServerResponseErrorCallback, Action _NetworkErrorCallback, bool _ShowConnectingWindow)
    {
        ConnectToServer(Server_RequestType.GetCashoutRecordList, _ServerResponseOkCallback, _ServerResponseErrorCallback, _NetworkErrorCallback, _ShowConnectingWindow);
    }
    private void ShowConnectErrorTip(string errorCode)
    {
        string errorString;
        switch (errorCode)
        {
            case "-2":
                errorString = "Coin reaches the limit. Come back tomorrow.";
                break;
            case "-3":
                errorString = "The reward has been claimed.";
                break;
            case "-6":
                errorString = "Abnormal behavior, try again later.";
                break;
            case "-7":
                errorString = "Found historical records, data is being updated.";
                break;
            default:
                errorString = "Error code :" + errorCode;
                break;
        }
        Master.Instance.ShowTip(errorString, 3);
    }
    #region connecting state
    const string errorTitle = "ERROR";
    const string connectingTilte = "";
    const string errorString = "Network connection is\n unavailable, please check network \n settings.";
    const string connectingString = "Connecting...";
    public void OnConnectServerFail()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        titleText.text = errorTitle;
        tipText.text = errorString;
        state_iconImage.transform.rotation = Quaternion.identity;
        state_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Server, "nonet");
        retryButton.gameObject.SetActive(true);
        StopCoroutine("WaitConnecting");
    }
    public void OnConnectServerSuccess()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        StopCoroutine("WaitConnecting");
    }
    public void OnConnectingServer()
    {
        isConnecting = true;
        StartCoroutine("WaitConnecting");
    }
    #endregion
}
public enum PlayerTaskTarget
{
    EnterSlotsOnce,//
    PlayBettingOnce,
    WatchRvOnce,//
    CashoutOnce,
    WritePaypalEmail,//
    OwnSomeGold,//
    WinnerOnce,//
    InviteAFriend,
    GetTicketFromSlotsOnce,//
    BuyTicketByGoldOnce,
    BuyTicketByRvOnce,
}
public class PlayerGetSlotsRewardReceiveData
{
    public int user_gold;
    public int user_gold_live;
    public int user_tickets;
    public int user_doller;
    public int user_doller_live;
}
public class PlayerFinishTaskReceiveData
{
    public int user_tickets;
    public int user_gold;
    public int user_gold_live;
    public int user_doller;
    public int user_doller_live;
}
public class PlayerBuyTicketReceiveData
{
    public int user_tickets;
    public int user_gold;
    public int user_gold_live;
}
public class PlayerBindPaypalReceiveData
{
    public string user_paypal;
}
public class PlayerCashoutReceiveData
{
    public int user_gold_live;
    public double user_coin_live;
    public int user_doller_live;
}
public class LocalCountryData
{
    public string ip;
    public string country;
}
public class GetLevelupRewardReceiveData
{
    public int user_tickets;
    public int user_level;
    public int user_double;
    public int next_double;
    public int level_exp;
    public int user_exp;
    public List<int> title_list;
    public int next_level;
}
public class GetNewPlayerRewardReceiveData
{
    public int user_doller_live;
}
#region newAllData
public class AllData
{
    public AllData_MainData user_panel;
    public AllData_SlotsState lucky_status;
    public AllData_BettingWinnerData award_ranking;
    public AllData_RefreshLeftTimeData get_time;
    public AllData_YesterdayRankData lucky_ranking;
    public AllData_FriendData fission_info;
    public AllData_TaskData lucky_schedule;
    public AllData_CashoutRecordData lucky_record;
    public bool day_flag;//今天是否已经开奖
    public string user_uuid;//用户uuid
}
public class AllData_MainData
{
    public string user_id;
    public int user_gold;
    public int user_gold_live;
    public int user_doller;
    public int user_doller_live;
    public string user_paypal;
    public int user_tickets;
    public int user_title;
    public int cur_betting;
    public string user_name;
    public int user_level;
    public int user_double;//当前票倍数
    public int next_double;//下一级票倍数
    public int level_exp;
    public int user_exp;
    public List<int> title_list;
    public int next_level;//升级奖励票数
    public List<int> title_level;
    public bool new_reward;//新手奖励
    public Reward level_type;//升级奖励类型
    public Reward new_data_type;//新手奖励类型
    public int new_data_num;//新手奖励数量
    public int lucky_count;//进入老虎机总次数
    public int lucky_total_cash;//老虎机获得的总共现金，不大于200
}
public class AllData_SlotsState
{
    public List<int> white_lucky;
    public int lucky_exp;
}
public class AllData_BettingWinnerData
{
    public List<AllData_BettingWinnerData_Winner> ranking;
    public int ysterday_tickets;//昨日总票数
    public int ticktes_flag;//参与抽奖所需票数
}
public class AllData_BettingWinnerData_Winner
{
    public int user_title;
    public string user_id;
    public int user_num;
}
public class AllData_RefreshLeftTimeData
{
    public int server_time;
}
public class AllData_YesterdayRankData
{
    public List<AllData_YesterdayRankData_Rank> gold_rank;
    public AllData_YesterdayRankData_Rank self_gold_info;
    public List<AllData_YesterdayRankData_Rank> tickets_rank;
}
public class AllData_YesterdayRankData_Rank
{
    public int user_title;
    public string user_id;
    public int user_token;
    public int user_num;
}
public class AllData_FriendData
{
    public AllData_FriendData_InviteRewardConfig reward_conf;
    public double user_doller;
    public string user_id;
    public int user_invite_people;
    public bool up_user;
    public double user_total;
    public string up_user_id;
    public double live_balance;
    public AllData_FriendData_FriendList up_user_info;
}
public class AllData_FriendData_InviteRewardConfig
{
    public int invite_receive;//邀请奖励领取的次数
    public int invite_flag;//邀请奖励分界人数, <=为小于部分，>为大于部分
    public Reward lt_flag_type;//小于部分
    public int lt_flag_num;
    public Reward gt_flag_type;//大于部分
    public int gt_flag_num;
}
public class AllData_FriendData_FriendList
{
    public double yestday_team_all;
    public List<AllData_FriendData_Friend> two_user_list;
}
public class AllData_FriendData_Friend
{
    public int user_img;
    public double yestday_doller;
    public int distance;//1直接好友，0间接好友
    public string user_name;
    public int user_level;
    public string user_time;
}
public class AllData_TaskData
{
    public int coin_ticket;//购买票所需金币
    public List<AllData_Task> user_task;
}
public class AllData_Task
{
    public string task_title;
    public int task_id;
    public PlayerTaskTarget taskTargetId;
    public string task_describe;
    public int task_type;
    public Reward reward_type;
    public int task_reward;
    public bool task_receive;
    public bool task_complete;
    public int task_cur;
    public int task_tar;
}
public class AllData_CashoutRecordData
{
    public List<AllData_CashoutRecordData_Record> record;
}
public class AllData_CashoutRecordData_Record
{
    public string apply_time;
    public int apply_doller;
    public CashoutType apply_type;
    public int apply_num;
    public int apply_status;
}
#endregion
