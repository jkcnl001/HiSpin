using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cashout : BaseUI
{
    public RectTransform anchor_rect;
    [Space(15)]
    public Text emailText;
    public Button emailInputButton;
    public Button recordButton;
    [Space(15)]
    public Text pt_numText;
    public Text pt_cashout_numText;
    public Button pt_cashout_leftButton;
    public Button pt_cashout_midButton;
    public Button pt_cashout_rightButton;
    [Space(15)]
    public Text cash_numText;
    public Button cashout_leftButton;
    public Button cashout_midButton;
    public Button cashout_rightButton;
    [Space(15)]
    public Text gold_numText;
    public Text gold_tipText;
    [Space(15)]
    public Button about_feeButton;
    //L,M,R
    static readonly int[] Cashout_Nums = new int[3] { 10, 50, 100 };
    protected override void Awake()
    {
        base.Awake();
        recordButton.AddClickEvent(OnRecordButtonClick);
        emailInputButton.AddClickEvent(OnEmailInput);
        about_feeButton.AddClickEvent(OnAboutFeeClick);
        if (Master.IsBigScreen)
        {
            anchor_rect.localPosition -= new Vector3(0, 100, 0);
            anchor_rect.sizeDelta += new Vector2(0, Screen.height - 1920 - 200);
            anchor_rect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }

        pt_cashout_leftButton.GetComponentInChildren<Text>().text = "$ " + Cashout_Nums[0];
        pt_cashout_midButton.GetComponentInChildren<Text>().text = "$ " + Cashout_Nums[1];
        pt_cashout_rightButton.GetComponentInChildren<Text>().text = "$ " + Cashout_Nums[2];
        cashout_leftButton.GetComponentInChildren<Text>().text = "$ " + Cashout_Nums[0];
        cashout_midButton.GetComponentInChildren<Text>().text = "$ " + Cashout_Nums[1];
        cashout_rightButton.GetComponentInChildren<Text>().text = "$ " + Cashout_Nums[2];

        pt_cashout_leftButton.AddClickEvent(() => { OnPtCashoutButtonClick(Cashout_Nums[0]); });
        pt_cashout_midButton.AddClickEvent(() => { OnPtCashoutButtonClick(Cashout_Nums[1]); });
        pt_cashout_rightButton.AddClickEvent(() => { OnPtCashoutButtonClick(Cashout_Nums[2]); });
        cashout_leftButton.AddClickEvent(() => { OnCashoutButtonClick(Cashout_Nums[0]); });
        cashout_midButton.AddClickEvent(() => { OnCashoutButtonClick(Cashout_Nums[1]); });
        cashout_rightButton.AddClickEvent(() => { OnCashoutButtonClick(Cashout_Nums[2]); });
    }
    private void OnRecordButtonClick()
    {
        UI.ShowBasePanel(BasePanel.CashoutRecord);
    }
    private void OnEmailInput()
    {
        UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.PaypalEmail);
    }
    private void OnPtCashoutButtonClick(int cashoutNum)
    {
        if (0 >= cashoutNum * PtCashoutRate * 100)
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.PT, cashoutNum * PtCashoutRate * 100);
        else
            Master.Instance.ShowTip("Sorry, your balance is not enough.");
    }
    private void OnCashoutButtonClick(int cashoutNum)
    {
        if (Save.data.mainData.user_doller_live >= cashoutNum * 100)
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Cashout, cashoutNum, (int)CashoutType.Cash, cashoutNum * 100);
        else
            Master.Instance.ShowTip("Sorry, your balance is not enough.");
    }
    private void OnAboutFeeClick()
    {
        Server.Instance.RequestData_GetLocalcountry(OnRequestLocalcountyCallback, null);
    }
    private void OnRequestLocalcountyCallback(string country)
    {
        Application.OpenURL(string.Format("https://www.paypal.com/{0}/webapps/mpp/paypal-fees", country));
    }
    const int CashoutNeedGold = 500000;
    const int PtCashoutRate = 1000;
    protected override void BeforeShowAnimation(params int[] args)
    {
        bool hasEmail = !string.IsNullOrEmpty(Save.data.mainData.user_paypal);
        if (hasEmail)
            emailText.text = Save.data.mainData.user_paypal;
        else
            emailText.text = "Please bind paypal account";
        pt_numText.text = "0<size=40>  PT</size>";
        pt_cashout_numText.text = "≈$0.00";
        cash_numText.text = "$ " + Save.data.mainData.user_doller_live.GetCashShowString();
        gold_tipText.text = (CashoutNeedGold - Save.data.mainData.user_gold_live).GetTokenShowString() + " more gold to redeem";
    }
    bool isPause = false;
    public override void Pause()
    {
        isPause = true;
    }
    public override void Resume()
    {
        if (!isPause) return;
        isPause = false;
        BeforeShowAnimation();
    }
}
public enum CashoutType
{
    PT,
    Cash,
    Gold
}
