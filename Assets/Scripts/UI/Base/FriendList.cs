using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendList : BaseUI
{
    public Button backButton;
    public Button direct_friendButton;
    public GameObject direct_friend_underline;
    public Button indirect_friendButton;
    public GameObject indirect_friend_underline;
    public Text list_titleText;
    public FriendItem single_friend_item;
    private List<FriendItem> all_friends = new List<FriendItem>();
    public RectTransform allRect;
    public RectTransform bottomRect;
    public RectTransform viewport;
    protected override void Awake()
    {
        base.Awake();
        backButton.AddClickEvent(OnBackButtonClick);
        direct_friendButton.AddClickEvent(OnDirectFriendButtonClick);
        indirect_friendButton.AddClickEvent(OnIndirectFriendButtonClick);
        if (Master.IsBigScreen)
        {
            allRect.sizeDelta = new Vector2(allRect.sizeDelta.x, allRect.sizeDelta.y + Master.TopMoveDownOffset);
            bottomRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
            viewport.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
        }
    }
    private void OnBackButtonClick()
    {
        UI.CloseCurrentBasePanel();
    }
    private void OnDirectFriendButtonClick()
    {
        direct_friend_underline.SetActive(true);
        indirect_friend_underline.SetActive(false);
        SetFriendListShow(true);
    }
    private void OnIndirectFriendButtonClick()
    {
        direct_friend_underline.SetActive(false);
        indirect_friend_underline.SetActive(true);
        SetFriendListShow(false);
    }
    static readonly List<AllData_FriendData_Friend> direct_friend_list = new List<AllData_FriendData_Friend>();
    static readonly List<AllData_FriendData_Friend> indirect_friend_list = new List<AllData_FriendData_Friend>();
    protected override void BeforeShowAnimation(params int[] args)
    {
        direct_friend_list.Clear();
        indirect_friend_list.Clear();
        List<AllData_FriendData_Friend> all_friend_list = Save.data.allData.fission_info.up_user_info.two_user_list;
        int friendCount = all_friend_list.Count;
        for(int i = 0; i < friendCount; i++)
        {
            if (all_friend_list[i].distance == 1)
                direct_friend_list.Add(all_friend_list[i]);
            else
                indirect_friend_list.Add(all_friend_list[i]);
        }
        SetFriendListShow(true);
    }
    const string direct_title = "Friends you invited";
    const string indirect_title = "Friends invited by your friends";
    private void SetFriendListShow(bool isDirect)
    {
        List<AllData_FriendData_Friend> willBeShow = isDirect ? direct_friend_list : indirect_friend_list;
        list_titleText.text = isDirect ? direct_title : indirect_title;
        foreach (var friend in all_friends)
            friend.gameObject.SetActive(false);
        int willbeShowCount = willBeShow.Count;
        for(int i = 0; i < willbeShowCount; i++)
        {
            if (i > all_friends.Count - 1)
            {
                FriendItem newFriend = Instantiate(single_friend_item, single_friend_item.transform.parent).GetComponent<FriendItem>();
                all_friends.Add(newFriend);
            }
            AllData_FriendData_Friend frinedInfo = willBeShow[i];
            all_friends[i].gameObject.SetActive(true);
            all_friends[i].Init(frinedInfo.user_img, frinedInfo.distance, frinedInfo.user_name, frinedInfo.user_time, frinedInfo.user_level);
        }
    }
}
