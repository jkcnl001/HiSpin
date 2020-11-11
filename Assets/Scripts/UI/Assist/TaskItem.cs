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
    public GameObject red_pointGo;

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
            {
                red_pointGo.SetActive(false);
                switch (taskTargetId)
                {
                    case PlayerTaskTarget.BuyTicketByGoldOnce:
                        adGo.SetActive(false);
                        button_contentText.text = "BUY";
                        getButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "button");
                        getButton.gameObject.SetActive(true);
                        break;
                    case PlayerTaskTarget.BuyTicketByRvOnce:
                        adGo.SetActive(true);
                        button_contentText.text = "    GET";
                        getButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "button");
                        getButton.gameObject.SetActive(true);
                        break;
                    case PlayerTaskTarget.InviteAFriend:
                    case PlayerTaskTarget.WritePaypalEmail:
                        adGo.SetActive(false);
                        button_contentText.text = "GO TO";
                        getButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "button");
                        getButton.gameObject.SetActive(true);
                        break;
                    case PlayerTaskTarget.OwnSomeGold:
                    case PlayerTaskTarget.WatchRvOnce:
                    case PlayerTaskTarget.PlaySlotsOnce:
                    case PlayerTaskTarget.PlayBettingOnce:
                    case PlayerTaskTarget.CashoutOnce:
                    case PlayerTaskTarget.WinnerOnce:
                    case PlayerTaskTarget.GetTicketFromSlotsOnce:
                    default:
                        adGo.SetActive(false);
                        button_contentText.text = "CLAIM";
                        getButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "button_grey");
                        getButton.gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                if (taskTargetId == PlayerTaskTarget.InviteAFriend)
                {
                    adGo.SetActive(false);
                    button_contentText.text = "GO TO";
                    getButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "button");
                    getButton.gameObject.SetActive(true);
                    red_pointGo.SetActive(false);
                }
                else
                {
                    adGo.SetActive(false);
                    getButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Task, "button");
                    button_contentText.text = "CLAIM";
                    getButton.gameObject.SetActive(true);
                    red_pointGo.SetActive(true);
                }
            }
        }
    }
    private void OnGetButtonClick()
    {
        if (!HasFinish)
        {
            switch (TaskTarget)
            {
                case PlayerTaskTarget.WritePaypalEmail:
                    UI.ShowBasePanel(BasePanel.Cashout);
                    break;
                case PlayerTaskTarget.InviteAFriend:
                    UI.ShowBasePanel(BasePanel.Friend);
                    break;
                case PlayerTaskTarget.BuyTicketByGoldOnce:
                    if (Save.data.allData.user_panel.user_gold_live >= 800)
                    {
                        Server.Instance.OperationData_BuyTickets(OnFinishTaskCallback, OnErrorCallback, false);
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
            switch (TaskTarget)
            {
                case PlayerTaskTarget.PlaySlotsOnce:
                case PlayerTaskTarget.PlayBettingOnce:
                case PlayerTaskTarget.WatchRvOnce:
                case PlayerTaskTarget.CashoutOnce:
                case PlayerTaskTarget.OwnSomeGold:
                case PlayerTaskTarget.WinnerOnce:
                case PlayerTaskTarget.GetTicketFromSlotsOnce:
                case PlayerTaskTarget.WritePaypalEmail:
                    Server.Instance.OperationData_FinishTask(OnFinishTaskCallback, OnErrorCallback, Task_ID, false, RewardType);
                    break;
                case PlayerTaskTarget.InviteAFriend:
                    UI.ShowBasePanel(BasePanel.Friend);
                    break;
                case PlayerTaskTarget.BuyTicketByGoldOnce:
                    break;
                case PlayerTaskTarget.BuyTicketByRvOnce:
                    break;
                default:
                    break;
            }
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
        OnErrorCallback();
    }
    private void OnErrorCallback()
    {
        Server.Instance.RequestData(Server.Server_RequestType.TaskData, () =>
        {
            Tasks tasks = UI.GetUI(BasePanel.Task) as Tasks;
            tasks.RefreshTaskInfo();
            bool hasFinish = false;
            foreach (var task in Save.data.allData.lucky_schedule.user_task)
            {
                if (task.taskTargetId == PlayerTaskTarget.InviteAFriend)
                    continue;
                if (task.task_cur >= task.task_tar && !task.task_receive)
                {
                    HasFinish = true;
                    break;
                }
            }
            UI.OnHasTaskFinished(hasFinish);
        }, null);
    }
    private void OnAdBuyTicketCallback()
    {
        Server.Instance.OperationData_BuyTickets(OnFinishTaskCallback, OnErrorCallback, true);
    }
}
