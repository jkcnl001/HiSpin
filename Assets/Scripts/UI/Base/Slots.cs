using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slots : BaseUI
{
    public List<SlotItem> allSlotsItems = new List<SlotItem>();
    public Button cash_outButton;
    public Text time_downText;
    bool isFirstEnter = true;
    protected override void Awake()
    {
        base.Awake();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        cash_outButton.AddClickEvent(OnCashoutButtonClick);
        RectTransform allRect = transform.GetChild(0) as RectTransform;
        if (Master.IsBigScreen)
        {
            allRect.localPosition -= new Vector3(0, Master.TopMoveDownOffset, 0);
            allRect.sizeDelta += new Vector2(0, 1920 * (Master.ExpandCoe - 1) - Master.TopMoveDownOffset);
        }
        allRect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        cash_outButton.gameObject.SetActive(Save.data.isPackB);
    }
    private void OnCashoutButtonClick()
    {
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    public void RefreshSlotsCardState()
    {
        int slotsCount = allSlotsItems.Count;
        int netCount = Save.data.allData.lucky_status.white_lucky.Count;
        if (slotsCount != netCount)
        {
            Debug.LogError("老虎机数量匹配错误");
            return;
        }
        for (int i = 0; i < slotsCount; i++)
        {
            int index = i;
            bool isFree = Save.data.allData.lucky_status.white_lucky[i] == 0;
            allSlotsItems[i].Init(isFree, index);
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        UpdateTimedownText(Master.time);
        RefreshSlotsCardState();
        if (Save.data.allData.user_panel.lucky_count >= 3 && !Save.data.hasRateus)
        {
            Save.data.hasRateus = true;
            UI.ShowPopPanel(PopPanel.CashoutPop, (int)AsCashoutArea.Rateus);
        }
    }
    public void UpdateTimedownText(string time)
    {
        time_downText.text = "NEXT SLOTS IN:\n" + time;
    }
    public void OnChangePackB()
    {
        cash_outButton.gameObject.SetActive(true);
    }
}
