using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friends : BaseUI
{
    public RectTransform topRect;
    [Space(15)]
    public Button backButton;
    public Button helpButton;
    public Text pt_numText;
    public Button cashoutButton;
    public Text myfriends_numText;
    public Button inviteButton;
    public Text invite_reward_numText;
    public Image invite_reward_iconImage;
    public Text yesterday_pt_numText;
    public Text total_pt_numText;
    public GameObject lastdayGo;
    public GameObject nofriend_tipGo;
    public FriendInviteRecordItem single_invite_record_item;
    private List<FriendInviteRecordItem> all_invite_friend_items = new List<FriendInviteRecordItem>();
    protected override void Awake()
    {
        base.Awake();
        backButton.AddClickEvent(OnBackButtonClick);
        helpButton.AddClickEvent(OnHelpButtonClick);
        cashoutButton.AddClickEvent(OnCashoutButtonClick);
        inviteButton.AddClickEvent(OnInviteButtonClick);
        all_invite_friend_items.Add(single_invite_record_item);
        if (Master.IsBigScreen)
        {
            topRect.sizeDelta = new Vector2(topRect.sizeDelta.x, topRect.sizeDelta.y + 100);
        }
        cashoutButton.gameObject.SetActive(Save.data.isPackB);
    }
    private void OnBackButtonClick()
    {
        UI.CloseCurrentBasePanel();
    }
    private void OnHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.InviteFriend);
    }
    private void OnCashoutButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    private AndroidJavaClass _aj;
    private AndroidJavaClass _AJ
    {
        get
        {
            if (_aj == null)
                _aj = new AndroidJavaClass("com.wyx.shareandcopy.Share_Copy");
            return _aj;

        }
    }
    private void OnInviteButtonClick()
    {
#if UNITY_EDITOR
        return;
#endif
        _AJ.CallStatic("ShareString", string.Format("http://admin.crsdk.com:8000/invita?user={0}&irsource_name={2}&app_name={1}", Save.data.allData.user_panel.user_id, "com.HiSpin.DailyCash.HugeRewards.FreeGame", Server.Bi_name));
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        RefreshFriendList();
        int invite_people_num = Save.data.allData.fission_info.user_invite_people;
        int invite_people_receive = Save.data.allData.fission_info.reward_conf.invite_receive;
        int not_received_invite_reward = invite_people_num - invite_people_receive;
        for (int i = 0; i < not_received_invite_reward; i++)
        {
            int receiveTime = invite_people_receive + i + 1;
            if (receiveTime <= Save.data.allData.fission_info.reward_conf.invite_flag)
                UI.ShowPopPanel(PopPanel.InviteOk, (int)Save.data.allData.fission_info.reward_conf.lt_flag_type, Save.data.allData.fission_info.reward_conf.lt_flag_num);
            else
                UI.ShowPopPanel(PopPanel.InviteOk, (int)Save.data.allData.fission_info.reward_conf.gt_flag_type, Save.data.allData.fission_info.reward_conf.gt_flag_num);
        }
    }
    public void RefreshFriendList()
    {
        pt_numText.text = ((int)Save.data.allData.fission_info.live_balance).GetTokenShowString() + " <size=70>Pt</size>";
        int invite_people_num = Save.data.allData.fission_info.user_invite_people;
        int invite_people_receive = Save.data.allData.fission_info.reward_conf.invite_receive;
        myfriends_numText.text = string.Format("My friends: <color=#0596E4>{0}</color>", invite_people_num.GetTokenShowString());
        yesterday_pt_numText.text = ((int)Save.data.allData.fission_info.up_user_info.yestday_team_all).GetTokenShowString() + " <size=55>Pt</size>";
        total_pt_numText.text = ((int)Save.data.allData.fission_info.user_total).GetTokenShowString() +" <size=55>Pt</size>";

        foreach (var friend in all_invite_friend_items)
            friend.gameObject.SetActive(false);

        List<AllData_FriendData_Friend> friend_Infos = Save.data.allData.fission_info.up_user_info.two_user_list;
        int count = friend_Infos.Count;
        for(int i = 0; i < count; i++)
        {
            if (i > all_invite_friend_items.Count - 1)
            {
                FriendInviteRecordItem newRecordItem = Instantiate(single_invite_record_item, single_invite_record_item.transform.parent).GetComponent<FriendInviteRecordItem>();
                all_invite_friend_items.Add(newRecordItem);
            }
            AllData_FriendData_Friend friendInfo = friend_Infos[i];
            all_invite_friend_items[i].gameObject.SetActive(true);
            all_invite_friend_items[i].Init(friendInfo.user_img, friendInfo.user_name, friendInfo.user_id, (int)friendInfo.yestday_doller);
        }
        bool noFriend = count == 0;
        lastdayGo.SetActive(!noFriend);
        nofriend_tipGo.SetActive(noFriend);

        int receiveTime = invite_people_receive + 1;
        if (receiveTime <= Save.data.allData.fission_info.reward_conf.invite_flag)
        {
            invite_reward_numText.text = string.Format("Invite friends to get <color=#FF9732>{0}</color>",
                Save.data.allData.fission_info.reward_conf.lt_flag_type == Reward.Cash ? "$" + Save.data.allData.fission_info.reward_conf.lt_flag_num.GetCashShowString() : Save.data.allData.fission_info.reward_conf.lt_flag_num.GetTokenShowString());
            invite_reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Friend, Save.data.allData.fission_info.reward_conf.lt_flag_type.ToString().ToLower());
        }
        else
        {
            invite_reward_numText.text = string.Format("Invite friends to get <color=#FF9732>{0}</color>",
                Save.data.allData.fission_info.reward_conf.gt_flag_type == Reward.Cash ? "$" + Save.data.allData.fission_info.reward_conf.gt_flag_num.GetCashShowString() : Save.data.allData.fission_info.reward_conf.gt_flag_num.GetTokenShowString());
            invite_reward_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Friend, Save.data.allData.fission_info.reward_conf.gt_flag_type.ToString().ToLower());
        }

    }
    public void OnChangePackB()
    {
        cashoutButton.gameObject.SetActive(true);
    }
}
