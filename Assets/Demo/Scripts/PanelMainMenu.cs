using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using System;
public class PanelMainMenu : PanelBase
{
    [SerializeField] private Button mBtnTest1;
    [SerializeField] private Button mBtnTest2;
    [SerializeField] private Button mBtnTest3;

    public override void OnPause(object userData = null)
    {
        base.OnPause(userData);
        CanvasGroup.blocksRaycasts = false;
    }

    public override void OnResume(object userData = null)
    {
        base.OnResume(userData);
        CanvasGroup.blocksRaycasts = true;
    }

   public void OnBtnClick(string path)
    {
        Z.UI.OpenUI(path);
    }
}