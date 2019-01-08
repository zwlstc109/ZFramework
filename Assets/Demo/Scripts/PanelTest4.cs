using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using DG.Tweening;
using System;
public class PanelTest4 : PanelBase
{
    
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        float y = (float)userData;
        transform.SetLocalPositionY(y);
        transform.localPosition = transform.localPosition + Vector3.left * 400;
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        Z.Debug.Log("a Test4 OnOpen");
        transform.DOBlendableLocalMoveBy(Vector3.right * 400, 0.6f)/*.onComplete += () => Z.Subject.Fire("Z_UIComplete", null)*/;
    }

    public override void OnClose(object userData = null)
    {       
        Z.Debug.Log("a Test4 OnClose");
        Available = false;//注意这里为了避免在dotween中重复触发OnClose（比如onSwicth触发关闭时又点了自己关闭） 提前available=false 一个面板想要支持switch的dotween 建议就这么写
        transform.DOBlendableLocalMoveBy(Vector3.left * 400, 0.4f).onComplete += () => Visible=false;
    }
    public override void OnSwitch(object userData = null)
    {
        base.OnSwitch(userData);
        Z.Debug.Log("a Test4 OnSwicth");
        CloseSelf();
    }
    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
       
    }
}