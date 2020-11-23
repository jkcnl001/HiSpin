using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Offerwall : BaseUI
{
    public Button helpButton;
    public Text pt_numText;
    public Button cashoutButton;
    [Space(15)]
    public Button adgemButton;
    public Text adgem_button_contentText;
    public GameObject adgem_coinGo;
    [Space(15)]
    public Button isButton;
    public Text is_button_contentText;
    public GameObject is_coinGo;
    [Space(15)]
    public Button fyberButton;
    public Text fyber_button_contentText;
    public GameObject fyber_coinGo;
    const string loading = "Loading...";
    const string ready = "Earn points     ";
    [Space(15)]
    public RectTransform topRect;
    public RectTransform viewportRect;

    protected override void Awake()
    {
        base.Awake();
        helpButton.AddClickEvent(OnHelpButtonClick);
        cashoutButton.AddClickEvent(OnCashoutButtonClick);
        adgemButton.AddClickEvent(OnAdgemButtonClick);
        isButton.AddClickEvent(OnISButtonClick);
        fyberButton.AddClickEvent(OnFyberButtonClick);
        if (Master.IsBigScreen)
        {
            topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + Master.TopMoveDownOffset);
            viewportRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
        }
    }
    private void OnHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.Offerwall);
    }
    private void OnCashoutButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    private void OnAdgemButtonClick()
    {

    }
    private void OnISButtonClick()
    {
        Ads._instance.ShowOfferwallAd();
    }
    private void OnFyberButtonClick()
    {

    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        base.BeforeShowAnimation(args);
        cashoutButton.gameObject.SetActive(Save.data.isPackB);
        pt_numText.text = ((int)Save.data.allData.fission_info.live_balance).GetTokenShowString() + " <size=70>Pt</size>";
    }
}
