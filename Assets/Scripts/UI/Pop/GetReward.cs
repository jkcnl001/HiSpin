using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetReward : PopUI
{
    public Image reward_iconImage;
    public Text tipText;
    public Text reward_numText;
    public Text double_getText;
    public Button double_getButton;
    public Button nothanksButton;
    protected override void Awake()
    {
        base.Awake();
        double_getButton.AddClickEvent(OnDoublegetButtonClick);
        nothanksButton.AddClickEvent(OnNothanksButtonClick);
    }
    int clickAdTime = 0;
    private void OnDoublegetButtonClick()
    {
        clickAdTime++;
        Ads._instance.ShowRewardVideo(OnAdCallback, clickAdTime, "多倍奖励" + reward_type, OnNothanksButtonClick);
    }
    private void OnAdCallback()
    {
        reward_num *= reward_type == Reward.Ticket ? 3 : 2;
        Get();
    }
    private void OnNothanksButtonClick()
    {
        Ads._instance.ShowInterstialAd(Get, "放弃多倍" + reward_type);
    }
    private void Get()
    {
        switch (reward_area)
        {
            case GetRewardArea.PlaySlots:
                Server.Instance.OperationData_GetSlotsReward(OnRequestCallback, null, reward_type, reward_num);
                break;
            default:
                Debug.LogError("奖励获得区域错误");
                break;
        }
    }
    private void OnRequestCallback()
    {
        UI.FlyReward(reward_type, reward_num, double_getButton.transform.position);
        TaskAgent.TriggerTaskEvent(PlayerTaskTarget.PlaySlotsOnce, 1);
        if (reward_type == Reward.Gold)
        {
            TaskAgent.TriggerTaskEvent(PlayerTaskTarget.OwnSomeGold, reward_num);
        }
        else if (reward_type == Reward.Ticket)
        {
            TaskAgent.TriggerTaskEvent(PlayerTaskTarget.GetTicketFromSlotsOnce, 1);
        }
        UI.ClosePopPanel(this);
    }
    const string GoldTip = "OH YEEAAAAH!\nTOKEN HAUL!";
    const string TicketTip = "THAT'S EPIC!\nYOU WON TICKET!";
    Reward reward_type = Reward.Null;
    GetRewardArea reward_area = GetRewardArea.Null;
    int reward_num = 0;
    protected override void BeforeShowAnimation(params int[] args)
    {
        clickAdTime = 0;
        reward_type = (Reward)args[0];
        reward_num = args[1];
        reward_area = (GetRewardArea)args[2];
        reward_numText.text = reward_num.ToString();
        switch (reward_type)
        {
            case Reward.Gold:
                reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "gold");
                tipText.text = GoldTip;
                double_getText.text = "GET   x2";
                break;
            case Reward.Ticket:
                reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "ticket");
                tipText.text = TicketTip;
                double_getText.text = "GET   x3";
                break;
            case Reward.Cash:
                reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetReward, "ticket");
                tipText.text = TicketTip;
                double_getText.text = "GET   x3";
                break;
            default:
                Debug.LogError("奖励类型错误");
                break;
        }
    }
}
public enum GetRewardArea
{
    Null,
    PlaySlots,
}
