using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class BaseUI : MonoBehaviour, IUIBase
{
    protected CanvasGroup canvasGroup;
    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    protected virtual void BeforeShowAnimation(params int[] args)
    {

    }
    protected virtual void AfterShowAnimation(params int[] args)
    {

    }
    protected virtual void BeforeCloseAnimation()
    {

    }
    protected virtual void AfterCloseAnimation()
    {

    }
    public virtual IEnumerator Show(params int[] args)
    {
        BeforeShowAnimation(args);
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        yield return null;
        AfterShowAnimation(args);
    }
    public virtual IEnumerator Close()
    {
        BeforeCloseAnimation();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        yield return null;
        AfterCloseAnimation();
    }
    public virtual void Pause()
    {
    }

    public virtual void Resume()
    {
    }

}
