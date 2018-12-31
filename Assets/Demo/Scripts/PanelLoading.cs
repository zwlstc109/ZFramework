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
    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        gameObject.Show(); // TODO 不是很优雅 也不想写到Base里 考虑一种更好的方式
    }
    public override void OnClose(object userData = null)
    {
        base.OnClose(userData);
        gameObject.Hide();
    }
    public override void OnUpdate(object userData = null)
    {
        base.OnUpdate(userData);
        mSlider.value = Z.Scene.LoadingProgress / 100.0f;
        if (Z.Scene.Already)
        {
            CloseUI();
        }
    }
}