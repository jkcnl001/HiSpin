using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    public Image bgImage;
    public Image titleImage;
    public Text reward_numText;
    public GameObject ad_maskGo;
    public Button button;
    [NonSerialized]
    public bool isAd = false;
    private int index = 0;
    private int cashNum;
    private void Awake()
    {
        button.AddClickEvent(OnClick);
    }
    public void Init(bool isFree,int index)
    {
        this.index = index;
        bgImage.sprite = Sprites.GetBGSprite("bg_" + index);
        titleImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Slots, "title_" + index);
        if (index == 3 || index == 6)
            cashNum = isFree ? 150 : 500;
        else
            cashNum = isFree ? 50 : 200;
        reward_numText.text = cashNum.ToString();
        isAd = !isFree;
        ad_maskGo.SetActive(isAd);
        reward_numText.transform.parent.gameObject.SetActive(!isAd);
    }
    private void OnClick()
    {
        if (isAd)
            Ads._instance.ShowRewardVideo(OnAdCallback, 2, "rv老虎机", null);
        else
            OnAdCallback();
    }
    private void OnAdCallback()
    {
        Server.Instance.OperationData_ClickSlotsCard(OnSuccessCallback, null, index);
    }
    private void OnSuccessCallback()
    {
        Save.data.allData.user_panel.lucky_count++;
        Master.Instance.SendAdjustEnterSlotsEvent(isAd);
        TaskAgent.TriggerTaskEvent(PlayerTaskTarget.EnterSlotsOnce, 1);
        UI.ShowBasePanel(BasePanel.PlaySlots, index, isAd ? 1 : 0, cashNum, Save.data.allData.lucky_status.lucky_exp);
        UI.MenuPanel.UpdateFreeSlotsLeftNumText();
    }
}
