using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zframework;
using System;
public class PanelStart : PanelBase
{
    [SerializeField]private Button mbtnStart = null;
    [SerializeField] private Button mbtnQuit = null;

    public override void OnLoad(object userData = null)
    {
        base.OnLoad(userData);
        mbtnStart.onClick.AddListener(() =>
        {
            Z.Scene.LoadSceneAsync("Assets/Demo/Scenes/TestScene1.unity", _ =>
            {
                CloseSelf();
                Z.Procedure.ChangeProcedure<ProcedurePlay>();              
            });
        });


        mbtnQuit.onClick.AddListener(() => 
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        });
    }

    public override void OnOpen(object userData = null)
    {
        base.OnOpen(userData);
      
    }
    public override void OnClose(object userData = null)
    {
        base.OnClose(userData);
        
    }
    public override void OnSwitch(object userData = null)
    {
        //禁用默认的切换行为
        //base.OnSwitch(userData);
    }
}