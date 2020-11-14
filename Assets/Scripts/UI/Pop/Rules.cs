using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rules : PopUI
{
    public Text titleText;
    public RectTransform play_slots_rulesRect;
    public RectTransform betting_rulesRect;
    public RectTransform game_rulesRect;
    public RectTransform invite_firend_rulesRect;
    public RectTransform myinfoRect;
    public RectTransform cashoutRect;
    public Button closeButton;
    public Button sureButton;
    public Button termsButton;
    public Button cashoutButton;
    protected override void Awake()
    {
        base.Awake();
        closeButton.AddClickEvent(ClosePop);
        sureButton.AddClickEvent(ClosePop);
        termsButton.AddClickEvent(OnTermsClick);
        cashoutButton.AddClickEvent(OnCashoutButtonClick);
    }
    private void OnCashoutButtonClick()
    {
        UI.ClosePopPanel(this);
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    private void OnTermsClick()
    {
        Application.OpenURL("http://luckyclub.vip/hispin-termofuse");
    }
    private void ClosePop()
    {
        UI.ClosePopPanel(this);
    }
    const string GameTitle = "CONTEST RULES";
    const string BettingTitle = "How it works?";
    const string PlaySlotsTitle= "How to win";
    const string InviteFriendTitle = "How it works";
    const string MyInfoTitle = "How it works";
    static string BettingContentTop = "1.  There is a prize of 1,000 USD in the Lucky\n    Draw every day.\n2. You must have {0} tickets to participate\n" +
        "    in the Lucky Draw. More tickets you have,\n    more chance to win.\n3. Every day at UTC 08:00:00, all your\n    tickets will be automatically put into the\n" +
        "    prize pool, and  the Lucky Draw will be\n    done.\n4. There will be many winners with different\n    bonus.";
    const string CashoutTitle = "HOW TO MAKE MONEY";
    private RuleArea ruleArea;
    protected override void BeforeShowAnimation(params int[] args)
    {
        ruleArea = (RuleArea)args[0];
        switch (ruleArea)
        {
            case RuleArea.GameRule:
                titleText.text = GameTitle;
                game_rulesRect.gameObject.SetActive(true);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(false);
                myinfoRect.gameObject.SetActive(false);
                cashoutRect.gameObject.SetActive(false);
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                sureButton.transform.localPosition = new Vector3(0, game_rulesRect.localPosition.y - game_rulesRect.sizeDelta.y- 50);
                break;
            case RuleArea.Betting:
                titleText.text = BettingTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(true);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(false);
                myinfoRect.gameObject.SetActive(false);
                cashoutRect.gameObject.SetActive(false);
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                sureButton.transform.localPosition = new Vector3(0, betting_rulesRect.localPosition.y - betting_rulesRect.sizeDelta.y - 50);
                betting_rulesRect.GetComponent<Text>().text = string.Format(BettingContentTop, Save.data.allData.award_ranking.ticktes_flag);
                break;
            case RuleArea.PlaySlots:
                titleText.text = PlaySlotsTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(true);
                invite_firend_rulesRect.gameObject.SetActive(false);
                myinfoRect.gameObject.SetActive(false);
                cashoutRect.gameObject.SetActive(false);
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                sureButton.transform.localPosition = new Vector3(0, play_slots_rulesRect.localPosition.y - play_slots_rulesRect.sizeDelta.y - 50);
                break;
            case RuleArea.InviteFriend:
                titleText.text = InviteFriendTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(true);
                myinfoRect.gameObject.SetActive(false);
                cashoutRect.gameObject.SetActive(false);
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                sureButton.transform.localPosition = new Vector3(0, invite_firend_rulesRect.localPosition.y - invite_firend_rulesRect.sizeDelta.y - 50);
                break;
            case RuleArea.MyInfo:
                titleText.text = MyInfoTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(false);
                myinfoRect.gameObject.SetActive(true);
                cashoutRect.gameObject.SetActive(false);
                cashoutButton.gameObject.SetActive(false);
                sureButton.gameObject.SetActive(true);
                sureButton.transform.localPosition = new Vector3(0, myinfoRect.localPosition.y - myinfoRect.sizeDelta.y - 50);
                break;
            case RuleArea.Cashout:
                titleText.text = CashoutTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(false);
                myinfoRect.gameObject.SetActive(false);
                cashoutRect.gameObject.SetActive(true);
                cashoutButton.gameObject.SetActive(true);
                sureButton.gameObject.SetActive(false);
                cashoutButton.transform.localPosition = new Vector3(0, cashoutRect.localPosition.y - cashoutRect.sizeDelta.y - 150);
                break;
        }
    }
}
public enum RuleArea
{
    GameRule,
    Betting,
    PlaySlots,
    InviteFriend,
    MyInfo,
    Cashout,
}
