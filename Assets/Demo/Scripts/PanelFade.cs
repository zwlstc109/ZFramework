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
   

    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);    
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        bool fadeIn = (bool)userData;
        if (fadeIn)
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.DOFade(1, 0.5f).onComplete += () =>
            {
                Z.Subject.Fire("Z_UIComplete", null);
                ReleaseSelf();
            };
        }
        else
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.DOFade(0, 1f).onComplete += () =>
            {
                Z.Subject.Fire("Z_UIComplete", null);
                ReleaseSelf();
            };
        }
       

    }

   
}