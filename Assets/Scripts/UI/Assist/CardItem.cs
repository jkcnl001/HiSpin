using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public Image head_iconImage;
    public Text idText;
    public Text numText;
    public CanvasGroup onCg;
    public void Init(int head_icon_index,string id,int cashNum)
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_index);
        idText.text = "ID:\n" + id;
        numText.text = "$" + cashNum.GetCashShowString();
        StartCoroutine(AutoOn());
    }
    public void SetOff()
    {
        onCg.alpha = 0;
    }
    IEnumerator AutoOn()
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime;
            onCg.alpha = progress;
            yield return null;
        }
    }
}
