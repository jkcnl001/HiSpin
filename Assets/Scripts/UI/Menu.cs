using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour, IUIBase
{
    CanvasGroup canvasGroup;

    public Button offerwallButton;
    public Button rankButton;
    public Button slotsButton;
    public Button lotteryButton;
    public Button firendButton;
    public Button settingButton;
    public Button add_ticketButton;
    public Button backButton;

    public RectTransform selectRect;
    [Space(15)]
    public Text gold_numText;
    public Text cash_numText;
    public Text ticket_numText;
    [Space(15)]
    public GameObject all_bottomGo;
    public GameObject all_topGo;
    public GameObject all_tokenGo;
    public Text top_titleText;
    [NonSerialized]
    public readonly Dictionary<Reward, Transform> fly_target_dic = new Dictionary<Reward, Transform>();
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        offerwallButton.AddClickEvent(OnOfferwallButtonClick);
        rankButton.AddClickEvent(OnRankButtonClick);
        slotsButton.AddClickEvent(OnSlotsButtonClick);
        lotteryButton.AddClickEvent(OnLotteryButtonClick);
        firendButton.AddClickEvent(OnFriendButtonClick);
        settingButton.AddClickEvent(OnSettingButtonClick);
        add_ticketButton.AddClickEvent(OnAddTicketButtonClick);
        backButton.AddClickEvent(OnBackButtonClick);
        fly_target_dic.Add(Reward.Gold, gold_numText.transform.parent);
        fly_target_dic.Add(Reward.Cash, cash_numText.transform.parent);
        fly_target_dic.Add(Reward.Ticket, ticket_numText.transform.parent);
        if (Master.IsBigScreen)
        {
            RectTransform topRect = all_topGo.transform as RectTransform;
            topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + 100);
        }
    }
    #region button event
    private void OnOfferwallButtonClick()
    {
        if (!Ads._instance.ShowOfferwallAd())
            Master.Instance.ShowTip("Sorry, loading failed. Try again later.", 2);
    }
    private void OnRankButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Rank);
    }
    private void OnSlotsButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Slots);
    }
    private void OnLotteryButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Betting);
    }
    private void OnFriendButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Friend);
    }
    private void OnSettingButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Setting);
    }
    private void OnAddTicketButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Task);
    }
    private void OnBackButtonClick()
    {
        UI.CloseCurrentBasePanel();
    }
    #endregion
    private Button currentBottomButton = null;
    private void OnChangeBottomButton(Button clickButton)
    {
        if (currentBottomButton != null)
            currentBottomButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, currentBottomButton.name + "_Off");
        currentBottomButton = clickButton;
        selectRect.localPosition = new Vector3(currentBottomButton.transform.localPosition.x, selectRect.localPosition.y);
        currentBottomButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Menu, currentBottomButton.name + "_On");
    }
    #region update top token text
    public void UpdateGoldText()
    {
        gold_numText.text = Save.data.mainData.user_gold_live.GetTokenShowString();
    }
    public void UpdateCashText()
    {
        cash_numText.text = Save.data.mainData.user_doller_live.GetCashShowString();
    }
    public void UpdateTicketText()
    {
        ticket_numText.text = Save.data.mainData.user_tickets.GetTokenShowString();
    }
    #endregion
    #region stateChange
    public IEnumerator Show(params int[] args)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        Server.Instance.RequestData(Server.Server_RequestType.BettingData, OnRequestDataCallback, null);
        yield return null;
    }
    public void OnStartNewBetting()
    {
        Server.Instance.RequestData(Server.Server_RequestType.BettingData, OnRequestDataCallback, null);
    }
    private void OnRequestDataCallback()
    {
        UpdateGoldText();
        UpdateTicketText();
        bool hasSelf = false;
        List<BettingWinnerInfo> bettingDatas = Save.data.betting_data.ranking;
        if (bettingDatas != null)
        {
            foreach (var winner in bettingDatas)
            {
                if (winner.user_id == Save.data.mainData.user_id)
                {
                    hasSelf = true;
                    cash_numText.text = (Save.data.mainData.user_doller_live - winner.user_num).GetCashShowString();
                    break;
                }
            }
            if (!hasSelf)
                UpdateCashText();
        }
        else
            UpdateCashText();
        OnSlotsButtonClick();
    }
    public IEnumerator Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        yield return null;
    }
    bool isPause = false;
    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        if (!isPause) return;
        isPause = false;
        UpdateGoldText();
        UpdateCashText();
        UpdateTicketText();
    }
    #endregion
    public void OnBasePanelShow(int panelIndex)
    {
        BasePanel basePanelType = (BasePanel)panelIndex;
        //OnChangeBottomButton(giftButton);
        //OnChangeBottomButton(firendButton);
        switch (basePanelType)
        {
            case BasePanel.Cashout:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(false);
                top_titleText.gameObject.SetActive(true);
                top_titleText.text = "CASH OUT";
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                break;
            case BasePanel.CashoutRecord:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(false);
                top_titleText.gameObject.SetActive(true);
                top_titleText.text = "RECORD";
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                break;
            case BasePanel.Task:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(true);
                top_titleText.gameObject.SetActive(false);
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(true);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(true);
                break;
            case BasePanel.PlaySlots:
                all_topGo.SetActive(true);
                all_tokenGo.SetActive(true);
                top_titleText.gameObject.SetActive(false);
                all_bottomGo.SetActive(false);
                backButton.gameObject.SetActive(false);
                settingButton.gameObject.SetActive(false);
                add_ticketButton.gameObject.SetActive(false);
                break;
            case BasePanel.Friend:
                all_bottomGo.SetActive(false);
                all_topGo.SetActive(false);
                break;
            case BasePanel.Rank:
                OnChangeBottomButton(rankButton);
                OnBottomBasePanelShow();
                break;
            case BasePanel.Slots:
                OnChangeBottomButton(slotsButton);
                OnBottomBasePanelShow();
                break;
            case BasePanel.Betting:
                OnChangeBottomButton(lotteryButton);
                OnBottomBasePanelShow();
                break;
            default:
                break;
        }
    }
    private void OnBottomBasePanelShow()
    {
        all_topGo.SetActive(true);
        all_tokenGo.SetActive(true);
        top_titleText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(true);
        all_bottomGo.SetActive(true);
        add_ticketButton.gameObject.SetActive(true);
    }
    public void FlyReward_GetTargetPosAndCallback_ThenFly(Reward type, int num, Vector3 startWorldPos)
    {
        FlyReward.Instance.FlyToTarget(startWorldPos, GetFlyTargetPos(type), num, type, FlyOverCallback);
    }
    private void FlyOverCallback(Reward type)
    {
        StopCoroutine("ExpandTarget");
        StartCoroutine("ExpandTarget", type);
    }
    IEnumerator ExpandTarget(Reward _flyTarget)
    {
        if (!fly_target_dic.TryGetValue(_flyTarget, out Transform tempTrans))
            yield break;
        bool toBiger = true;
        while (true)
        {
            yield return null;
            if (toBiger)
            {
                tempTrans.localScale += Vector3.one * Time.deltaTime * 3;
                if (tempTrans.localScale.x >= 1.3f)
                {
                    toBiger = false;
                    switch (_flyTarget)
                    {
                        case Reward.Gold:
                            UpdateGoldText();
                            break;
                        case Reward.Cash:
                            UpdateCashText();
                            break;
                        case Reward.Ticket:
                            UpdateTicketText();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                tempTrans.localScale -= Vector3.one * Time.deltaTime * 3;
                if (tempTrans.localScale.x <= 1f)
                    break;
            }
        }
        yield return null;
        tempTrans.localScale = Vector3.one;
    }
    private Vector3 GetFlyTargetPos(Reward type)
    {
        if (fly_target_dic.ContainsKey(type))
            return fly_target_dic[type].position;
        else
            return Vector3.zero;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.CloseCurrentBasePanel();
        }
    }
}
