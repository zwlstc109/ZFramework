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
      
        transform.localPosition = transform.localPosition+Vector3.up*600;
        mBtnClose.onClick.AddListener(()=>CloseSelf());
        mBtnSub.onClick.AddListener(()=>Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest1Sub.prefab"));
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);    
        Z.Debug.Log("Test1 OnOpen");  
        transform.DOBlendableLocalMoveBy(Vector3.down*600, 0.7f).onComplete += () => Z.Subject.Fire("Z_UIComplete", null);
    }

    public override void OnClose(object userData = null)
    {     
        Z.Debug.Log("Test1 OnClose");
        transform.DOBlendableLocalMoveBy(Vector3.up*600, 0.4f).onComplete += () =>base.OnClose(userData);           
    }

    public override void OnSwitch(object userData = null)
    {
        base.OnSwitch(userData);
        Z.Debug.Log("Test1 OnSwicth");
        CloseSelf();
    }

    public override void OnUnLoad(object userData = null)
    {
        base.OnUnLoad(userData);
        mBtnClose.onClick.RemoveAllListeners();
        mBtnSub.onClick.RemoveAllListeners();
    }
}