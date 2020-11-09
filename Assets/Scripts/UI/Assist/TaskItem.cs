using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
    public Text titleText;
    public Text desText;
    public Text reward_numText;
    public Image reward_iconImage;
    public Button getButton;
    public RectTransform butttonRect;
    public GameObject adGo;
    public Text button_contentText;
    public GameObject doneGo;

    private Reward RewardType;
    private int RewardNum;
    private PlayerTaskTarget TaskTarget;
    private bool HasFinish;
    private int Task_ID;
    private int TaskType;
    private void Awake()
    {
        getButton.AddClickEvent(OnGetButtonClick);
    }
    public void Init(int task_id,string title, string des,PlayerTaskTarget taskTargetId, Reward rewardType, int rewardNum, bool hasdone, bool hasFinish,int taskType)
    {
        Task_ID = task_id;
        titleText.text = title;
        desText.text = des;
        RewardType = rewardType;
        RewardNum = rewardNum;
        TaskTarget = taskTargetId;
        HasFinish = hasFinish;
        TaskType = taskType;

        switch (RewardType)
        {
            case Reward.Gold:
                reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "gold");
                reward_numText.text = rewardNum.GetTokenShowString();
                break;
            case Reward.Cash:
                reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "cash");
                reward_numText.text = rewardNum.GetCashShowString();
                break;
            case Reward.Ticket:
                reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "ticket");
                reward_numText.text = rewardNum.GetTokenShowString();
                break;
            default:
                Debug.LogError("任务奖励错误");
                break;
        }
        if (hasdone)
        {
            doneGo.SetActive(true);
            getButton.gameObject.SetActive(false);
        }
        else
        {
            doneGo.SetActive(false);
            if (!hasFinish)
                switch (taskTargetId)
                {
                    case PlayerTaskTarget.BuyTicketByGoldOnce:
                        adGo.SetActive(false);
                        button_contentText.text = "BUY";
                        getButton.gameObject.SetActive(true);
                        break;
                    case PlayerTaskTarget.BuyTicketByRvOnce:
                        adGo.SetActive(true);
                        button_contentText.text = "    GET";
                        getButton.gameObject.SetActive(true);
                        break;
                    case PlayerTaskTarget.OwnSomeGold:
                    case PlayerTaskTarget.WatchRvOnce:
                        getButton.gameObject.SetActive(false);
                        break;
                    case PlayerTaskTarget.PlaySlotsOnce:
                    case PlayerTaskTarget.PlayBettingOnce:
                    case PlayerTaskTarget.CashoutOnce:
                    case PlayerTaskTarget.WritePaypalEmail:
                    case PlayerTaskTarget.WinnerOnce:
                    case PlayerTaskTarget.InviteAFriend:
                    case PlayerTaskTarget.GetTicketFromSlotsOnce:
                    default:
                        adGo.SetActive(false);
                        button_contentText.text = "GO TO";
                        getButton.gameObject.SetActive(true);
                        break;
                }
            else
            {
                adGo.SetActive(false);
                button_contentText.text = "GET";
            }
        }
    }
    private void OnGetButtonClick()
    {
        if (!HasFinish)
        {
            switch (TaskTarget)
            {
                case PlayerTaskTarget.PlaySlotsOnce:
                    UI.ShowBasePanel(BasePanel.Slots);
                    break;
                case PlayerTaskTarget.PlayBettingOnce:
                    UI.ShowBasePanel(BasePanel.Betting);
                    break;
                case PlayerTaskTarget.OwnSomeGold:
                case PlayerTaskTarget.WatchRvOnce:
                    break;
                case PlayerTaskTarget.CashoutOnce:
                    UI.ShowBasePanel(BasePanel.Cashout);
                    break;
                case PlayerTaskTarget.WritePaypalEmail:
                    UI.ShowBasePanel(BasePanel.Cashout);
                    break;
                case PlayerTaskTarget.WinnerOnce:
                    UI.ShowBasePanel(BasePanel.Betting);
                    break;
                case PlayerTaskTarget.InviteAFriend:
                    UI.ShowBasePanel(BasePanel.Friend);
                    break;
                case PlayerTaskTarget.GetTicketFromSlotsOnce:
                    UI.ShowBasePanel(BasePanel.Slots);
                    break;
                case PlayerTaskTarget.BuyTicketByGoldOnce:
                    if (Save.data.mainData.user_gold_live >= 800)
                    {
                        Server.Instance.OperationData_BuyTickets(OnFinishTaskCallback, null, false);
                    }
                    else
                        Master.Instance.ShowTip("Sorry, you have not enough coins");
                    break;
                case PlayerTaskTarget.BuyTicketByRvOnce:
                    Ads._instance.ShowRewardVideo(OnAdBuyTicketCallback, 2, "rv买票",null);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (TaskTarget != PlayerTaskTarget.BuyTicketByRvOnce && TaskTarget != PlayerTaskTarget.BuyTicketByGoldOnce)
            {
                Server.Instance.OperationData_FinishTask(OnFinishTaskCallback, null, Task_ID, false, RewardType);
            }
            else
                Debug.LogError("购买票任务状态错误");
        }
    }
    private void OnFinishTaskCallback()
    {
        Master.Instance.SendAdjustFinishTaskEvent(Task_ID, TaskType, RewardType, RewardNum);
        UI.FlyReward(RewardType, RewardNum, getButton.transform.position);
        UI.MenuPanel.Pause();
        UI.MenuPanel.Resume();
        UI.CurrentBasePanel.Pause();
        UI.CurrentBasePanel.Resume();
    }
    private void OnAdBuyTicketCallback()
    {
        Server.Instance.OperationData_BuyTickets(OnFinishTaskCallback, null, true);
    }
}
