using System.Collections;
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
    PanelBase pnlTest1 = null;
    PanelBase pnlTest2 = null;
    PanelBase pnlTest3 = null;

    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        mBtnTest1.onClick.AddListener(() =>
        {
            if (/*pnlTest1 == null*/true)
                pnlTest1 = Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest1.prefab", await: true);
            else
                Open(pnlTest1);
        });
        mBtnTest2.onClick.AddListener(() =>
        {
            if (/*pnlTest2 == null*/true)
                pnlTest2 = Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest2.prefab", await: true);
            else
                Open(pnlTest2);
        });
        mBtnTest3.onClick.AddListener(() =>
        {
            if (/*pnlTest3 == null*/true)
                pnlTest3 = Open("Assets/GameData/Prefabs/UGUI/Panel/PnlTest3.prefab", await: true);
            else
                Open(pnlTest3);
        });
    }

    public override void OnCover(object userData = null)
    {
        //base.OnCover(userData);
        //CanvasGroup.blocksRaycasts = false;
        Z.Debug.Log("MainMenu OnCover");
    }

    public override void OnReveal(object userData = null)
    {
        //base.OnReveal(userData);
        //CanvasGroup.blocksRaycasts = true;
        Z.Debug.Log("MainMenu OnReveal");
    }

   //public void OnBtnClick(string path)
   // {


   //    var pnl=  Open(path);
   //     if (ReferenceEquals(path, "Assets/GameData/Prefabs/UGUI/Panel/PnlTest1.prefab")) 
   //     pnlTest1 = pnl;
   //     else if(ReferenceEquals(path, "Assets/GameData/Prefabs/UGUI/Panel/PnlTest2.prefab"))
   //         pnlTest2 = pnl;
   //     else if (ReferenceEquals(path, "Assets/GameData/Prefabs/UGUI/Panel/PnlTest3.prefab"))
   //         pnlTest3 = pnl;
   // }
}