using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slots : BaseUI
{
    public List<SlotItem> allSlotsItems = new List<SlotItem>();
    public Button cash_outButton;
    public Text time_downText;
    bool isFirstEnter = true;
    protected override void Awake()
    {
        base.Awake();
        cash_outButton.AddClickEvent(OnCashoutButtonClick);
        Server.Instance.RequestData(Server.Server_RequestType.SlotsStateData, OnRequestDataCallback, null);
        RectTransform allRect = transform.GetChild(0) as RectTransform;
        if (Master.IsBigScreen)
        {
            allRect.localPosition -= new Vector3(0, 100, 0);
            allRect.sizeDelta += new Vector2(0, Screen.height - 1920-200);
        }
        allRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
    }
    private void OnCashoutButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    private void OnRequestDataCallback()
    {
        int slotsCount = allSlotsItems.Count;
        int netCount = Save.data.slots_state.white_lucky.Count;
        if (slotsCount != netCount)
        {
            Debug.LogError("老虎机数量匹配错误");
            return;
        }
        for (int i = 0; i < slotsCount; i++)
        {
            int index = i;
            bool isFree = Save.data.slots_state.white_lucky[i] == 0;
            allSlotsItems[i].Init(isFree, index);
        }
        if (Save.data.enter_slots_time >= 3&&!Save.data.hasRateus)
        {
            Save.data.hasRateus = true;
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Rateus);
        }
        Server.Instance.RequestData_GetBettingLeftTime(OnGetRefreshLeftTimeCallback, null);
    }
    private void OnGetRefreshLeftTimeCallback()
    {
        StopCoroutine("AutoTimedown");
        StartCoroutine("AutoTimedown", Save.data.betting_lefttime.server_time + 1);
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        canRefreshSlotsCard = true;
        if(leftTime<=0)
            Server.Instance.RequestData(Server.Server_RequestType.SlotsStateData, OnRequestDataCallback, null);
        if (isFirstEnter)
            isFirstEnter = false;
        else
        {
            OnRequestDataCallback();
        }
    }
    protected override void BeforeCloseAnimation()
    {
        canRefreshSlotsCard = false;
    }
    public override void Pause()
    {
        canRefreshSlotsCard = false;
    }
    public override void Resume()
    {
        canRefreshSlotsCard = true;
        if (leftTime <= 0)
            Server.Instance.RequestData(Server.Server_RequestType.SlotsStateData, OnRequestDataCallback, null);
    }
    int leftTime = 9999999;
    bool canRefreshSlotsCard = false;
    private IEnumerator AutoTimedown(int leftSeconds)
    {
        leftTime = leftSeconds;
        WaitForSeconds oneSecond = new WaitForSeconds(1);
        while (true)
        {
            int second = leftSeconds % 60;
            int minute = leftSeconds % 3600 / 60;
            int hour = leftSeconds / 3600;
            time_downText.text = "NEW SLOTS IN:\n"+(hour < 10 ? "0" + hour : hour.ToString()) + ":" + (minute < 10 ? "0" + minute : minute.ToString()) + ":" + (second < 10 ? "0" + second : second.ToString());
            yield return oneSecond;
            leftSeconds--;
            leftTime = leftSeconds;
            if (leftSeconds == 0)
            {
                if (canRefreshSlotsCard)
                    Server.Instance.RequestData(Server.Server_RequestType.SlotsStateData, OnRequestDataCallback, null);
                yield break;
            }
        }
    }
}
