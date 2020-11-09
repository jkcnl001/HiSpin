using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Betting : BaseUI
{
    public ContentSizeFitter all_root;
    public Button helpButton;
    public Button get_ticketsButton;
    public Text sum_prizesText;
    public Text time_downText;
    public Text ticket_numText;
    public Text tipText;
    public const int JoinNeedTicketNum = 150;
    public BettingWinnerItem single_winner_item;
    private List<BettingWinnerItem> all_winner_items = new List<BettingWinnerItem>();
    protected override void Awake()
    {
        base.Awake();
        helpButton.AddClickEvent(OnHelpClick);
        get_ticketsButton.AddClickEvent(OnGetTicketsClick);
        all_winner_items.Add(single_winner_item);
        if (Master.IsBigScreen)
        {
            RectTransform allRect = all_root.transform.parent as RectTransform;
            allRect.localPosition -= new Vector3(0, 100, 0);
            allRect.sizeDelta += new Vector2(0, Screen.height - 1920 - 200);
            allRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }
        Server.Instance.RequestData_GetBettingLeftTime(OnRequestDataCallback, null);
    }
    private void OnHelpClick()
    {
        UI.ShowPopPanel(PopPanel.Rules,(int)RuleArea.Betting);
    }
    private void OnGetTicketsClick()
    {
        UI.ShowBasePanel(BasePanel.Task);
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        canStarBetting = true;
        if (Save.data.mainData.user_tickets >= JoinNeedTicketNum)
            tipText.text = "More tickets, more\nchance to win! ";
        else
            tipText.text = string.Format("You need <color=#F4D10F>{0}</color> more Tickets\nto take part in this Lucky Draw", JoinNeedTicketNum);

        ticket_numText.text = string.Format("You have <color=#FF8A01>{0}</color> tickets", Save.data.mainData.user_tickets);

        if (leftTime <= 0)
        {
            OnRefreshWinnerAndStartBetting();
        }
    }
    private void OnRefreshWinnerAndStartBetting()
    {
        Server.Instance.RequestData(Server.Server_RequestType.MainData, OnGetMainDataEnd, null);
    }
    private void OnGetMainDataEnd()
    {
        Server.Instance.RequestData_GetBettingLeftTime(OnMenuRequestEnd, null);
    }
    private void OnMenuRequestEnd()
    {
        OnRequestDataCallback();
        UI.MenuPanel.OnStartNewBetting();
    }
    int leftTime = 9999999;
    private IEnumerator AutoTimedown(int leftSeconds)
    {
        leftTime = leftSeconds;
        WaitForSeconds oneSecond = new WaitForSeconds(1);
        while (true)
        {
            int second = leftSeconds % 60;
            int minute = leftSeconds % 3600 / 60;
            int hour = leftSeconds / 3600;
            time_downText.text = (hour < 10 ? "0" + hour : hour.ToString()) + ":" + (minute < 10 ? "0" + minute : minute.ToString()) + ":" + (second < 10 ? "0" + second : second.ToString());
            yield return oneSecond;
            leftSeconds--;
            leftTime = leftSeconds;
            if (leftSeconds == 0)
            {
                if(canStarBetting)
                    OnRefreshWinnerAndStartBetting();
                yield break;
            }
        }
    }
    private void OnRequestDataCallback()
    {
        foreach(var winner in all_winner_items)
            winner.gameObject.SetActive(false);

        List<BettingWinnerInfo> winnerDatas = Save.data.betting_data.ranking;
        if (winnerDatas != null)
        {
            int winnerCount = winnerDatas.Count;
            for (int i = 0; i < winnerCount; i++)
            {
                if (i > all_winner_items.Count - 1)
                {
                    BettingWinnerItem newWinnerItem = Instantiate(single_winner_item, single_winner_item.transform.parent).GetComponent<BettingWinnerItem>();
                    all_winner_items.Add(newWinnerItem);
                }
                BettingWinnerInfo winnerInfo = winnerDatas[i];
                all_winner_items[i].gameObject.SetActive(true);
                all_winner_items[i].Init(winnerInfo.user_title, winnerInfo.user_id, winnerInfo.user_num);
            }
        }
        StartCoroutine("DelayRefreshLayout");
        StartCoroutine(AutoTimedown(Save.data.betting_lefttime.server_time+60));
    }
    private IEnumerator DelayRefreshLayout()
    {
        all_root.enabled = false;
        yield return new WaitForEndOfFrame();
        all_root.enabled = true;
    }
    bool canStarBetting = false;
    public override void Pause()
    {
        canStarBetting = false;
    }
    public override void Resume()
    {
        canStarBetting = true;
        if (leftTime <= 0)
        {
            OnRefreshWinnerAndStartBetting();
        }
    }
    protected override void BeforeCloseAnimation()
    {
        canStarBetting = false;
    }
}
