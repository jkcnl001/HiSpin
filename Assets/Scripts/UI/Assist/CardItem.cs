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
    public RectTransform cash_numRect;
    public RectTransform cash_iconRect;
    public void Init(int head_icon_index,string id,int cashNum)
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_index);
        idText.text = id;
        numText.text = "$" + cashNum.GetCashShowString();
        StartCoroutine(AutoOn());
        StartCoroutine(AutoDealyOrder());
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
    IEnumerator AutoDealyOrder()
    {
        yield return null;
        float totalWidth = cash_iconRect.sizeDelta.x + cash_numRect.sizeDelta.x + 10;
        float x = totalWidth / 2;
        cash_numRect.localPosition = new Vector3(x - cash_numRect.sizeDelta.x / 2, cash_numRect.localPosition.y);
        cash_iconRect.localPosition = new Vector3(-(x - cash_iconRect.sizeDelta.x / 2), cash_iconRect.localPosition.y);
    }
}
