using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : PopUI
{
    public Button bgButton;
    public Image guideImage;
    public RectTransform guide_maskRect;
    protected override void Awake()
    {
        base.Awake();
        bgButton.AddClickEvent(OnBgClick);
    }
    int guideStep = 0;
    private void OnBgClick()
    {
        guideStep++;
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        guideStep = 1;
        guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_" + guideStep);
        guide_maskRect.position = UI.MenuPanel.GetGudieMaskPosAndSize(guideStep, out Vector2 size);
        guide_maskRect.sizeDelta = size;
        guideImage.transform.localPosition = new Vector3(0, guide_maskRect.localPosition.y - 340, 0);
        StartCoroutine("WaitForClick");
    }
    IEnumerator WaitForClick()
    {
        while (guideStep == 1)
        {
            yield return null;
        }
        guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_" + guideStep);
        guide_maskRect.position = UI.MenuPanel.GetGudieMaskPosAndSize(guideStep, out Vector2 size2);
        guide_maskRect.sizeDelta = size2;
        guideImage.transform.localPosition = new Vector3(0, guide_maskRect.localPosition.y + 451, 0);
        while (guideStep == 2)
        {
            yield return null;
        }
        guideImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.GetCash, "guide_" + guideStep);
        guide_maskRect.position = UI.MenuPanel.GetGudieMaskPosAndSize(guideStep, out Vector2 size3);
        guide_maskRect.sizeDelta = size3;
        guideImage.transform.localPosition = new Vector3(0, guide_maskRect.localPosition.y + 451, 0);
        while (guideStep == 3)
        {
            yield return null;
        }
        UI.ClosePopPanel(this);
    }
}
