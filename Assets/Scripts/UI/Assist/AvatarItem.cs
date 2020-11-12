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
    public void Init(int head_icon_index,int unlock_lv,bool isNew,bool isSelect)
    {
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + head_icon_index);
        lockText.text = "Lv." + unlock_lv + "\nunlock";
        newGo.SetActive(!isSelect && isNew);
        selectGo.SetActive(isSelect);
    }
}
