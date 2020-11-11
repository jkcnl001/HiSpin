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
                allData = null,
                hasOpenBettingPrize = false,
                sound_on = true,
                music_on = true,
                enter_slots_time = 0,
                input_eamil_time = 0,
                hasRateus = false,
                isPackB = false
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
    public AllData allData;
    public bool hasOpenBettingPrize;
    public bool sound_on;
    public bool music_on;
    public int enter_slots_time;
    public int input_eamil_time;
    public bool hasRateus;
    public bool isPackB;
}
