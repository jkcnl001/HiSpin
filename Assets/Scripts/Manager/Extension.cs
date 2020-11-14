using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Extension
{
    public static void AddClickEvent(this Button button,UnityAction call)
    {
        button.onClick.AddListener(call);
        button.onClick.AddListener(Master.PlayButtonClickSound);
    }
    public static void RemoveAllClickEvent(this Button button)
    {
        button.onClick.RemoveAllListeners();
    }
    public static void RemoveClickEvent(this Button button,UnityAction call)
    {
        button.onClick.RemoveListener(call);
    }
    public static string GetCashShowString(this int cashNum)
    {
        string str = cashNum.ToString();
        if (str.Length == 1)
            return "0.0" + str;
        else if (str.Length == 2)
            return "0." + str;
        else
        {
            str = str.Insert(str.Length - 2, ".");
            int length = str.Length;
            int pos = 0;
            for(int i = length - 4; i > 0; i--)
            {
                pos++;
                if (pos % 3 == 0)
                    str = str.Insert(i, ",");
            }
            return str;
        }
    }
    public static string GetTokenShowString(this int tokenNum)
    {
        string str = tokenNum.ToString();
        int length = str.Length;
        int pos = 0;
        for (int i = length - 1; i > 0; i--)
        {
            pos++;
            if (pos % 3 == 0 )
                str = str.Insert(i, ",");
        }
        return str;
    }
    public static string GetTicketMultipleString(this int multiple)
    {
        string mulStr = multiple.ToString();
        if (mulStr.Length == 1)
            return "0." + mulStr;
        else
            return mulStr.Insert(mulStr.Length - 1, ".");
    }
    public static string CheckName(this string str)
    {
        if (str == null || str.Length == 0) { return ""; }

        StringBuilder @string = new StringBuilder();
        int charCount = 0;
        int stringIndex = 0;
        int stringLength = str.Length;
        while (stringIndex < stringLength)
        {
            @string.Append(str[stringIndex]);
            charCount++;
            if (str[stringIndex] > 128)
                charCount++;
            if (charCount >= 16)
                break;
            stringIndex++;
        }
        return @string.ToString();
    }
}
