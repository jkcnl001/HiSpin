using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save
{
    public static PlayerLocalData data;
    public Save()
    {
        string dataString = PlayerPrefs.GetString("local_Data","");
        if (string.IsNullOrEmpty(dataString))
        {
            data = new PlayerLocalData()
            {
                mainData = new PlayerMainData(),
                slots_state=new PlayerSlotsStateData(),
                task_list=new PlayerTaskList(),
                rank_data=new PlayerRankData(),
                betting_data=new BettingData(),
                cashout_record_data=new CashoutRecordData(),
                friend_data=new FriendReceiveData(),
                betting_lefttime=new BettingLeftTime(),
                sound_on = true,
                music_on = true,
                enter_slots_time=0,
                input_eamil_time=0,
                hasRateus=false,
            };
            PlayerPrefs.SetString("local_Data", JsonMapper.ToJson(data));
            PlayerPrefs.Save();
        }
        else
            data = JsonMapper.ToObject<PlayerLocalData>(dataString);
    }
    public static void SaveLocalData()
    {
        PlayerPrefs.SetString("local_Data", JsonMapper.ToJson(data));
        PlayerPrefs.Save();
    }
}
public class PlayerLocalData
{
    public PlayerMainData mainData;
    public PlayerSlotsStateData slots_state;
    public PlayerTaskList task_list;
    public PlayerRankData rank_data;
    public BettingData betting_data;
    public CashoutRecordData cashout_record_data;
    public FriendReceiveData friend_data;
    public BettingLeftTime betting_lefttime;
    public bool sound_on;
    public bool music_on;
    public int enter_slots_time;
    public int input_eamil_time;
    public bool hasRateus;
}
