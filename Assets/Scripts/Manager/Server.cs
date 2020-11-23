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

public class Server : MonoBehaviour
{
    public const string Bi_name = Ads.AppName;
    public static Server Instance;
    public CanvasGroup canvasGroup;
    public Image state_iconImage;
    public Text titleText;
    public Text tipText;
    public Button retryButton;
    static string deviceID;
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
        switch (requestType)
        {
            case Server_RequestType.AllData:
                StartCoroutine(ConnectToGetData(requestType, requestOkCallback, requestNoCallback, needShowConnecting));
                break;
            case Server_RequestType.TaskData:
                StartCoroutine(ConnectToGetData(Server_RequestType.TaskData, requestOkCallback, requestNoCallback, needShowConnecting));
                break;
            case Server_RequestType.ClickSlotsCard:
                StartCoroutine(ConnectToClickSlotsCard(requestOkCallback, requestNoCallback, clickSlotsIndex));
                break;
            case Server_RequestType.GetSlotsReward:
                StartCoroutine(ConnectToGetSlotsReward(requestOkCallback, requestNoCallback, getSlotsRewardTypeIndex, getSlotsRewardNum));
                break;
            case Server_RequestType.FinishTask:
                StartCoroutine(ConnectToFinishTask(requestOkCallback, requestNoCallback, finishTaskId, finishTaskDoubleReward, finishTaskOptypes));
                break;
            case Server_RequestType.BuyTickets:
                StartCoroutine(ConnectToBuyTicket(requestOkCallback, requestNoCallback, isRv));
                break;
            case Server_RequestType.WatchRvEvent:
                StartCoroutine(ConnectToSendRVEvent(requestOkCallback, requestNoCallback));
                break;
            case Server_RequestType.BindPaypal:
                StartCoroutine(ConnectToBindPaypal(requestOkCallback, requestNoCallback, paypal));
                break;
            case Server_RequestType.Cashout:
                StartCoroutine(ConnectToCashout(requestOkCallback, requestNoCallback, (int)cashoutType, cashoutTypeNum, cashoutNum));
                break;
            case Server_RequestType.GetLocalCountry:
                StartCoroutine(ConnectToGetlocalcountry(getlocalcountryOkCallback, requestNoCallback));
                break;
            case Server_RequestType.OpenBettingPrize:
                StartCoroutine(ConnectToOpenBettingPrize(requestOkCallback, requestNoCallback));
                break;
            case Server_RequestType.ChangeHead_Name:
                StartCoroutine(ConnectToChangeHeadOrName(requestOkCallback, requestNoCallback, New_head_id, New_name));
                break;
            case Server_RequestType.GetLevelUpReward:
                StartCoroutine(ConnectToGetLevelupReward(requestOkCallback, requestNoCallback, LevelupRewardMultiple));
                break;
            case Server_RequestType.GetNewPlayerReward:
                StartCoroutine(ConnectToGetNewPlayerReward(requestOkCallback, requestNoCallback, NewPlayerRewardMultiple));
                break;
            default:
                break;
        }
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
        deviceID = SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
        Application.RequestAdvertisingIdentifierAsync(
           (string advertisingId, bool trackingEnabled, string error) =>
           {
               deviceID = advertisingId;
           }
       );
#elif UNITY_IOS && !UNITY_EDITOR
         deviceID = Getidfa();
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
    };
    #region request server function
    Server_RequestType requestType;
    Action requestOkCallback;
    Action requestNoCallback;
    bool needShowConnecting;
    public void RequestData(Server_RequestType _RequestType, Action successCallback, Action failCallback, bool needShowConnecting = true)
    {
        requestType = _RequestType;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        this.needShowConnecting = needShowConnecting;
        StartCoroutine(ConnectToGetData(_RequestType, successCallback, failCallback, needShowConnecting));
    }
    int clickSlotsIndex = -1;
    public void OperationData_ClickSlotsCard(Action successCallback,Action failCallback,int slotsIndex)
    {
        clickSlotsIndex = slotsIndex;
        requestType = Server_RequestType.ClickSlotsCard;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        needShowConnecting = true;
        StartCoroutine(ConnectToClickSlotsCard(successCallback, failCallback, slotsIndex));
    }
    int getSlotsRewardTypeIndex;
    int getSlotsRewardNum;
    public void OperationData_GetSlotsReward(Action successCallback,Action failCallback,Reward type,int num)
    {
        requestType = Server_RequestType.GetSlotsReward;
        int typeIndex = 0;
        if (type == Reward.Gold)
            typeIndex = 0;
        else if (type == Reward.Ticket)
            typeIndex = 1;
        else if (type == Reward.Cash)
            typeIndex = 2;
        else
        {
            Debug.LogError("老虎机奖励类型错误");
            return;
        }
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        getSlotsRewardTypeIndex = typeIndex;
        getSlotsRewardNum = num;
        StartCoroutine(ConnectToGetSlotsReward(successCallback, failCallback, typeIndex, num));
    }
    int finishTaskId;
    Reward[] finishTaskOptypes;
    bool finishTaskDoubleReward;
    public void OperationData_FinishTask(Action successCallback, Action failCallback, int taskID, bool double_reward = false, params Reward[] opTypes)
    {
        requestType = Server_RequestType.FinishTask;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        finishTaskId = taskID;
        finishTaskOptypes = opTypes;
        finishTaskDoubleReward = double_reward;
        StartCoroutine(ConnectToFinishTask(successCallback, failCallback, taskID, double_reward, opTypes));
    }
    bool isRv;
    public void OperationData_BuyTickets(Action successCallback,Action failCallback,bool isRv)
    {
        requestType = Server_RequestType.BuyTickets;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        this.isRv = isRv;
        StartCoroutine(ConnectToBuyTicket(successCallback, failCallback, isRv));
    }
    public void OperationData_RvEvent(Action successCallback,Action failCallback)
    {
        requestType = Server_RequestType.WatchRvEvent;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        StartCoroutine(ConnectToSendRVEvent(successCallback, failCallback));
    }
    string paypal;
    public void OperationData_BindPaypal(Action successCallback, Action failCallback,string paypal)
    {
        requestType = Server_RequestType.BindPaypal;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        this.paypal = paypal;
        StartCoroutine(ConnectToBindPaypal(successCallback, failCallback, paypal));
    }
    CashoutType cashoutType;
    int cashoutTypeNum;
    int cashoutNum;
    Action<string> getlocalcountryOkCallback;
    public void OperationData_Cashout(Action successCallback, Action failCallback,CashoutType type,int typeNum,int cashNum)
    {
        requestType = Server_RequestType.Cashout;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        cashoutType = type;
        cashoutTypeNum = typeNum;
        cashoutNum = cashNum;
        StartCoroutine(ConnectToCashout(successCallback, failCallback, (int)type, typeNum, cashNum));
    }
    public void RequestData_GetLocalcountry(Action<string> successCallback, Action failCallback)
    {
        requestType = Server_RequestType.GetLocalCountry;
        getlocalcountryOkCallback = successCallback;
        requestNoCallback = failCallback;
        StartCoroutine(ConnectToGetlocalcountry(successCallback, failCallback));
    }
    public void OperationData_OpenBettingPrize(Action successCallback, Action failCallback)
    {
        requestType = Server_RequestType.OpenBettingPrize;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        StartCoroutine(ConnectToOpenBettingPrize(successCallback, failCallback));
    }
    int New_head_id;
    string New_name;
    public void OperationData_ChangeHead_Name(Action successCallback, Action failCallback,int new_head_id,string new_name)
    {
        requestType = Server_RequestType.ChangeHead_Name;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        New_head_id = new_head_id;
        New_name = new_name;
        StartCoroutine(ConnectToChangeHeadOrName(successCallback, failCallback, new_head_id, new_name));
    }
    int LevelupRewardMultiple = 1;
    public void OperationData_GetLevelupReward(Action successCallback, Action failCallback,int multiple)
    {
        requestType = Server_RequestType.GetLevelUpReward;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        LevelupRewardMultiple = multiple;
        StartCoroutine(ConnectToGetLevelupReward(successCallback, failCallback, multiple));
    }
    int NewPlayerRewardMultiple = 1;
    public void OperationData_GetNewPlayerReawrd(Action successCallback, Action failCallback)
    {
        requestType = Server_RequestType.GetNewPlayerReward;
        requestOkCallback = successCallback;
        requestNoCallback = failCallback;
        NewPlayerRewardMultiple = 1;
        StartCoroutine(ConnectToGetNewPlayerReward(successCallback, failCallback, NewPlayerRewardMultiple));
    }
    #endregion
    #region IEnumerator connecting server
    IEnumerator ConnectToGetData(Server_RequestType _RequestType, Action successCallback, Action failCallback, bool needShowConnecting)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("app_name", Bi_name));
        if (needShowConnecting)
            OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[_RequestType], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                switch (_RequestType)
                {
                    case Server_RequestType.AllData:
                        try
                        {
                            AllData allData = JsonMapper.ToObject<AllData>(downText);
                            Save.data.allData = allData;
                            Ads._instance.InitFyber(allData.user_panel.user_id, "");
                        }
                        catch(Exception e)
                        {
                            failCallback?.Invoke();
                            ShowConnectErrorTip(e.Message);
                            yield break;
                        }
                        break;
                    case Server_RequestType.TaskData:
                        AllData_TaskData taskData = JsonMapper.ToObject<AllData_TaskData>(downText);
                        Save.data.allData.lucky_schedule = taskData;
                        break;
                    default:
                        break;
                }
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToClickSlotsCard(Action successCallback, Action failCallback,int slotsIndex)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("lucky_id", slotsIndex.ToString()));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.ClickSlotsCard], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                AllData_SlotsState slotsStateData = JsonMapper.ToObject<AllData_SlotsState>(www.downloadHandler.text);
                Save.data.allData.lucky_status = slotsStateData;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToGetSlotsReward(Action successCallback ,Action  failCallback,int reward_type,int reward_num)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("reward_type", reward_type.ToString()));
        iparams.Add(new MultipartFormDataSection("reward_num", reward_num.ToString()));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.GetSlotsReward], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                PlayerGetSlotsRewardReceiveData receiveData = JsonMapper.ToObject<PlayerGetSlotsRewardReceiveData>(www.downloadHandler.text);
                if (reward_type == 0)
                {
                    Save.data.allData.user_panel.user_gold = receiveData.user_gold;
                    Save.data.allData.user_panel.user_gold_live = receiveData.user_gold_live;
                }
                else if (reward_type == 2)
                {
                    Save.data.allData.user_panel.user_doller = receiveData.user_doller;
                    Save.data.allData.user_panel.user_doller_live = receiveData.user_doller_live;
                }
                else
                    Save.data.allData.user_panel.user_tickets = receiveData.user_tickets;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToFinishTask(Action successCallback,Action failCallback,int taskID, bool double_reward,params Reward[] opTypes)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("task_id", taskID.ToString()));
        iparams.Add(new MultipartFormDataSection("double", double_reward ? "2" : "1"));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.FinishTask], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                PlayerFinishTaskReceiveData receiveData = JsonMapper.ToObject<PlayerFinishTaskReceiveData>(downText);
                int length = opTypes.Length;
                for(int i = 0; i < length; i++)
                {
                    switch (opTypes[i])
                    {
                        case Reward.Gold:
                            Save.data.allData.user_panel.user_gold = receiveData.user_gold;
                            Save.data.allData.user_panel.user_gold_live = receiveData.user_gold_live;
                            break;
                        case Reward.Cash:
                            Save.data.allData.user_panel.user_doller = receiveData.user_doller;
                            Save.data.allData.user_panel.user_doller_live = receiveData.user_doller_live;
                            break;
                        case Reward.Ticket:
                            Save.data.allData.user_panel.user_tickets = receiveData.user_tickets;
                            break;
                        default:
                            Debug.LogError("错误的任务完成变动类型");
                            break;
                    }
                }
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToBuyTicket(Action successCallback,Action failCallback,bool isRv)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("reward_type", isRv ? "1" : "0"));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.BuyTickets], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                PlayerBuyTicketReceiveData receiveData = JsonMapper.ToObject<PlayerBuyTicketReceiveData>(downText);

                if (isRv)
                {
                    Save.data.allData.user_panel.user_tickets = receiveData.user_tickets;
                }
                else
                {
                    Save.data.allData.user_panel.user_gold = receiveData.user_gold;
                    Save.data.allData.user_panel.user_gold_live = receiveData.user_gold_live;
                    Save.data.allData.user_panel.user_tickets = receiveData.user_tickets;
                }
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToSendRVEvent(Action successCallback,Action failCallback)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.WatchRvEvent], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText != "ok")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToBindPaypal(Action successCallback,Action failCallback,string paypal)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("paypal", paypal));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.BindPaypal], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                PlayerBindPaypalReceiveData paypalData = JsonMapper.ToObject<PlayerBindPaypalReceiveData>(www.downloadHandler.text);
                Save.data.allData.user_panel.user_paypal = paypalData.user_paypal;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToCashout(Action successCallback, Action failCallback,int type,int num,int cash)
    {
        isConnecting = true;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(deviceID));
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("app_name", Bi_name));
        iparams.Add(new MultipartFormDataSection("withdrawal_type", type.ToString()));
        iparams.Add(new MultipartFormDataSection("withdrawal", num.ToString()));
        iparams.Add(new MultipartFormDataSection("doller", cash.ToString()));
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.Cashout], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            failCallback?.Invoke();
            OnConnectServerFail();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                PlayerCashoutReceiveData receiveData = JsonMapper.ToObject<PlayerCashoutReceiveData>(www.downloadHandler.text);
                CashoutType cashoutType = (CashoutType)type;
                switch (cashoutType)
                {
                    case CashoutType.PT:
                        break;
                    case CashoutType.Cash:
                        Save.data.allData.user_panel.user_doller_live = receiveData.user_doller_live;
                        break;
                    case CashoutType.Gold:
                        Save.data.allData.user_panel.user_gold_live = receiveData.user_gold_live;
                        break;
                }
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    private string localCountry = "";
    IEnumerator ConnectToGetlocalcountry(Action<string> successCallback, Action failCallback)
    {
        if (!string.IsNullOrEmpty(localCountry))
        {
            successCallback?.Invoke(localCountry);
            yield break;
        }
        isConnecting = true;
        OnConnectingServer();
        UnityWebRequest www = UnityWebRequest.Get(getdata_uri_dic[Server_RequestType.GetLocalCountry]);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            string downText = www.downloadHandler.text;
            LocalCountryData countryData = JsonMapper.ToObject<LocalCountryData>(downText);
            localCountry = countryData.country.ToLower();
            OnConnectServerSuccess();
            successCallback?.Invoke(localCountry);
        }
        www.Dispose();
    }
    IEnumerator ConnectToOpenBettingPrize(Action successCallback, Action failCallback)
    {
        isConnecting = true;
        OnConnectingServer();
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.OpenBettingPrize], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText!="ok")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                Save.data.allData.day_flag = true;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToChangeHeadOrName(Action successCallback, Action failCallback, int head_id, string name)
    {
        isConnecting = true;
        OnConnectingServer();
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        if (head_id > 0)
            iparams.Add(new MultipartFormDataSection("title_id", head_id.ToString()));
        if(!string.IsNullOrEmpty(name))
            iparams.Add(new MultipartFormDataSection("user_name", name));
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.ChangeHead_Name], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText != "ok")
            {
                Master.Instance.ShowTip("Sorry, the name was used.", 2);
                failCallback?.Invoke();
            }
            else
            {
                if (head_id > 0)
                    Save.data.allData.user_panel.user_title = head_id;
                if (!string.IsNullOrEmpty(name))
                    Save.data.allData.user_panel.user_name = name;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToGetLevelupReward(Action successCallback, Action failCallback,int multiple)
    {
        isConnecting = true;
        OnConnectingServer();
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("double", multiple.ToString()));
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.GetLevelUpReward], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                GetLevelupRewardReceiveData receiveData = JsonMapper.ToObject<GetLevelupRewardReceiveData>(downText);
                Save.data.allData.user_panel.user_tickets = receiveData.user_tickets;
                Save.data.allData.user_panel.user_level = receiveData.user_level;
                Save.data.allData.user_panel.user_double = receiveData.user_double;
                Save.data.allData.user_panel.next_double = receiveData.next_double;
                Save.data.allData.user_panel.level_exp = receiveData.level_exp;
                Save.data.allData.user_panel.user_exp = receiveData.user_exp;
                Save.data.allData.user_panel.title_list = receiveData.title_list;
                Save.data.allData.user_panel.next_level = receiveData.next_level;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    IEnumerator ConnectToGetNewPlayerReward(Action successCallback, Action failCallback, int multiple)
    {
        isConnecting = true;
        OnConnectingServer();
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("device_id", deviceID));
        iparams.Add(new MultipartFormDataSection("double", multiple.ToString()));
        UnityWebRequest www = UnityWebRequest.Post(getdata_uri_dic[Server_RequestType.GetNewPlayerReward], iparams);
        yield return www.SendWebRequest();
        isConnecting = false;
        if (www.isNetworkError || www.isHttpError)
        {
            OnConnectServerFail();
            failCallback?.Invoke();
        }
        else
        {
            OnConnectServerSuccess();
            string downText = www.downloadHandler.text;
            if (downText == "-1" || downText == "-2" || downText == "-3" || downText == "-4" || downText == "-5" || downText == "-6")
            {
                ShowConnectErrorTip(downText);
                failCallback?.Invoke();
            }
            else
            {
                GetNewPlayerRewardReceiveData receiveData = JsonMapper.ToObject<GetNewPlayerRewardReceiveData>(downText);
                Save.data.allData.user_panel.user_doller_live = receiveData.user_doller_live;
                successCallback?.Invoke();
            }
        }
        www.Dispose();
    }
    #endregion
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
            default:
                errorString = "Error code :" + errorCode;
                break;
        }
        Master.Instance.ShowTip(errorString,3);
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
    public int user_coin_live;
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
