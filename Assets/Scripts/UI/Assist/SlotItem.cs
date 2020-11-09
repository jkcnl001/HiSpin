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
    private void Awake()
    {
        button.AddClickEvent(OnClick);
    }
    public void Init(int rewardedCashNum,bool isFree,int index)
    {
        this.index = index;
        bgImage.sprite = Sprites.GetBGSprite("bg_" + index);
        titleImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Slots, "title_" + index);
        reward_numText.text = rewardedCashNum.GetTokenShowString();
        isAd = !isFree;
        ad_maskGo.SetActive(isAd);
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
        Master.Instance.SendAdjustEnterSlotsEvent(isAd);
        UI.ShowBasePanel(BasePanel.PlaySlots, index, isAd ? 1 : 0);
    }
}
