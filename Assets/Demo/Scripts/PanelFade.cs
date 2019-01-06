using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using DG.Tweening;
using System;
public class PanelFade : PanelBase
{
    private IDisposable mFadeInActionDisposer = null;
    private FadeData tempData = null;
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        tempData = (FadeData)userData;
        switch (tempData.Mode)
        {
            case FadeMode.FadeIn:
                CanvasGroup.alpha = 0;
                CanvasGroup.DOFade(1, 0.5f).onComplete += () =>
                {
                    tempData.FadeInCallback?.Invoke();                   
                    ReleaseSelf(false,tempData);
                };
                break;
            case FadeMode.FadeOut:
                _FadeOut(null);
                break;
            case FadeMode.FadeInOut:
                CanvasGroup.alpha = 0;
                CanvasGroup.DOFade(1, 0.5f).onComplete += () =>
                {
                    tempData.FadeInCallback?.Invoke();
                    mFadeInActionDisposer = Z.Subject.GetSubject("Z_FadeOutAction").Subscribe(_FadeOut);
                };
                break;
            default:
                break;
        }
    }
    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        var temp = (FadeData)userData;
        Z.Pool.Return(ref temp);

    }
    private void _FadeOut(object n)
    {
        mFadeInActionDisposer?.Dispose();
        CanvasGroup.alpha = 1;
        CanvasGroup.DOFade(0, 1f).onComplete += () =>
        {         
            ReleaseSelf(false, tempData);
        };
    }
   
}