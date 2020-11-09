using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CashoutRecord : BaseUI
{
    public RectTransform anchor_rect;
    public CashoutRecordItem single_cashout_record_item;
    private List<CashoutRecordItem> cashoutRecordItems = new List<CashoutRecordItem>();
    protected override void Awake()
    {
        base.Awake();
        cashoutRecordItems.Add(single_cashout_record_item);
        if (Master.IsBigScreen)
        {
            anchor_rect.localPosition -= new Vector3(0, 100, 0);
            anchor_rect.sizeDelta += new Vector2(0, Screen.height - 1920 - 200);
            anchor_rect.GetComponentInChildren<ScrollRect>().normalizedPosition = Vector2.one;
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        Server.Instance.RequestData(Server.Server_RequestType.CashoutRecordData, InitRecord, null);
    }
    private void InitRecord()
    {
        foreach (var record in cashoutRecordItems)
            record.gameObject.SetActive(false);
        List<CashoutRecordInfo> all_records_info = Save.data.cashout_record_data.record;
        int count = all_records_info.Count;
        for(int i = 0; i < count; i++)
        {
            if (i > cashoutRecordItems.Count - 1)
            {
                CashoutRecordItem newItem = Instantiate(single_cashout_record_item, single_cashout_record_item.transform.parent).GetComponent<CashoutRecordItem>();
                cashoutRecordItems.Add(newItem);
            }
            CashoutRecordInfo recordInfo = all_records_info[i];
            cashoutRecordItems[i].gameObject.SetActive(true);
            cashoutRecordItems[i].Init(recordInfo.apply_type, recordInfo.apply_num, recordInfo.apply_time, recordInfo.apply_status, recordInfo.apply_doller);
        }
    }
}
