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
    public Button closeButton;
    public Button sureButton;
    public Button termsButton;
    protected override void Awake()
    {
        base.Awake();
        closeButton.AddClickEvent(ClosePop);
        sureButton.AddClickEvent(ClosePop);
        termsButton.AddClickEvent(OnTermsClick);
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
                sureButton.transform.localPosition = new Vector3(0, game_rulesRect.localPosition.y - game_rulesRect.sizeDelta.y- 50);
                break;
            case RuleArea.Betting:
                titleText.text = BettingTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(true);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(false);
                sureButton.transform.localPosition = new Vector3(0, betting_rulesRect.localPosition.y - betting_rulesRect.sizeDelta.y - 50);
                break;
            case RuleArea.PlaySlots:
                titleText.text = PlaySlotsTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(true);
                invite_firend_rulesRect.gameObject.SetActive(false);
                sureButton.transform.localPosition = new Vector3(0, play_slots_rulesRect.localPosition.y - play_slots_rulesRect.sizeDelta.y - 50);
                break;
            case RuleArea.InviteFriend:
                titleText.text = InviteFriendTitle;
                game_rulesRect.gameObject.SetActive(false);
                betting_rulesRect.gameObject.SetActive(false);
                play_slots_rulesRect.gameObject.SetActive(false);
                invite_firend_rulesRect.gameObject.SetActive(true);
                sureButton.transform.localPosition = new Vector3(0, invite_firend_rulesRect.localPosition.y - invite_firend_rulesRect.sizeDelta.y - 150);
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
}
