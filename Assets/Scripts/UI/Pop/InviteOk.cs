using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteOk : PopUI
{
    public Image reward_iconImage;
    public Text reward_numText;
    public Text double_buttonText;
    public Button double_rewardButton;
    public Button single_rewardButton;
    public GameObject adGo;
    protected override void Awake()
    {
        base.Awake();
        double_rewardButton.AddClickEvent(OnDoubleClick);
        single_rewardButton.AddClickEvent(OnSingleClick);
    }
    private void OnDoubleClick()
    {
        OnGetTaskListCallback(false);
    }
    private void OnGetTaskListCallback(bool doublReward)
    {
        List<AllData_Task> taskDatas = Save.data.allData.lucky_schedule.user_task;
        AllData_Task inviteTaskData = null;
        if (taskDatas != null && taskDatas.Count > 0)
        {
            foreach (var task in taskDatas)
            {
                if (task.taskTargetId == PlayerTaskTarget.InviteAFriend)
                {
                    inviteTaskData = task;
                    break;
                }
            }
        }
        if (inviteTaskData == null)
            Master.Instance.ShowTip("Error: can not get task id", 2);
        else
            //Server.Instance.OperationData_FinishTask(OnGetRewardCallback, null, inviteTaskData.task_id, doublReward, Reward.Cash, Reward.Ticket);
            Server_New.Instance.ConnectToServer_FinishTask(OnGetRewardCallback, null, null, true, inviteTaskData.task_id, doublReward, Reward.Cash, Reward.Ticket);
    }
    private void OnGetRewardCallback()
    {
        Save.data.allData.fission_info.reward_conf.invite_receive++;
        UI.ClosePopPanel(this);
    }
    private void OnSingleClick()
    {
        OnGetTaskListCallback(false);
    }
    Reward invite_ok_reward_type;
    int invite_ok_reward_num;
    protected override void BeforeShowAnimation(params int[] args)
    {
        invite_ok_reward_type = (Reward)args[0];
        invite_ok_reward_num = args[1];
        reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.InviteOk, invite_ok_reward_type.ToString().ToLower());
        single_rewardButton.gameObject.SetActive(false);
        adGo.SetActive(false);
        double_buttonText.text = "Claim";
        if (invite_ok_reward_type == Reward.Cash)
        {
            reward_numText.text = "x " + invite_ok_reward_num.GetCashShowString();
        }
        else
        {
            reward_numText.text = "x " + invite_ok_reward_num.GetTokenShowString();
        }
    }
}
