using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rank : BaseUI
{
    public Button coin_rankButton;
    public Button ticket_rankButton;
    private Text coin_rankText;
    private Text ticket_rankText;
    [Space(10)]
    public RectTransform top_lineRect;
    public RectTransform coin_rank_topRect;
    public RectTransform ticket_rankRect;
    [Space(10)]
    public CanvasGroup coin_rankGroup;
    public CanvasGroup ticket_rankGroup;
    public RankItem self_coin_rank;

    public RankItem single_coin_rank;
    public RankItem single_ticket_rank;
    private List<RankItem> all_coin_ranks = new List<RankItem>();
    private List<RankItem> all_ticket_ranks = new List<RankItem>();
    protected override void Awake()
    {
        base.Awake();
        coin_rankText = coin_rankButton.GetComponent<Text>();
        ticket_rankText = ticket_rankButton.GetComponent<Text>();
        coin_rankButton.AddClickEvent(OnCoinRankClick);
        ticket_rankButton.AddClickEvent(OnTicketRnakClick);
        all_coin_ranks.Add(single_coin_rank);
        all_ticket_ranks.Add(single_ticket_rank);
        if (Master.IsBigScreen)
        {
            coin_rankButton.transform.localPosition -= new Vector3(0, Master.TopMoveDownOffset);
            ticket_rankButton.transform.localPosition -= new Vector3(0, Master.TopMoveDownOffset);
            top_lineRect.transform.localPosition -= new Vector3(0, Master.TopMoveDownOffset);

            coin_rank_topRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset);
            coin_rank_topRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            coin_rank_topRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
            ticket_rankRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset);
            ticket_rankRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            ticket_rankRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }

        OnCoinRankClick();
    }
    Color rankTextOffColor = new Color32(49, 49, 49, 255);
    Color rankTextOnColor = Color.white;
    private void OnCoinRankClick()
    {
        coin_rankText.color = rankTextOnColor;
        ticket_rankText.color = rankTextOffColor;
        coin_rankGroup.alpha = 1;
        coin_rankGroup.blocksRaycasts = true;
        ticket_rankGroup.alpha = 0;
        ticket_rankGroup.blocksRaycasts = false;
    }
    private void OnTicketRnakClick()
    {
        coin_rankText.color = rankTextOffColor;
        ticket_rankText.color = rankTextOnColor;
        coin_rankGroup.alpha = 0;
        coin_rankGroup.blocksRaycasts = false;
        ticket_rankGroup.alpha = 1;
        ticket_rankGroup.blocksRaycasts = true;
    }
    private void InitCoinRanks()
    {
        foreach (var rank in all_coin_ranks)
            rank.gameObject.SetActive(false);

        List<AllData_YesterdayRankData_Rank> allGoldRankInfo = Save.data.allData.lucky_ranking.gold_rank;
        int allRankCount = allGoldRankInfo.Count;
        int rankIndex = 0;
        for(int i = 0; i < allRankCount; i++)
        {
            if (rankIndex > all_coin_ranks.Count - 1)
            {
                RankItem newRankItem = Instantiate(single_coin_rank, single_coin_rank.transform.parent).GetComponent<RankItem>();
                all_coin_ranks.Add(newRankItem);
            }
            AllData_YesterdayRankData_Rank rankInfo = allGoldRankInfo[i];
            all_coin_ranks[i].gameObject.SetActive(true);
            all_coin_ranks[i].Init(rankInfo.user_title, rankInfo.user_id, rankInfo.user_num, rankInfo.user_token);
            rankIndex++;
        }
        AllData_YesterdayRankData_Rank selfRankInfo = Save.data.allData.lucky_ranking.self_gold_info;
        self_coin_rank.Init(selfRankInfo.user_title, selfRankInfo.user_id, selfRankInfo.user_num, selfRankInfo.user_token);
    }
    private void InitTicketRanks()
    {
        foreach (var rank in all_ticket_ranks)
            rank.gameObject.SetActive(false);

        List<AllData_YesterdayRankData_Rank> allTicketRankInfo = Save.data.allData.lucky_ranking.tickets_rank;
        int allRankCount = allTicketRankInfo.Count;
        int rankIndex = 0;
        for(int i = 0; i < allRankCount; i++)
        {
            if (rankIndex > all_ticket_ranks.Count - 1)
            {
                RankItem newRankItem = Instantiate(single_ticket_rank, single_ticket_rank.transform.parent).GetComponent<RankItem>();
                all_ticket_ranks.Add(newRankItem);
            }
            AllData_YesterdayRankData_Rank rankInfo = allTicketRankInfo[i];
            all_ticket_ranks[i].gameObject.SetActive(true);
            all_ticket_ranks[i].Init(rankInfo.user_title, rankInfo.user_id, rankInfo.user_num, rankInfo.user_token);
            rankIndex++;
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        RefreshRankList();
    }
    public void RefreshRankList()
    {
        InitCoinRanks();
        InitTicketRanks();
    }
}
