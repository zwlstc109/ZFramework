﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using System;
public class PanelMainMenu : PanelBase
{
    [SerializeField] private Button mBtnTest1 = null;
    [SerializeField] private Button mBtnTest2 = null;
    [SerializeField] private Button mBtnTest3 = null;


    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        mBtnTest1.onClick.AddListener(() =>
        {
            Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest1.prefab", await: true);
        });
        mBtnTest2.onClick.AddListener(() =>
        {
            Switch("Assets/GameData/Prefabs/UGUI/Panel/PnlTest2.prefab", await: true);
        });
        mBtnTest3.onClick.AddListener(() =>
        {
            Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest3.prefab", await: true);
            
        });
    }

    public override void OnCover(object userData = null)
    {
       
        Z.Debug.Log("MainMenu OnCover");
    }

    public override void OnReveal(object userData = null)
    {
        
        Z.Debug.Log("MainMenu OnReveal");
    }

    public override void OnSwitch(object userData = null)
    {
        //禁用默认的切换行为
        //base.OnSwitch(userData);
    }
   
}