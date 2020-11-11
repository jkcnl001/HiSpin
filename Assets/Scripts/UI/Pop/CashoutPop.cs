using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CashoutPop : PopUI
{
    public Text titleText;
    public Image baseImage;
    public Button closeButton;
    [Space(15)]
    public CanvasGroup Input_emailCg;
    public InputField paypal_accountInput;
    public Button confirm_accountButton;
    [Space(15)]
    public CanvasGroup Cash_outCg;
    public Text cashout_numText;
    public Text handling_feeText;
    public Button cashoutButton;
    [Space(15)]
    public CanvasGroup cashout_fail_helpCg;
    [Space(15)]
    public CanvasGroup rate_usCg;
    public Button no_starButton;
    public Button yes_starButton;
    protected override void Awake()
    {
        base.Awake();
        closeButton.AddClickEvent(OnCloseButtonClick);
        confirm_accountButton.AddClickEvent(OnConfirmAccountClick);
        cashoutButton.AddClickEvent(OnCashoutClick);
        no_starButton.AddClickEvent(OnCloseButtonClick);
        yes_starButton.AddClickEvent(OnFiveStarClick);
    }
    private void OnFiveStarClick()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.HiSpin.DailyCash.HugeRewards.FreeGame");
#elif UNITY_IOS
        var url = string.Format(
           "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",
           "");
        Application.OpenURL(url);
#endif
    }
    private void OnCloseButtonClick()
    {
        UI.ClosePopPanel(this);
    }
    private void OnConfirmCallback()
    {
        Master.Instance.SendAdjustInputEmailEvent(paypal_accountInput.text);
        TaskAgent.TriggerTaskEvent(PlayerTaskTarget.WritePaypalEmail, 1);
        UI.ClosePopPanel(this);
    }
    private void OnConfirmAccountClick()
    {
        string email = paypal_accountInput.text;
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            Master.Instance.ShowTip("Account is empty!");
        else
            Server.Instance.OperationData_BindPaypal(OnConfirmCallback, null, paypal_accountInput.text);
    }
    private void OnCashoutClick()
    {
        Server.Instance.OperationData_Cashout(OnCashoutCallback, null, cashoutType, cashoutTypeNum, cashoutNum);
    }
    private void OnCashoutCallback()
    {
        Master.Instance.ShowTip("Succeed! We will finish it in 7 working days.",3);
        UI.ClosePopPanel(this);
    }
    AsCashoutArea asArea;
    CashoutType cashoutType;
    int cashoutTypeNum;
    int cashoutNum;
    protected override void BeforeShowAnimation(params int[] args)
    {
        asArea = (AsCashoutArea)args[0];
        switch (asArea)
        {
            case AsCashoutArea.PaypalEmail:
                Input_emailCg.alpha = 1;
                Input_emailCg.blocksRaycasts = true;
                Cash_outCg.alpha = 0;
                Cash_outCg.blocksRaycasts = false;
                cashout_fail_helpCg.alpha = 0;
                cashout_fail_helpCg.blocksRaycasts = false;
                rate_usCg.alpha = 0;
                rate_usCg.blocksRaycasts = false;
                titleText.text = "PAYPAL ACCOUNT";
                baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_n");
                break;
            case AsCashoutArea.Cashout:
                Input_emailCg.alpha = 0;
                Input_emailCg.blocksRaycasts = false;
                Cash_outCg.alpha = 1;
                Cash_outCg.blocksRaycasts = true;
                cashout_fail_helpCg.alpha = 0;
                cashout_fail_helpCg.blocksRaycasts = false;
                rate_usCg.alpha = 0;
                rate_usCg.blocksRaycasts = false;
                cashoutNum = args[1];
                cashoutType = (CashoutType)args[2];
                cashoutTypeNum = args[3];
                cashout_numText.text = "$ " + args[1].GetTokenShowString();
                titleText.text = "CASH OUT";
                baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_n");
                break;
            case AsCashoutArea.FailHelp:
                Input_emailCg.alpha = 0;
                Input_emailCg.blocksRaycasts = false;
                Cash_outCg.alpha = 0;
                Cash_outCg.blocksRaycasts = false;
                cashout_fail_helpCg.alpha = 1;
                cashout_fail_helpCg.blocksRaycasts = true;
                rate_usCg.alpha = 0;
                rate_usCg.blocksRaycasts = false;
                titleText.text = "FAILED";
                baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_f");
                break;
            case AsCashoutArea.Rateus:
                Input_emailCg.alpha = 0;
                Input_emailCg.blocksRaycasts = false;
                Cash_outCg.alpha = 0;
                Cash_outCg.blocksRaycasts = false;
                cashout_fail_helpCg.alpha = 0;
                cashout_fail_helpCg.blocksRaycasts = false;
                rate_usCg.alpha = 1;
                rate_usCg.blocksRaycasts = true;
                titleText.text = "RATE US";
                baseImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.AsCashoutPop, "base_n");
                break;
        }
    }
}
public enum AsCashoutArea
{
    PaypalEmail,
    Cashout,
    FailHelp,
    Rateus,
}
