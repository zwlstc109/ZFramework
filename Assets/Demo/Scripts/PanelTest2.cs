using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using DG.Tweening;
using System;
public class PanelTest2 : PanelBase
{
    [SerializeField] private Button mBtnClose=null;
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        transform.localScale = Vector3.zero;
        mBtnClose.onClick.AddListener(() => CloseSelf());
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        transform.SetAsLastSibling();
        Z.Debug.Log("Test2 OnOpen");      
        transform.DOScale(Vector3.one, 0.5f).onComplete += () => Z.Subject.Fire("Z_UIComplete", null);       
    }

    public override void OnClose(object userData = null)
    {      
        Z.Debug.Log("Test2 OnClose");
        transform.SetAsFirstSibling();
        transform.DOScale(Vector3.zero, 0.5f).onComplete += () =>base.OnClose(userData);
    }
    public override void OnSwitch(object userData = null)
    {
        base.OnSwitch(userData);
        Z.Debug.Log("Test2 OnSwicth");
        CloseSelf();
    }
    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        mBtnClose.onClick.RemoveAllListeners();
    }
}