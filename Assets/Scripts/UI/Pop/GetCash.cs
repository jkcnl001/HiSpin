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
                Server.Instance.OperationData_GetNewPlayerReawrd(OnGetNewplayerRewardCallback, null);
                break;
            case GetCashArea.PlaySlots:
                Server.Instance.OperationData_GetSlotsReward(OnGetSlotsRewardCallback, null, Reward.Cash, getcashNum);
                break;
        }
    }
    private void OnGetSlotsRewardCallback()
    {
        UI.ClosePopPanel(this);
    }
    private void OnGetNewplayerRewardCallback()
    {
        UI.ClosePopPanel(this);
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
}
public enum GetCashArea
{
    NewPlayerReward,
    PlaySlots
}
