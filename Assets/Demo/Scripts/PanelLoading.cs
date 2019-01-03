using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using System;
public class PanelLoading : PanelBase
{
    [SerializeField]private Slider mSlider = null;
    //public override void OnOpen(object userData = null)
    //{
    //    base.OnOpen(userData);
       
    //}
    //public override void OnClose(object userData = null)
    //{
    //    base.OnClose(userData);
       
    //}
    public override void OnUpdate(object userData = null)
    {
        base.OnUpdate(userData);
        mSlider.value = Z.Scene.LoadingProgress / 100.0f;
        if (Z.Scene.Already)
        {
            CloseSelf();
        }
    }
}