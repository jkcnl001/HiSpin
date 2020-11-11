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
            allRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
            allRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            allRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }
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
        if (Save.data.allData.user_panel.user_tickets >= Save.data.allData.award_ranking.ticktes_flag)
            tipText.text = "More tickets, more\nchance to win! ";
        else
            tipText.text = string.Format("You must have <color=#F4D10F>{0}</color> tickets\nto participate in the Lucky Draw.", Save.data.allData.award_ranking.ticktes_flag);

        ticket_numText.text = Save.data.allData.user_panel.user_tickets >= Save.data.allData.award_ranking.ticktes_flag ? string.Format("You have <color=#FF8A01>{0}</color> tickets", Save.data.allData.user_panel.user_tickets) : "not enough tickets";
        RefreshBettingWinner();
        UpdateTimeDownText(Master.time);
    }
    public void UpdateTimeDownText(string time)
    {
        time_downText.text = time;
    }
    public void RefreshBettingWinner()
    {
        foreach (var winner in all_winner_items)
            winner.gameObject.SetActive(false);

        List<AllData_BettingWinnerData_Winner> winnerDatas = Save.data.allData.award_ranking.ranking;
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
                AllData_BettingWinnerData_Winner winnerInfo = winnerDatas[i];
                all_winner_items[i].gameObject.SetActive(true);
                all_winner_items[i].Init(winnerInfo.user_title, winnerInfo.user_id, winnerInfo.user_num);
            }
        }
        StartCoroutine("DelayRefreshLayout");
    }
    private IEnumerator DelayRefreshLayout()
    {
        all_root.enabled = false;
        yield return new WaitForEndOfFrame();
        all_root.enabled = true;
    }
}
