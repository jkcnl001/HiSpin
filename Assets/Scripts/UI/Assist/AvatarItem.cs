using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItem : MonoBehaviour
{
    public Image head_iconImage;
    public GameObject lockImage;
    public Text lockText;
    public GameObject newGo;
    public GameObject selectGo;
    public Button headButton;
    private bool isLock;
    private bool isSelect;
    private int headid;
    private int index;
    private void Awake()
    {
        headButton.AddClickEvent(OnChangeHead);
    }
    public void Init(int head_icon_index,int unlock_lv,bool isNew,bool isSelect,int index)
    {
        this.index = index;
        headid = head_icon_index;
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_index);
        lockText.text = "Lv." + unlock_lv + "\nunlock";
        isLock = unlock_lv > Save.data.allData.user_panel.user_level;
        lockImage.SetActive(isLock);
        newGo.SetActive(!isLock && !isSelect && isNew);
        selectGo.SetActive(!isLock && isSelect);
        this.isSelect = isSelect;
    }
    private void OnChangeHead()
    {
        if (isSelect) return;
        if (isLock)
            Master.Instance.ShowTip("Level up to unlock", 2);
        else
            //Server.Instance.OperationData_ChangeHead_Name(OnChangeHeadCallback, null, headid, null);
            Server_New.Instance.ConnectToServer_ChangeHedOrName(OnChangeHeadCallback, null, null, true, headid, null);
    }
    private void OnChangeHeadCallback()
    {
        Save.data.head_icon_hasCheck[index] = true;
        Me myInfo = UI.GetUI(BasePanel.Me) as Me;
        if (myInfo != null)
            myInfo.RefreshAvatarList();
    }
}
