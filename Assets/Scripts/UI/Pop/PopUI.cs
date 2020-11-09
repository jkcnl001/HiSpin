using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PopUI : MonoBehaviour,IUIBase
{
    CanvasGroup canvasGroup;
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
    public IEnumerator Show(params int[] args)
    {
        BeforeShowAnimation(args);
        canvasGroup.blocksRaycasts = true;
        Transform content = transform.GetChild(1);
        AnimationCurve scaleCurve = Master.Instance.popAnimationScale;
        AnimationCurve alphaCurve = Master.Instance.popAnimationAlpha;
        float scaleEndTime = scaleCurve[scaleCurve.length - 1].time;
        float alphaEndTime = alphaCurve[alphaCurve.length - 1].time;
        float maxTime = Mathf.Max(scaleEndTime, alphaEndTime);
        content.localScale = Vector3.one * scaleCurve[0].value;
        canvasGroup.alpha = alphaCurve[0].value;
        float progress = 0;
        while (progress < maxTime)
        {
            progress += Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
            progress = Mathf.Clamp(progress, 0, maxTime);
            content.localScale = Vector3.one * scaleCurve.Evaluate(progress > scaleEndTime ? scaleEndTime : progress);
            canvasGroup.alpha = alphaCurve.Evaluate(progress > alphaEndTime ? alphaEndTime : progress);
            yield return null;
        }
        canvasGroup.interactable = true;
        AfterShowAnimation(args);
    }
    public IEnumerator Close()
    {
        BeforeCloseAnimation();
        canvasGroup.interactable = false;
        Transform content = transform.GetChild(1);
        AnimationCurve scaleCurve = Master.Instance.popAnimationScale;
        AnimationCurve alphaCurve = Master.Instance.popAnimationAlpha;
        float scaleEndTime = scaleCurve[scaleCurve.length - 1].time;
        float alphaEndTime = alphaCurve[alphaCurve.length - 1].time;
        float maxTime = Mathf.Max(scaleEndTime, alphaEndTime);
        content.localScale = Vector3.one * scaleCurve[0].value;
        canvasGroup.alpha = alphaCurve[0].value;
        float progress = maxTime;
        while (progress > 0)
        {
            progress -= Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
            progress = Mathf.Clamp(progress, 0, maxTime);
            content.localScale = Vector3.one * scaleCurve.Evaluate(progress > scaleEndTime ? scaleEndTime : progress);
            canvasGroup.alpha = alphaCurve.Evaluate(progress > alphaEndTime ? alphaEndTime : progress);
            yield return null;
        }
        canvasGroup.blocksRaycasts = false;
        AfterCloseAnimation();
    }

    public void Pause()
    {
    }

    public void Resume()
    {
    }

}
