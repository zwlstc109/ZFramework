using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using DG.Tweening;
using System;
public class PanelTest3 : PanelBase
{
    [SerializeField] private Button mBtnClose = null;
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);

        mBtnClose.onClick.AddListener(() => CloseSelf());
        transform.localPosition = transform.localPosition + Vector3.right * 700;
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
        Z.Debug.Log("Test3 OnOpen");
        transform.DOBlendableLocalMoveBy(Vector3.left * 700, 0.7f).onComplete += () => Z.Subject.Fire("Z_UIComplete", null);
    }

    public override void OnClose(object userData = null)
    {       
        Z.Debug.Log("Test3 OnClose");
        transform.DOBlendableLocalMoveBy(Vector3.right * 700, 0.4f).onComplete += () => base.OnClose(userData);
    }
    public override void OnSwitch(object userData = null)
    {
        base.OnSwitch(userData);
        Z.Debug.Log("Test3 OnSwicth");
        CloseSelf();
    }
    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        mBtnClose.onClick.RemoveAllListeners();
    }
}