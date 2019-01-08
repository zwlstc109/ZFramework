using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using System;
public class PanelMainMenu2 : PanelBase
{
    [SerializeField] private Transform mMask = null;
    [SerializeField] private Button mBtnTest1 = null;
    [SerializeField] private Button mBtnTest2 = null;
    [SerializeField] private Button mBtnTest3 = null;
    PanelBase pnl1 = null;
    PanelBase pnl2 = null;
    PanelBase pnl3 = null;
    
    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        mBtnTest1.onClick.AddListener(() =>
        {
            if (pnl1 == null)
            {
                pnl1= Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest4.prefab",parent:mMask, userData: mBtnTest1.transform.localPosition.y,allowMultiInstance:true);
            }
            else
                Switch(pnl1);
        });
        mBtnTest2.onClick.AddListener(() =>
        {
            if (pnl2 == null)
            {
              pnl2=  Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest4.prefab", parent: mMask, userData: mBtnTest2.transform.localPosition.y, allowMultiInstance: true);
            }
            else
                Switch(pnl2);
        });
        mBtnTest3.onClick.AddListener(() =>
        {
            if (pnl3 == null)
            {
              pnl3=  Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest4.prefab", parent: mMask, userData: mBtnTest3.transform.localPosition.y, allowMultiInstance: true);
            }
            else
                Switch(pnl3);

        });
    }

    public override void OnCover(object userData = null)
    {
       
        Z.Debug.Log("MainMenu2 OnCover");
    }

    public override void OnReveal(object userData = null)
    {
        
        Z.Debug.Log("MainMenu2 OnReveal");
    }

    public override void OnSwitch(object userData = null)
    {
        //禁用默认的切换行为
        //base.OnSwitch(userData);
    }
   
}