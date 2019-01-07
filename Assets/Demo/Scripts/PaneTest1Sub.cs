using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using DG.Tweening;
using System;
public class PaneTest1Sub : PanelBase
{
    [SerializeField] private Button mBtnClose = null;
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);

        mBtnClose.onClick.AddListener(() => CloseSelf());
        
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        Z.Debug.Log("sub OnOpen");
        Z.UI.Lock();
    }

    public override void OnClose(object userData = null)
    {
        base.OnClose(userData);
        Z.Debug.Log("sub OnClose");
        Z.UI.UnLock();
    }
    
    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        mBtnClose.onClick.RemoveAllListeners();
    }
}