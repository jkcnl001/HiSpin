using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendInviteRecordItem : MonoBehaviour
{
    public Image head_iconImage;
    public Text nameText;
    public Text idText;
    public Text reward_pt_numText;
    public void Init(int head_icon_id, string name, string id, int rewardPtNum)
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_id);
        nameText.text = name;
        idText.text = "ID:" + id;
        reward_pt_numText.text = string.Format("+{0} <size=40>Pt</size>", rewardPtNum.GetTokenShowString());
    }
}
