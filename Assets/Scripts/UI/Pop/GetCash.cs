using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCash : PopUI
{
    public Button SaveInButton;
    public Text cash_numText;
    protected override void Awake()
    {
        base.Awake();
        SaveInButton.AddClickEvent(OnSaveButtonClick);
    }
    private void OnSaveButtonClick()
    {
        switch (getCashArea)
        {
            case GetCashArea.NewPlayerReward:
                //Server.Instance.OperationData_GetNewPlayerReawrd(OnGetNewplayerRewardCallback, null);
                Server_New.Instance.ConnectToServer_GetNewPlayerReward(OnGetNewplayerRewardCallback, null, null, true);
                break;
            case GetCashArea.PlaySlots:
                //Server.Instance.OperationData_GetSlotsReward(OnGetSlotsRewardCallback, null, Reward.Cash, getcashNum);
                Server_New.Instance.ConnectToServer_GetSlotsReward(OnGetSlotsRewardCallback, null, null, true, Reward.Cash, getcashNum);
                break;
        }
    }
    private void OnGetSlotsRewardCallback()
    {
        Save.data.allData.user_panel.lucky_total_cash += getcashNum;
        UI.FlyReward(Reward.Cash, getcashNum, SaveInButton.transform.position);
        UI.ClosePopPanel(this);
    }
    private void OnGetNewplayerRewardCallback()
    {
        UI.FlyReward(Reward.Cash, getcashNum, SaveInButton.transform.position);
        UI.ClosePopPanel(this);
        if (Save.data.isPackB)
            UI.ShowPopPanel(PopPanel.Guide);
    }
    GetCashArea getCashArea;
    int getcashNum;
    protected override void BeforeShowAnimation(params int[] args)
    {
        getCashArea = (GetCashArea)args[0];
        getcashNum = args[1];
        cash_numText.text = "$" + getcashNum.GetCashShowString();
    }
    protected override void AfterShowAnimation(params int[] args)
    {
        Master.Instance.ShowEffect(Reward.Cash);
    }
}
public enum GetCashArea
{
    NewPlayerReward,
    PlaySlots
}
