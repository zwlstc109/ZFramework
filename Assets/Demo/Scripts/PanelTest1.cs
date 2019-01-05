using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using DG.Tweening;
using System;
public class PanelTest1 : PanelBase
{
    [SerializeField]private Button mBtnClose = null;
    [SerializeField] private Button mBtnSub = null;

    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);

        mBtnClose.onClick.AddListener(()=>CloseSelf());
        mBtnSub.onClick.AddListener(()=>Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest1Sub.prefab"));
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
     
        Z.Debug.Log("Test1 OnOpen");

        RectTransform rect = transform as RectTransform;
        CanvasGroup.alpha = 0;
        CanvasGroup.DOFade(1, 2).onComplete += () =>
        {
            Z.Subject.Fire("Z_UIComplete",null);
        };
    }

    public override void OnClose(object userData = null)
    {
        base.OnClose(userData);
        Z.Debug.Log("Test1 OnClose");
    }

    public override void OnSwitch(object userData = null)
    {
        base.OnSwitch(userData);
        Z.Debug.Log("Test1 OnSwicth");
    }

    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        mBtnClose.onClick.RemoveAllListeners();
        mBtnSub.onClick.RemoveAllListeners();
    }
}