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
   
    private FadeData tempData = null;
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        tempData = (FadeData)userData;
        tempData.FadeEnterCallback?.Invoke();
        switch (tempData.Mode)
        {
            case FadeMode.FadeIn:
                CanvasGroup.alpha = 0;
                CanvasGroup.DOFade(1, 0.5f).onComplete += () =>
                {
                    tempData.FadeInDoneCallback?.Invoke();                   
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
                    Z.Subject.Get("Z_FadeOutAction").First().Subscribe(_FadeOut);
                    tempData.FadeInDoneCallback?.Invoke();
                };
                break;
            case FadeMode.WaitingThenFadeOut:
                CanvasGroup.alpha = 1;
                Z.Subject.Get("Z_FadeOutAction").First().Subscribe(_FadeOut);
                tempData.FadeInDoneCallback?.Invoke();
                break;
            default:
                break;
        }
    }
    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        var temp = (FadeData)userData;
        Z.Pool.Return(temp);

    }
    public override void OnUpdate(object userData = null)
    {
        base.OnUpdate(userData);
        Z.Audio.BackGroundVolume = 1 - CanvasGroup.alpha;
    }
    private void _FadeOut(object n)
    {
       
        CanvasGroup.alpha = 1;
        CanvasGroup.DOFade(0, 1f).onComplete += () =>
        {         
            ReleaseSelf(false, tempData);
        };
    }
   
}